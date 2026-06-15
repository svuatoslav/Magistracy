namespace Assets.Scripts.DataSerialiizers
{
    public interface IDataManager
    {
        T? Read<T>(string path) where T : class;
        void Write<T>(T data, string path) where T : class;
    }
}
