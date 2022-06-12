using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Test.Models;

public class User : IUser
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Password { get; set; } = null!;

}
