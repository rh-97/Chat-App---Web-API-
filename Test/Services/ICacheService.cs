namespace Test.Services;

public interface ICacheService
{
    Task Set<T>(string key, T value);
    Task<List<string>> getAll();
    Task Delete(string key);
}
