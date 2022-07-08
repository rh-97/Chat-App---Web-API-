using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Security.Cryptography;
using System.Text;
using Test.Database;
using Test.Helpers;
using Test.Models;

namespace Test.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _usersCollection;


        public UserService(IOptions<DatabaseSettings> _dbsettings)
        {
            var settings = MongoClientSettings.FromConnectionString(_dbsettings.Value.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var db = client.GetDatabase(_dbsettings.Value.DatabaseName);
            _usersCollection = db.GetCollection<User>(_dbsettings.Value.CollectionName[0]);
        }



        // Register New User
        public async Task<bool> Register(User user)
        {
            user.Password = UserHelpers.GenerateHash(user.Password);
            return await _usersCollection.InsertOneAsync(user)
                .ContinueWith(r => r.IsCompletedSuccessfully);
        }



        // Get a list of users for a given page number and number of users per page
        public async Task<List<UserGet>> Fetch(int itemsPerPage = 15, int pageNumber = 1)
        {
            var projection = Builders<User>.Projection.Expression(p => new UserGet
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth
            });

            return await _usersCollection.Find(_ => true)
                .Project(projection)
                .Skip((pageNumber-1)*itemsPerPage)
                .Limit(itemsPerPage)
                .ToListAsync();
        }



        // Get all the info for a specific user identified by Id
        public async Task<UserGet> FetchById(string Id)
        {
            var projection = Builders<User>.Projection.Expression(p => new UserGet
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth
            });
            var filter = Builders<User>.Filter.Eq<string>("Id", Id);
            return await _usersCollection.Find(filter).Project(projection).FirstOrDefaultAsync();
        }



        // Delete a specific user identified by an Id
        public async Task<DeleteResult> Remove(string Id)
        {
            var filter = Builders<User>.Filter.Eq<string>("Id", Id);
            var res = await _usersCollection.DeleteOneAsync(filter);
            return res;
        }



        // Update a specific user data 
        public async Task<bool> Update(UserUpdate user)
        {
            var update = Builders<User>.Update
                .Set("Email", user.Email)
                .Set("DateOfBirth", user.DateOfBirth);

            var res = await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);

            return res.IsAcknowledged;
        }


        // Check username is unique or not for register
        public async Task<bool> IsUniqueUsername(string name)
        {
            var filter = Builders<User>.Filter.Eq<string>("Name", name);
            var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();
            if (user != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        // Check email is unique or not for register
        public async Task<bool> IsUniqueEmail(string email)
        {
            var filter = Builders<User>.Filter.Eq<string>("Email", email);
            var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();
            if (user != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        // Check email is unique or not for register
        public async Task<bool> IsUniqueEmailForUpdate(UserUpdate userUpdate)
        {
            var filter = (Builders<User>.Filter.Ne<string>("Id", userUpdate.Id) &
                        Builders<User>.Filter.Eq<string>("Email", userUpdate.Email));
            //u => (u.Id != userUpdate.Id && u.Email == userUpdate.Email)

            var user = await _usersCollection.Find(filter)
                .FirstOrDefaultAsync();
            
            if (user != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
       
    }
}
