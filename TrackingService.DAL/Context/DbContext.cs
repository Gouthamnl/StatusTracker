using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using TrackingService.DAL.Models;

namespace TrackingService.DAL.Context
{
    public interface IDbContext
    {
        IMongoCollection<StatusDataModel> Status { get; }
    }
    public class DbContext : IDbContext
    {
        private readonly IMongoDatabase _db;
        private readonly string _databaseName;
        public DbContext(IOptions<DatabaseModel> options)
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(options.Value.MongoConnectionString));
            var client = new MongoClient(settings);
            _databaseName = options.Value.Database;
            _db = client.GetDatabase(_databaseName);
        }

        public IMongoCollection<StatusDataModel> Status => _db.GetCollection<StatusDataModel>(_databaseName);
    }
}
