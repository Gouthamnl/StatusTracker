using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logger;
using TrackingService.DAL.Context;
using TrackingService.DAL.Models;

namespace TrackingService.DAL.Repository
{
    public class ConcurDatabaseRepository
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILog _logManager;

        public ConcurDatabaseRepository(ILog logManager, IConnectionManager connectionManager)
        {
            _logManager = logManager;
            _connectionManager = connectionManager;
        }

        public List<Company> GetCompanyConnections()
        {
            var dbQuery = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT
            ID,
            CompanyName
            FROM Company(nolock)");
            var dbDetails = _connectionManager.Handle<Company>(dbQuery);
            return dbDetails.OrderBy(x => x.CompanyName).ToList();
        }
    }
}
