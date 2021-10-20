using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using Logger.Serialization;
using TrackingService.DAL.Models;
using TrackingService.DAL.Repository;
using TrackingService.MessageBroker;
using TrackingService.Models;

namespace TrackingService.Operations
{

    public interface IOperation
    {
        Task DoOperation(StatusMessageModel request, CancellationToken cancellationToken);
    }
    public class StatusUpdateOperation : IOperation
    {
        private readonly IStatusTrackerRepository _statusTrackerRepository;
        private readonly ISerializationManager _serializationManager;
        private readonly ILog _logManager;

        public StatusUpdateOperation(ILog logger, ISerializationManager serializationManager, IMessageBroker messageBroker, IStatusTrackerRepository statusTrackerRepository, IOptions<MessageBrokerOptions> options)
        {
            _logManager = logger;
            _serializationManager = serializationManager;
            _statusTrackerRepository = statusTrackerRepository;
        }
        public async Task DoOperation(StatusMessageModel request, CancellationToken cancellationToken)
        {
            var mes = _serializationManager.Serialize(request);

            if (request.Service.Equals(ServiceTypes.RecoveryEngine.ToString(), StringComparison.InvariantCultureIgnoreCase)) //ServiceTypes.DownloadEngine.ToString() temporary change. Remove when DowloadEngine starts logging.
            {
                var processStatus = request.Status.ToLower().Equals(Status.Success.ToString().ToLower()) ? ProcessStatus.InProgress.ToString() : ProcessStatus.PartiallyCompleted.ToString();
                var statusModel = new StatusDataModel()
                {
                    CompanyId = request.CompanyId,
                    TrackingId = request.TrackingId,
                    ExpenseId = request.ExpenseId,
                    ReportId = request.ReportId,
                    CurrentService = request.Service,
                    Status = request.Status,
                    RecoveryEngine = request.ProcessedDate, // temporary -replace with DownloadEngine
                    StartDate = request.ProcessedDate,
                    Process = processStatus,
                    Message = request.Message
                };
                await _statusTrackerRepository.Create(statusModel, cancellationToken);
            }
            else
            {
                var statusModel = new StatusDataModel()
                {
                    ExpenseId = request.ExpenseId,
                    CurrentService = request.Service,
                    ProcessedDate = request.ProcessedDate,
                    Status = request.Status,
                    Message = request.Message
                };
                await _statusTrackerRepository.Update(statusModel, cancellationToken);
            }
            //var output = await _statusTrackerRepository.Exist(request.ExpenseId, cancellationToken);
        }
    }
}
