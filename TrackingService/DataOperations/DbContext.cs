using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using TrackingService.Models;

namespace TrackingService.DataOperations
{
    public interface IDbContextOld
    {
        IMongoCollection<StatusDataModelOld> Status { get; }
    }
    public class DbContextOld : IDbContextOld
    {
        private readonly IMongoDatabase _db;
        private readonly string _databaseName;
        public DbContextOld(IOptions<DatabaseModelOld> options)
        {
            var client = new MongoClient(options.Value.MongoConnectionString);
            _databaseName = options.Value.Database;
            _db = client.GetDatabase(_databaseName);
        }

        public IMongoCollection<StatusDataModelOld> Status => _db.GetCollection<StatusDataModelOld>(_databaseName);
    }
}
