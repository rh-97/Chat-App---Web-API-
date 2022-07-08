using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Test.Models;

public class Token : IToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string RefreshToken { get; set; } = String.Empty;
    public DateTime ExpireDateTime { get; set; }
}
