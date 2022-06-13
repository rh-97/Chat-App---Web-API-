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



        // Register New User
        public async Task<int> Register(User user)
        {
            var filter = (Builders<User>.Filter.Eq<string>("Name", user.Name) | 
                          Builders<User>.Filter.Eq<string>("Email", user.Email));

            User? user0 = collection.Find(filter).FirstOrDefault();

            if (user0 == null)
            {
                bool success = false;
                user.Password = GenerateHash(user.Password);
                await collection.InsertOneAsync(user).ContinueWith(r => success = r.IsCompletedSuccessfully);
                return (success ? 1 : -1);
            }
            else
            {
                return 0;
            }

        }



        // Get a list of users for a given page number and number of users per page
        public async Task<List<User>> Fetch(int itemsPerPage = 15, int pageNumber = 1)
        {
            return await collection.Find(_ => true).Skip((pageNumber-1)*itemsPerPage).Limit(itemsPerPage).ToListAsync();
        }



        // Get all the info for a specific user identified by Id
        public async Task<User> FetchById(string Id)
        {
            var filter = Builders<User>.Filter.Eq<string>("Id", Id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }


        // Verify login credentials(username & password
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



        // Delete a specific user identified by an Id
        public async Task<DeleteResult> Remove(string Id)
        {
            var filter = Builders<User>.Filter.Eq<string>("Id", Id);
            var res = await collection.DeleteOneAsync(filter);
            return res;
        }



        // Update a specific user data 
        public async Task<User> Update(User user)
        {
            return user;
        }






        // A helper function to generate password hash
        private string GenerateHash(string password)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("Learnathon"));
            var temp = Encoding.UTF8.GetBytes(password);
            var hash = hmac.ComputeHash(temp);
            return BitConverter.ToString(hash, 0, hash.Length).Replace("-", String.Empty);
        }

       
    }
}
