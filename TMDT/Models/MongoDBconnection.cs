using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using TMDT.Models;
using TMDT.MongoDB;

namespace TMDT.Models
{
    public class MongoDBconnection
    {
        private IMongoCollection<BsonDocument> _collection;
        public List<Bmessage> lsMessage = new List<Bmessage>();
        public List<LikeProduct> lsLikeProducts = new List<LikeProduct>();

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

        private IMongoCollection<BsonDocument> getCollectionLikeProduct()
        {
            var connectionString = "mongodb+srv://popy002:popy002@cluster0.sjbw4z0.mongodb.net/";

            // Tạo kết nối đến MongoDB
            var client = new MongoClient(connectionString);

            // Chọn cơ sở dữ liệu
            var database = client.GetDatabase("Chat");
            var collection = database.GetCollection<BsonDocument>("LikeProduct");

            return collection;
        }

        public void getDB()
        {
            var collection = getCollectionMessage();

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

            collection = getCollectionLikeProduct();

            var allLikeProduct = collection.Find(new BsonDocument()).ToList();
            
            foreach(var item in allLikeProduct) {
                LikeProduct like = new LikeProduct {
                    idPro = item["idPro"].AsInt32,
                    namePro = item["namePro"].AsString,
                    idUser = item["idUser"].AsString,
                    status = item["status"].AsBoolean
                };

                this.lsLikeProducts.Add(like);
            }
        }

        public void AddCollectionLikeProduct(LikeProduct likeProduct)
        {
            var collection = getCollectionMessage();

            string formattedPhoneNumber = likeProduct.idUser.PadLeft(10, '0');

            // Chuyển đổi từ đối tượng Bmessage sang BsonDocument
            BsonDocument document = new BsonDocument
            {
                { "idUser", formattedPhoneNumber },
                { "idPro", likeProduct.idPro },
                { "namePro", likeProduct.namePro },
                { "status", likeProduct.status }
            };

            // Thêm document vào collection
            collection.InsertOne(document);
        }

        public bool UpdateStatus(int productId, string userId, bool newStatus)
        {
            bool test = false;

            var collection = getCollectionLikeProduct();

            var filter = Builders<BsonDocument>.Filter.Eq("idPro", productId) & Builders<BsonDocument>.Filter.Eq("idUser", userId);
            var update = Builders<BsonDocument>.Update.Set("status", newStatus);

            var result = collection.UpdateOne(filter, update);

            if (result.ModifiedCount == 1) {
                test = true;
            }

            return test;
        }

        public bool CheckIfExists(int productId, string userId)
        {
            bool test = false;

            var collection = getCollectionLikeProduct();

            var filter = Builders<BsonDocument>.Filter.Eq("idPro", productId) & Builders<BsonDocument>.Filter.Eq("idUser", userId);

            var result = collection.Find(filter).FirstOrDefault();

            if (result != null)
                test = true;

            return test; // Nếu result khác null, bản ghi tồn tại
        }

        public void AddCollectionMessage(Bmessage bmessage)
        {
            var collection = getCollectionMessage();

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