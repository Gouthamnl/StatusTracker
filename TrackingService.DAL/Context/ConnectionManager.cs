using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Logger;
using Logger.Models;
using TrackingService.DAL.Models;

namespace TrackingService.DAL.Context
{
    public interface IConnectionManager
    {
        IEnumerable<T> Handle<T>(string query, CancellationToken cancellationToken, ConcurClientDbConnectionModel clientConnectionDetails = null);
        IEnumerable<T> Handle<T>(string query, ConcurClientDbConnectionModel clientConnectionDetails = null);
    }
    public class ConnectionManager : IConnectionManager
    {
        private readonly ILog _logManager;
        private readonly DatabaseModel _concurServicedatabase;

        public ConnectionManager(ILog logManager, IOptions<DatabaseModel> concurServicedatabase)
        {
            _logManager = logManager;
            _concurServicedatabase = concurServicedatabase.Value;
        }

        public IEnumerable<T> Handle<T>(string query, CancellationToken cancellationToken, ConcurClientDbConnectionModel clientConnectionDetails = null)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                IEnumerable<T> data = default(IEnumerable<T>);
                using (var _connection = new SqlConnection(_concurServicedatabase.ConcurConnection))
                {
                    data = _connection.Query<T>(query);
                }
                return data;
            }
            catch (Exception e)
            {
                _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{e.Message} connection: {clientConnectionDetails.DatabaseName} Query : {query}",
                    Source = "Database Operation",
                    EventType = EventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 110
                }, cancellationToken);
                return null;
            }
            finally
            {
                // do something here
            }
        }

        public IEnumerable<T> Handle<T>(string query, ConcurClientDbConnectionModel clientConnectionDetails = null)
        {
            try
            {
                IEnumerable<T> data = default(IEnumerable<T>);
                using (var _connection = new SqlConnection(_concurServicedatabase.ConcurConnection))
                {
                    data = _connection.Query<T>(query);
                }
                return data;
            }
            catch (Exception e)
            {
                _logManager.LogErrorAsync(new LogModel
                {
                    Message = $"{e.Message} connection: {clientConnectionDetails.DatabaseName} Query : {query}",
                    Source = "Database Operation",
                    EventType = EventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 110
                });
                return null;
            }
            finally
            {
                // do something here
            }
        }
    }
}
