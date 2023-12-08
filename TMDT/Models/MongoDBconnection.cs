using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using TMDT.Models;

namespace TMDT.Models
{
    public class MongoDBconnection
    {
        private IMongoCollection<BsonDocument> _collection;
        public List<Bmessage> lsMessage = new List<Bmessage>();

        public MongoDBconnection() {
            //_collection = getCollectionMessage();
            getDB();
        }

        private IMongoCollection<BsonDocument> getCollectionMessage()
        {
            var connectionString = "mongodb+srv://popy002:popy002@cluster0.sjbw4z0.mongodb.net/";

            // Tạo kết nối đến MongoDB
            var client = new MongoClient(connectionString);

            // Chọn cơ sở dữ liệu
            var database = client.GetDatabase("Chat");
            var collection = database.GetCollection<BsonDocument>("message");

            return collection;
        }

        public void getDB()
        {
            var connectionString = "mongodb+srv://popy002:popy002@cluster0.sjbw4z0.mongodb.net/";

            // Tạo kết nối đến MongoDB
            var client = new MongoClient(connectionString);

            // Chọn cơ sở dữ liệu
            var database = client.GetDatabase("Chat");
            var collection = database.GetCollection<BsonDocument>("message");

            // Lấy danh sách tên của các collection trong database
            var allDocuments = collection.Find(new BsonDocument()).ToList();

            foreach (var document in allDocuments) {
                // Chuyển đổi từ document BSON thành đối tượng Bmessage
                Bmessage message = new Bmessage {
                    user_id = document["user_id"].AsString,
                    message = document["message"].AsString,
                    timestamp = document["timestamp"].ToUniversalTime(), // Chuyển về dạng Universal Time
                    decentral = document["decentral"].AsInt32
                };

                this.lsMessage.Add(message); // Thêm vào danh sách
            }
        }

        public void AddCollectionMessage(Bmessage bmessage)
        {
            var connectionString = "mongodb+srv://popy002:popy002@cluster0.sjbw4z0.mongodb.net/";

            // Tạo kết nối đến MongoDB
            var client = new MongoClient(connectionString);

            // Chọn cơ sở dữ liệu
            var database = client.GetDatabase("Chat");
            var collection = database.GetCollection<BsonDocument>("message");

            string formattedPhoneNumber = bmessage.user_id.PadLeft(10, '0');

            // Chuyển đổi từ đối tượng Bmessage sang BsonDocument
            BsonDocument document = new BsonDocument
            {
                { "user_id", formattedPhoneNumber },
                { "message", bmessage.message },
                { "timestamp", bmessage.timestamp },
                { "decentral", bmessage.decentral }
            };
            
            // Thêm document vào collection
            collection.InsertOne(document);
        }
    }
}