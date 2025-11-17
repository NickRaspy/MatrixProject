using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JSONTool
{

    public static T LoadFromJson<T>(string json, JsonSerializerSettings settings = null)
    {
        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public static void SaveToJson(object obj, string filename, Formatting formatting = Formatting.None, JsonSerializerSettings settings = null)
    {
        string json = JsonConvert.SerializeObject(obj, formatting, settings);
        string directory = Application.streamingAssetsPath;
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        string path = Path.Combine(directory, $"{filename}.json");
        File.WriteAllText(path, json);
    }
}