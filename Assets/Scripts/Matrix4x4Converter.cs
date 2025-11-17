using Newtonsoft.Json;
using UnityEngine;


namespace MatrixProject
{
    public class Matrix4x4Converter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(Matrix4x4);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Matrix4x4 m = (Matrix4x4)value;
            writer.WriteStartObject();
            writer.WritePropertyName("m00"); writer.WriteValue(m.m00);
            writer.WritePropertyName("m01"); writer.WriteValue(m.m01);
            writer.WritePropertyName("m02"); writer.WriteValue(m.m02);
            writer.WritePropertyName("m03"); writer.WriteValue(m.m03);
            writer.WritePropertyName("m10"); writer.WriteValue(m.m10);
            writer.WritePropertyName("m11"); writer.WriteValue(m.m11);
            writer.WritePropertyName("m12"); writer.WriteValue(m.m12);
            writer.WritePropertyName("m13"); writer.WriteValue(m.m13);
            writer.WritePropertyName("m20"); writer.WriteValue(m.m20);
            writer.WritePropertyName("m21"); writer.WriteValue(m.m21);
            writer.WritePropertyName("m22"); writer.WriteValue(m.m22);
            writer.WritePropertyName("m23"); writer.WriteValue(m.m23);
            writer.WritePropertyName("m30"); writer.WriteValue(m.m30);
            writer.WritePropertyName("m31"); writer.WriteValue(m.m31);
            writer.WritePropertyName("m32"); writer.WriteValue(m.m32);
            writer.WritePropertyName("m33"); writer.WriteValue(m.m33);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            Matrix4x4 m = new Matrix4x4();
            reader.Read();
            while (reader.TokenType == JsonToken.PropertyName)
            {
                string prop = (string)reader.Value;
                reader.Read();
                switch (prop)
                {
                    case "m00": m.m00 = (float)(double)reader.Value; break;
                    case "m01": m.m01 = (float)(double)reader.Value; break;
                    case "m02": m.m02 = (float)(double)reader.Value; break;
                    case "m03": m.m03 = (float)(double)reader.Value; break;
                    case "m10": m.m10 = (float)(double)reader.Value; break;
                    case "m11": m.m11 = (float)(double)reader.Value; break;
                    case "m12": m.m12 = (float)(double)reader.Value; break;
                    case "m13": m.m13 = (float)(double)reader.Value; break;
                    case "m20": m.m20 = (float)(double)reader.Value; break;
                    case "m21": m.m21 = (float)(double)reader.Value; break;
                    case "m22": m.m22 = (float)(double)reader.Value; break;
                    case "m23": m.m23 = (float)(double)reader.Value; break;
                    case "m30": m.m30 = (float)(double)reader.Value; break;
                    case "m31": m.m31 = (float)(double)reader.Value; break;
                    case "m32": m.m32 = (float)(double)reader.Value; break;
                    case "m33": m.m33 = (float)(double)reader.Value; break;
                }
                reader.Read();
            }
            return m;
        }
    }
}