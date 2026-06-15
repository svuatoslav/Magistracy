using System;
using System.IO;
using UnityEditor.Overlays;

namespace Assets.Scripts.DataSerialiizers
{
    public class TXTManager : IDataManager
    {
        private readonly string _format = ".txt";

        public T Read<T>(string path) where T : class
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (DirectoryNotFoundException) { return null; }
            catch (FileNotFoundException) { return null; }
            catch (Exception) { return null; }
        }

        public void Write<T>(T data, string path) where T : class
        {
            string fullPath = Path.Combine(path, _format);
            try
            {
                using var sw = new StreamWriter(fullPath);
                { sw.Write(data.ToString()); }
            }
            catch (DirectoryNotFoundException) { }
            catch (InvalidOperationException) { }
            catch (Exception) { }
        }
    }
}
