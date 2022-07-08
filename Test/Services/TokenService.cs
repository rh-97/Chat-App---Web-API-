using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Test.Database;
using Test.Helpers;
using Test.Models;

namespace Test.Services;

public class TokenService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly IMongoCollection<Token> _tokensCollection;

    public TokenService(IOptions<DatabaseSettings> _dbsettings)
    {
        var settings = MongoClientSettings.FromConnectionString(_dbsettings.Value.ConnectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        var db = client.GetDatabase(_dbsettings.Value.DatabaseName);
        _usersCollection = db.GetCollection<User>(_dbsettings.Value.CollectionName[0]);
        _tokensCollection = db.GetCollection<Token>(_dbsettings.Value.CollectionName[1]);
    }



    // Verify login credentials(username & password
    public async Task<bool> VerifyLogin(UserLogin user)
    {
        var filter = (Builders<User>.Filter.Eq<string>("Name", user.Name) &
                       Builders<User>.Filter.Eq<string>("Password", UserHelpers.GenerateHash(user.Password)));
        var res = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        if (res == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public async Task<bool> SaveRefreshToken(Token refreshToken)
    {
        return await _tokensCollection.InsertOneAsync(refreshToken)
                .ContinueWith(r => r.IsCompletedSuccessfully);
    }

    public async Task<bool> DaleteRefreshToken(string name, string refreshToken)
    {
        var result = await _tokensCollection.DeleteOneAsync(t => t.Name == name && t.RefreshToken == refreshToken);
        return result.DeletedCount > 0;
    }

    public async Task<Token> GetToken(string name, string refreshToken)
    {
        Token token = await _tokensCollection.Find(t => t.Name == name && t.RefreshToken == refreshToken).FirstOrDefaultAsync();
        return token;
    }
}
