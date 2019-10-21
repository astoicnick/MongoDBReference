using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZenTube.Repository
{
    public class Mongo
    {
        private IMongoDatabase db;
        public Mongo(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }
        public async void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
             await collection.InsertOneAsync(record);
        }
        public async Task<T> GetRecord<T>(string table, string keyword)
        {
            var collection = db.GetCollection<T>(table);
            //Can't deserialize the record into a string -- duh
            var filter = Builders<T>.Filter.Eq("TitleOfVideo", keyword);
            return await collection.FindAsync<T>(filter).Result.FirstAsync();

        }
        public async Task<List<T>> SearchRecords<T>(string table, string keyword)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Title", keyword);
            return await collection.FindAsync<T>(filter).Result.ToListAsync();
        }
        public async Task<List<T>> GetAllRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return await collection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task<string> UpsertRecord(string table,  TestObject toUpdate)
        {
            var collection = db.GetCollection<TestObject>(table);
            var filter = Builders<TestObject>.Filter.In("TitleOfVideo", toUpdate.TitleOfVideo);
            var updateFunction = await collection.ReplaceOneAsync(filter, toUpdate, new UpdateOptions { IsUpsert = true });
            return updateFunction.UpsertedId.AsString;
        }
    }
    public class TestObject
    {
        [BsonId]
        public Guid TestID { get; set; }
        [BsonElement("Title")]
        public string TitleOfVideo { get; set; }
    }
}
