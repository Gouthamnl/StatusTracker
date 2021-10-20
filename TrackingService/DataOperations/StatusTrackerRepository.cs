using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using TBI.Logger;
using TBI.Logger.Models;
using TrackingService.Models;

namespace TrackingService.DataOperations
{
    public interface IStatusTrackerRepositoryOld
    {
        Task Create(StatusDataModelOld status, CancellationToken cancellationToken);
        Task<bool> Update(StatusDataModelOld status, CancellationToken cancellationToken);
        //Task<bool> UpdateAsImageDownloader(StatusDataModel status, CancellationToken cancellationToken);
        //Task<bool> UpdateAsPdfSplitter(StatusDataModel status, CancellationToken cancellationToken);
        //Task<bool> UpdateAsML(StatusDataModel status, CancellationToken cancellationToken);
        //Task<bool> UpdateRuleEngine(StatusDataModel status, CancellationToken cancellationToken);
        Task<StatusDataModelOld> Exist(string expenseId, CancellationToken cancellationToken);
    }
    public class StatusTrackerRepositoryOld : IStatusTrackerRepositoryOld
    {
        private readonly IDbContextOld _context;
        private readonly ILog _logManager;

        public StatusTrackerRepositoryOld(IDbContextOld context, ILog logManager)
        {
            _context = context;
            _logManager = logManager;
        }

        public async Task<StatusDataModelOld> Exist(string expenseId, CancellationToken cancellationToken)
        {
            try
            {
                FilterDefinition<StatusDataModelOld> filter = Builders<StatusDataModelOld>.Filter.Eq(m => m.ExpenseId, expenseId);
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
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
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
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
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
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, cancellationToken);
                return null;
            }
        }

        public async Task Create(StatusDataModelOld status, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Status.InsertOneAsync(status);
            }
            catch(MongoConnectionException mce)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mce.Message}",
                    Source = "Database Operation",
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, cancellationToken);
            }
            catch(MongoWriteException mwe)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mwe.Message}",
                    Source = "Database Operation",
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, cancellationToken);
            }
            catch(Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{ex.Message}",
                    Source = "Database Operation",
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, cancellationToken);
            }
        }

        #region UnnecessaryMethods

        //public async Task<bool> UpdateAsPdfSplitter(StatusDataModel status, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        UpdateResult updateResult =
        //            await _context
        //                    .Status
        //                    .UpdateOneAsync(
        //                        Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
        //                        Builders<StatusDataModel>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
        //                                                        .Set(m => m.PdfSplitter, status.PdfSplitter));
        //        return updateResult.IsAcknowledged
        //                && updateResult.ModifiedCount > 0;
        //    }
        //    catch (MongoConnectionException mce)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mce.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (MongoWriteException mwe)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mwe.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{ex.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //}

        //public async Task<bool> UpdateAsImageDownloader(StatusDataModel status, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        UpdateResult updateResult =
        //            await _context
        //                    .Status
        //                    .UpdateOneAsync(
        //                        Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
        //                        Builders<StatusDataModel>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
        //                                                        .Set(m => m.ImageDownloader, status.ImageDownloader));
        //        return updateResult.IsAcknowledged
        //                && updateResult.ModifiedCount > 0;
        //    }
        //    catch (MongoConnectionException mce)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mce.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (MongoWriteException mwe)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mwe.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{ex.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //}

        //public async Task<bool> UpdateAsML(StatusDataModel status, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        UpdateResult updateResult =
        //            await _context
        //                    .Status
        //                    .UpdateOneAsync(
        //                        Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
        //                        Builders<StatusDataModel>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
        //                                                        .Set(m => m.ML, status.ML));
        //        return updateResult.IsAcknowledged
        //                && updateResult.ModifiedCount > 0;
        //    }
        //    catch (MongoConnectionException mce)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mce.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (MongoWriteException mwe)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mwe.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{ex.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //}

        //public async Task<bool> UpdateRuleEngine(StatusDataModel status, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        UpdateResult updateResult =
        //            await _context
        //                    .Status
        //                    .UpdateOneAsync(
        //                        Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
        //                        Builders<StatusDataModel>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
        //                                                        .Set(m => m.RulesEngine, status.RulesEngine));
        //        return updateResult.IsAcknowledged
        //                && updateResult.ModifiedCount > 0;
        //    }
        //    catch (MongoConnectionException mce)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mce.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (MongoWriteException mwe)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{mwe.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logManager.LogErrorAsync(new LogModel
        //        {
        //            Message = $"{ex.Message}",
        //            Source = "Database Operation",
        //            EventType = QueueReaderEventType.Exception,
        //            LogCategory = LogType.Critical,
        //            HttpStatusCode = 500
        //        }, cancellationToken);
        //        return false;
        //    }
        //}

        #endregion

        public async Task<bool> Update(StatusDataModelOld status, CancellationToken cancellationToken)
        {
            UpdateResult updateResult = null;
            try
            {

                if (status.CurrentStatus.ToLower().Equals(ServiceTypesOld.ImageDownloader.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModelOld>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModelOld>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
                                                                .Set(m => m.ImageDownloader, status.ProcessedDate)
                                                                .Set(m => m.Process, ProcessStatusOld.InProgress.ToString()));
                }
                else if (status.CurrentStatus.ToLower().Equals(ServiceTypesOld.PdfSplitter.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModelOld>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModelOld>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
                                                                .Set(m => m.PdfSplitter, status.ProcessedDate)
                                                                .Set(m => m.Process, ProcessStatusOld.InProgress.ToString()));
                }
                else if (status.CurrentStatus.ToLower().Equals(ServiceTypesOld.ML.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModelOld>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModelOld>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
                                                                .Set(m => m.ML, status.ProcessedDate)
                                                                .Set(m => m.Process, ProcessStatusOld.InProgress.ToString()));
                }
                else if (status.CurrentStatus.ToLower().Equals(ServiceTypesOld.RulesEngine.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModelOld>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModelOld>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
                                                                .Set(m => m.RulesEngine, status.ProcessedDate)
                                                                .Set(m => m.Process, ProcessStatusOld.InProgress.ToString()));
                }
                else if (status.CurrentStatus.ToLower().Equals(ServiceTypesOld.Migration.ToString().ToLower()))
                {
                    updateResult =
                    await _context
                            .Status
                            .UpdateOneAsync(
                                Builders<StatusDataModelOld>.Filter.Eq(m => m.ExpenseId, status.ExpenseId),
                                Builders<StatusDataModelOld>.Update.Set(m => m.CurrentStatus, status.CurrentStatus)
                                                                .Set(m => m.Migration, status.ProcessedDate)
                                                                .Set(m => m.Process, ProcessStatusOld.Completed.ToString()));
                }
                return updateResult.IsAcknowledged
                        && updateResult.ModifiedCount > 0;
            }
            catch (MongoConnectionException mce)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mce.Message}",
                    Source = "Database Operation",
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, cancellationToken);
                return false;
            }
            catch (MongoWriteException mwe)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{mwe.Message}",
                    Source = "Database Operation",
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, cancellationToken);
                return false;
            }
            catch (Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{ex.Message}",
                    Source = "Database Operation",
                    EventType = QueueReaderEventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, cancellationToken);
                return false;
            }
        }
    }
}
