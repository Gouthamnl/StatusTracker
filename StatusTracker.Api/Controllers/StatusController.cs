using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StatusTracker.Api.Models;
using TrackingService.DAL.Models;
using TrackingService.DAL.Repository;

namespace StatusTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusTrackerRepository _statusTrackerRepository;
        protected CancellationToken cancellationToken = CancellationToken.None;

        public StatusController(IStatusTrackerRepository statusTrackerRepository)
        {
            _statusTrackerRepository = statusTrackerRepository;
        }
        // GET: api/Status
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<List<StatusDataModel>>> Get()
        {
            List<StatusDataModel> allStatus = null;
            try
            {
                allStatus = await _statusTrackerRepository.All(cancellationToken);
                //allStatus = allStatus.Skip(10 * (1 - 1))
                //          .Take(10)
                //          .ToList();
                if (allStatus == null || allStatus.Count == 0)
                {
                    return NotFound(allStatus);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(allStatus);
        }

        [Produces("application/json")]
        [HttpPost]
        [Route("Search")]
        public async Task<ActionResult<StatusResultModel>> FilterData([FromBody]FilterModel filterModel, [FromHeader]int pageSize, [FromHeader]int pageNumber, [FromHeader]bool download = false)
        {
            var builder = Builders<StatusDataModel>.Filter;
            List<StatusDataModel> filteredStatus = null;
            var resultModel = new StatusResultModel();
            
            try
            {
                IList<FilterDefinition<StatusDataModel>> filters = new List<FilterDefinition<StatusDataModel>>();
                if (filterModel != null)
                {
                    if (filterModel?.CompanyId >= 0 && string.IsNullOrEmpty(filterModel?.ReportId) && string.IsNullOrEmpty(filterModel?.ExpenseId))
                    {
                        filters.Add(Builders<StatusDataModel>.Filter.Eq(m => m.CompanyId, filterModel.CompanyId));
                    }
                    if (!string.IsNullOrEmpty(filterModel?.ReportId))
                    {
                        var reportIds = filterModel.ReportId.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string reportId in reportIds)
                        {
                            filters.Add(Builders<StatusDataModel>.Filter.Eq(m => m.ReportId, reportId.Trim()));
                        }
                    }
                    if (!string.IsNullOrEmpty(filterModel?.ExpenseId))
                    {
                        var expenseIds = filterModel.ExpenseId.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string expenseId in expenseIds)
                        {
                            filters.Add(Builders<StatusDataModel>.Filter.Eq(m => m.ExpenseId, expenseId.Trim()));
                        }
                    }
                    if (!filterModel.from.Year.Equals(0001) && string.IsNullOrEmpty(filterModel?.ReportId) && string.IsNullOrEmpty(filterModel?.ExpenseId))
                    {
                        filters.Add(Builders<StatusDataModel>.Filter.Gte(m => m.StartDate, filterModel.from));
                        filters.Add(Builders<StatusDataModel>.Filter.Lte(m => m.StartDate, filterModel.to));
                    }
                }
                else
                {
                    return BadRequest();
                }
                var filterConcat = builder.And(filters);
                filteredStatus = await _statusTrackerRepository.FilterAsync(filterConcat, pageSize, pageNumber, cancellationToken);

                if (filteredStatus == null || filteredStatus?.Count == 0)
                {
                    return Ok(resultModel);
                }
                resultModel.TotalCount = filteredStatus.Count();
                
                if(download)
                {
                    resultModel.StatusDataModel = filteredStatus;
                }
                else if (pageSize != 0 && pageNumber != 0)
                {
                    resultModel.StatusDataModel = filteredStatus.Skip(pageSize * (pageNumber - 1))
                              .Take(pageSize)
                              .ToList();
                }
                else
                {
                    resultModel.StatusDataModel = filteredStatus;
                }
                //resultModel.Success = filteredStatus.Count(x => x.Process.Equals(ProcessStatus.Completed.ToString()));
                //resultModel.InProgress = filteredStatus.Count(x => x.Process.Equals(ProcessStatus.InProgress.ToString()));
                //resultModel.Failure = filteredStatus.Count(x => x.Status.Equals(Status.Failed.ToString()));

                return Ok(resultModel);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
