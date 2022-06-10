using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Security.Cryptography;
using System.Text;
using Test.Database;
using Test.Models;

namespace Test.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> collection;

        public UserService(IOptions<DatabaseSettings> _dbsettings)
        {
            var settings = MongoClientSettings.FromConnectionString(_dbsettings.Value.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var db = client.GetDatabase(_dbsettings.Value.DatabaseName);
            collection = db.GetCollection<User>(_dbsettings.Value.CollectionName);
        }

        public async Task Register(User user)
        {
            var filter = (Builders<User>.Filter.Eq<string>("Name", user.Name) | 
                          Builders<User>.Filter.Eq<string>("Email", user.Email));

            User? user0 = collection.Find(filter).FirstOrDefault();

            if (user0 == null)
            {
                user.Password = GenerateHash(user.Password);
                await collection.InsertOneAsync(user);
            }
            else
            {
                throw new Exception("User already exists.");
            }
            return;
        }

        public async Task<List<User>> Fetch(int itemsPerPage = 15, int pageNumber = 1)
        {
            return await collection.Find(_ => true).Skip((pageNumber-1)*itemsPerPage).Limit(itemsPerPage).ToListAsync();
        }

        public async Task<User> FetchById(string Id)
        {
            var filter = Builders<User>.Filter.Eq<string>("Id", Id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> VerifyLogin(UserLogin user)
        {
            var filter = (Builders<User>.Filter.Eq<string>("Name", user.Name) & 
                           Builders<User>.Filter.Eq<string>("Password", GenerateHash(user.Password)));
            var res = await collection.Find(filter).FirstOrDefaultAsync();

            if (res == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<DeleteResult> Remove(string Id)
        {
            var filter = Builders<User>.Filter.Eq<string>("Id", Id);
            var res = await collection.DeleteOneAsync(filter);
            return res;
        }

        private string GenerateHash(string password)
        {

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("Learnathon"));
            var temp = Encoding.UTF8.GetBytes(password);
            var hash = hmac.ComputeHash(temp);
            return BitConverter.ToString(hash, 0, hash.Length).Replace("-", String.Empty);
        }

       
    }
}
