using Newtonsoft.Json;
using System.IO;

namespace console_flappy_bird.Services
{
    class InputOutputService
    {
        public static void Save(object value, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var ser = new JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public static T Load<T>(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }
    }
}
