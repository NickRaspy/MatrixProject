using Newtonsoft.Json;

namespace MatrixProject
{
    //there were issues with conversion so it has special settings
    public class MatrixJSONSettings : JsonSerializerSettings
    {
        public MatrixJSONSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            MaxDepth = 10;
            Converters = new JsonConverter[] { new Matrix4x4Converter() };
        }
    }
}