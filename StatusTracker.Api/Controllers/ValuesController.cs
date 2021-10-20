using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Logger;
using Logger.Serialization;
using TrackingService.DAL.Context;
using TrackingService.DAL.Models;
using TrackingService.DAL.Repository;

namespace StatusTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ConcurDatabaseRepository _concurDatabaseOperations;
        protected CancellationToken cancellationToken = CancellationToken.None;
        private readonly ILog _logManager;
        private readonly ISerializationManager _serializationManager;

        public ValuesController(ILog log,ISerializationManager serializationManager, IConnectionManager connectionManager)
        {
            _logManager = log;
            _serializationManager = serializationManager;
            _concurDatabaseOperations = new ConcurDatabaseRepository(_logManager, connectionManager);
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        [Route("Companies")]
        public ActionResult<List<Company>> GetCompanies()
        {
            return _concurDatabaseOperations.GetCompanyConnections();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
