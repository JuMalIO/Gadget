using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Gadget.Utilities
{
	public class Json
    {
        public static T DeserializeFile<T>(string path) where T : new()
        {
            try
            {
                return Deserialize<T>(File.ReadAllText(path));
            }
            catch
            {
                return new T();
            }
        }

        public static void SerializeFile<T>(string path, T data)
        {
            try
            {
                File.WriteAllText(path, Serialize(data));
            }
            catch
            {
            }
        }

        public static string Serialize<T>(T data)
		{
            using (var stream = new MemoryStream())
            {
                var json = new DataContractJsonSerializer(typeof(T));
                json.WriteObject(stream, data);
                var bytes = stream.ToArray();
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
		}

        public static T Deserialize<T>(string str)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                var json = new DataContractJsonSerializer(typeof(T));
                return (T)json.ReadObject(stream);
            }
        }
    }
}
