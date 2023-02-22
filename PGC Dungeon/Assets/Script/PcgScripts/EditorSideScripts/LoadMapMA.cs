using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class LoadMapMA : MonoBehaviour
{
    public PCGManager PcgManager;






    public void InspectorAwake()
    {
        PcgManager = this.transform.GetComponent<PCGManager>();
    }



    public Tile[][] LoadDataCall(string fileName) 
    {

        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Error", "The file name given is not valie", "OK");
            return null;
        }
        

        string filePath = Application.dataPath + "/Resources/Resources_Algorithms/Saved_Gen_Data/" + fileName;


        if (File.Exists(filePath))
        {
            byte[] data = File.ReadAllBytes(filePath);
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data);
            SerializableTile[][] serializableMap = (SerializableTile[][])formatter.Deserialize(stream);

            // Convert the serializable map back to a Tile[][] array
            Tile[][] map = new Tile[serializableMap.Length][];
            for (int i = 0; i < serializableMap.Length; i++)
            {
                map[i] = new Tile[serializableMap[i].Length];
                for (int j = 0; j < serializableMap[i].Length; j++)
                {
                    map[i][j] = new Tile(serializableMap[i][j].position, serializableMap[i][j].tileWeight, serializableMap[i][j].cost,serializableMap[i][j].idx, serializableMap[i][j].visited, serializableMap[i][j].tileType);
                }
            }

            return map;
        }
        else 
        {

            EditorUtility.DisplayDialog("Error", "The file name given is not valie", "OK");
        }
        return null;
    }


}
