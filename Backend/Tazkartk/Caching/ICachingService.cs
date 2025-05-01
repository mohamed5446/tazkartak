namespace Tazkartk.Caching
{
    public interface ICachingService
    {
        T? GetData<T>(string key);
        void SetData<T>(string Key, T Data, int minutes);
        void Remove(string Key);


    }
}
