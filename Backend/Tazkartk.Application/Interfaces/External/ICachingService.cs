namespace Tazkartk.Application.Interfaces.External
{
    public interface ICachingService
    {
        T? GetData<T>(string key);
        void SetData<T>(string Key, T Data, int? minutes=null);
        void Remove(string Key);


    }
}
