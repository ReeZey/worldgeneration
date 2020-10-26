using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class SaveSystem
{
    static string savePath = $"./world";

    public static void Save(Tilemap obj)
    {
        BinaryFormatter formattter = new BinaryFormatter();
        string path = $"{savePath}/{obj.name}.lol";


        FileStream stream = new FileStream(path, FileMode.Create);
        BuildingData data = new BuildingData(obj);

        formattter.Serialize(stream, data);
        stream.Close();
    }

    public static BuildingData Load(string objectToLoad)
    {
        string path = $"{savePath}/{objectToLoad}.lol";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            BuildingData data = formatter.Deserialize(stream) as BuildingData;

            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

}