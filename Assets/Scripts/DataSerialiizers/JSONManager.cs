using Newtonsoft.Json;
using System.IO;

namespace Assets.Scripts.DataSerialiizers
{
    public class JSONManager : IDataManager
    {
        private readonly string _format = ".json";
        public T Read<T>(string path) where T : class
        {
            try { return JsonConvert.DeserializeObject<T>(File.ReadAllText(Path.Combine(path + _format))); }
            catch (IOException) { return null; }
            catch { return null; }
        }

        public void Write<T>(T data, string path) where T : class
        {
            try { File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented)); }
            catch (IOException) { }
            catch { }
        }
    }
}
