using Newtonsoft.Json;

namespace MatrixProject
{
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