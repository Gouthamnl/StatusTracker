using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using Logger.Models;
using Logger.Serialization;
using TrackingService.DAL.Context;
using TrackingService.DAL.Models;

namespace TrackingService.DAL.Repository
{
    public interface IStatusTrackerRepository
    {
        Task Create(StatusDataModel status, CancellationToken cancellationToken);
        Task Update(StatusDataModel status, CancellationToken cancellationToken);
        Task<StatusDataModel> Exist(string expenseId, CancellationToken cancellationToken);
        Task<List<StatusDataModel>> FilterAsync(FilterDefinition<StatusDataModel> filter, int pageSize, int pageNumber, CancellationToken cancellationToken);
        Task<List<StatusDataModel>> All(CancellationToken cancellationToken);
    }
    public class StatusTrackerRepository : IStatusTrackerRepository
    {
        private readonly IDbContext _context;
        private readonly ILog _logManager;
        private readonly ISerializationManager _serializationManager;
        private UpdateOptions updateOptions = new UpdateOptions { IsUpsert = true };

        public StatusTrackerRepository(IDbContext context, ILog logManager, ISerializationManager serializationManager)
        {
            _context = context;
            _logManager = logManager;
            _serializationManager = serializationManager;
        }

        public async Task<StatusDataModel> Exist(string expenseId, CancellationToken cancellationToken)
        {
            try
            {
                FilterDefinition<StatusDataModel> filter = Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, expenseId);
                return await
                        _context
                        .Status
                        .Find(filter)
                        .FirstOrDefaultAsync();
            }
            catch (MongoConnectionException mce)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mce.Message}",
                    Source = "Database Operation",
                    ExpenseId = expenseId,
                    EventType = EventType.ClientConnection,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
            catch (MongoWriteException mwe)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mwe.Message}",
                    Source = "Database Operation",
                    ExpenseId = expenseId,
                    EventType = EventType.DatabaseReadFailure,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
            catch (Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{ex.Message}",
                    Source = "Database Operation",
                    ExpenseId = expenseId,
                    EventType = EventType.Exception,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
        }

        public async Task Create(StatusDataModel status, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Status.InsertOneAsync(status, null, cancellationToken);
                await _logManager.LogInfoAsync(new LogModel
                {
                    Message = $"Expense status created in tracking system",
                    Source = "Database Operation",
                    ExpenseId = status.ExpenseId,
                    Service = status.CurrentService,
                    EventType = EventType.DatabaseInsert,
                    LogCategory = LogType.Information,
                    HttpStatusCode = 200
                }, cancellationToken);
            }
            catch (MongoConnectionException mce)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mce.Message}",
                    Source = "Database Operation",
                    ExpenseId = status.ExpenseId,
                    EventType = EventType.ClientConnection,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
            }
            catch (MongoWriteException mwe)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mwe.Message}",
                    Source = "Database Operation",
                    ExpenseId = status.ExpenseId,
                    EventType = EventType.DatabaseInsertFailure,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{ex.Message}",
                    Source = "Database Operation",
                    ExpenseId = status.ExpenseId,
                    EventType = EventType.Exception,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
            }
        }

        public async Task Update(StatusDataModel status, CancellationToken cancellationToken)
        {
            UpdateResult updateResult = null;
            try
            {
                var processStatus = status.Status.ToLower().Equals(Status.Success.ToString().ToLower()) ? ProcessStatus.InProgress.ToString() : ProcessStatus.PartiallyCompleted.ToString();
                if (status.CurrentService.ToLower().Equals(ServiceTypes.RecoveryEngine.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.RecoveryEngine, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.CompanyName, status.CompanyName) //temporary
                                                                .Set(m => m.CompanyId, status.CompanyId) //temporary
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.DatabaseService.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.DatabaseService, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.ImageDownloader.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.ImageDownloader, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.CompanyName, status.CompanyName) //temporary
                                                                .Set(m => m.CompanyId, status.CompanyId) //temporary
                                                                .Set(m => m.StartDate, status.ProcessedDate) //temporary
                                                                .Set(m => m.ReportId, status.ReportId) //temporary
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.GoogleVisionService.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.GoogleVisionService, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.CompanyName, status.CompanyName) //temporary
                                                                .Set(m => m.CompanyId, status.CompanyId) //temporary
                                                                .Set(m => m.StartDate, status.ProcessedDate) //temporary
                                                                .Set(m => m.ReportId, status.ReportId) //temporary
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.VATValidationService.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.VATValidationService, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.CompanyName, status.CompanyName) //temporary
                                                                .Set(m => m.CompanyId, status.CompanyId) //temporary
                                                                .Set(m => m.StartDate, status.ProcessedDate) //temporary
                                                                .Set(m => m.ReportId, status.ReportId) //temporary
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.PdfSplitter.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.PdfSplitter, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.CompanyId, status.CompanyId) //temporary
                                                                .Set(m => m.CompanyName, status.CompanyName) //temporary
                                                                .Set(m => m.StartDate, status.ProcessedDate) //temporary
                                                                .Set(m => m.ReportId, status.ReportId) //temporary
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.IsOriginal.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.IsOriginal, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.CompanyId, status.CompanyId) //temporary
                                                                .Set(m => m.CompanyName, status.CompanyName) //temporary
                                                                .Set(m => m.StartDate, status.ProcessedDate) //temporary
                                                                .Set(m => m.ReportId, status.ReportId) //temporary
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.MLSender.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.MLSender, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.MLReceiver.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.MLReceiver, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.ML.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.ML, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.RulesEngine.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.RulesEngine, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.AutoAudit.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.AutoAudit, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.Process, processStatus), updateOptions, cancellationToken);
                }
                else if (status.CurrentService.ToLower().Equals(ServiceTypes.Migration.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModel>.Update.Set(m => m.CurrentService, status.CurrentService)
                                                                .Set(m => m.Migration, status.ProcessedDate)
                                                                .Set(m => m.Message, status.Message)
                                                                .Set(m => m.Status, status.Status)
                                                                .Set(m => m.Process, ProcessStatus.Completed.ToString()), updateOptions, cancellationToken);
                }
                await _logManager.LogInfoAsync(new LogModel
                {
                    Message = $"Expense current status updated in tracking system",
                    Source = "Database Operation",
                    Service = status.CurrentService,
                    ExpenseId = status.ExpenseId,
                    EventType = EventType.DatabaseUpdate,
                    LogCategory = LogType.Information,
                    HttpStatusCode = 200
                }, cancellationToken);
                //return updateResult.IsAcknowledged; //temporary changes
                        //&& updateResult.ModifiedCount > 0;
            }
            catch (MongoConnectionException mce)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mce.Message}, Source : {mce.Source}, Stacktrace:{mce.StackTrace}",
                    Source = "Database Operation",
                    ExpenseId = status.ExpenseId,
                    EventType = EventType.ClientConnection,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                //return false;
            }
            catch (MongoWriteException mwe)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mwe.Message}, Source : {mwe.Source}, Stacktrace:{mwe.StackTrace}",
                    Source = "Database Operation",
                    ExpenseId = status.ExpenseId,
                    EventType = EventType.DatabaseUpdateFailure,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                //return false;
            }
            catch (Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{ex.Message}, Source : {ex.Source}, Stacktrace:{ex.StackTrace}",
                    Source = "Database Operation",
                    ExpenseId = status.ExpenseId,
                    EventType = EventType.Exception,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                //return false;
            }
        }

        public async Task<List<StatusDataModel>> FilterAsync(FilterDefinition<StatusDataModel> filter, int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            try
            {
                return await
                        _context
                        .Status
                        .Find(filter)
                        //.Skip(pageSize * (pageNumber - 1))
                        //.Limit(pageSize)
                        .ToListAsync();
            }
            catch (MongoConnectionException mce)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mce.Message}",
                    Source = "Database Filter Operation",
                    EventType = EventType.ClientConnection,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
            catch (MongoWriteException mwe)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mwe.Message}",
                    Source = "Database Filter Operation",
                    EventType = EventType.DatabaseUpdateFailure,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
            catch (Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{ex.Message}",
                    Source = "Database Filter Operation",
                    EventType = EventType.Exception,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
        }

        public async Task<List<StatusDataModel>> All(CancellationToken cancellationToken)
        {
            try
            {
                FilterDefinition<StatusDataModel> filter = Builders<StatusDataModel>.Filter.Empty;
                return  await
                        _context
                        .Status                        
                        .Find(filter)
                        .ToListAsync();
            }
            catch (MongoConnectionException mce)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mce.Message}",
                    Source = "Database Operation",
                    EventType = EventType.ClientConnection,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
            catch (MongoWriteException mwe)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mwe.Message}",
                    Source = "Database Operation",
                    EventType = EventType.DatabaseReadFailure,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
            catch (Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{ex.Message}",
                    Source = "Database Operation",
                    EventType = EventType.Exception,
                    LogCategory = LogType.Error,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
        }
    }
}
