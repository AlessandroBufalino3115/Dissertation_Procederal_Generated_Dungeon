using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneralUtil 
{

    public static int[,] childPosArry4Side = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
    public static int[,] childPosArry8Side = { { 0, -1 }, { 1, -1 }, { -1, -1 },  { -1, 0 }, { 1, 0 },    { 0, 1 }, { 1, 1 }, { -1, 1 } };





    /// <summary>
    /// from 0 
    /// </summary>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <returns></returns>
    public static Vector2Int RanVector2Int(int maxX,int maxY) 
    {
        int ranX = Random.Range(0,maxX);
        int ranY = Random.Range(0, maxY);


        return new Vector2Int(ranX, ranY);
    }


    /// <summary>
    /// from 0 
    /// </summary>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <returns></returns>
    public static Vector3Int RanVector3Int(int maxX, int maxY, int maxZ)
    {
        int ranX = Random.Range(0, maxX);
        int ranY = Random.Range(0, maxY);
        int ranZ = Random.Range(0, maxZ);


        return new Vector3Int(ranX, ranY,ranZ);
    }

    public static Vector2 RanVector2Float(float maxX, float maxY)
    {
        float ranX = Random.Range(0f, maxX);
        float ranY = Random.Range(0f, maxY);

        return new Vector2(ranX, ranY);
    }

    public static int PerfTimer(bool start, int startTimer =0) 
    {
        if (start) 
        {
            return Environment.TickCount & Int32.MaxValue;
        }
        else 
        {
            int timerEnd = Environment.TickCount & Int32.MaxValue;

            Debug.Log($"<color=yellow>Performance: This operation took {timerEnd - startTimer} ticks</color>");

            return timerEnd - startTimer;
        }
    }



    public static float EuclideanDistance2D(Vector2 point1, Vector2 point2)
    {
        return MathF.Sqrt(MathF.Pow((point1.x - point2.x), 2) + MathF.Pow((point1.y - point2.y), 2));
    }

    public static float ManhattanDistance2D(Vector2 point1, Vector2 point2)
    {
        return Mathf.Abs((point1.x - point2.x)) + Mathf.Abs((point1.y - point2.y));
    }

    public static void SpacesUILayout(int spaceNum)
    {
        for (int i = 0; i < spaceNum; i++)
        {
            EditorGUILayout.Space();
        }
    }





    /// <summary>
    /// Sets the colour of the pixel that is saved in the class instance
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="gridArray2D"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextSelfCol(Tile[][] gridArray2D)
    {

        Texture2D texture = new Texture2D(gridArray2D[0].Length, gridArray2D.Length);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, gridArray2D[y][x].color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();


        return texture;
    }

    /// <summary>
    /// either black or white, if = 0 white if = 1 black
    /// </summary>
    /// <param name="gridArray2D"></param>
    /// <param name="black"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextBiColAnchor(Tile[][] gridArray2D, bool black = false)
    {

        Texture2D texture = new Texture2D(gridArray2D[0].Length, gridArray2D.Length);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = new Color();

                if (black)
                {
                    color = ((gridArray2D[y][x].tileWeight) == 0 ? Color.white : Color.black);
                }
                else
                {
                    color = ((gridArray2D[y][x].tileWeight) == 0 ? Color.white : Color.grey);
                }

                texture.SetPixel(x, y, color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();


        return texture;
    }

    /// <summary>
    /// Set the shade of black and white with a given max and min weight then weight
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="gridArray2D"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextBiColShade(Tile[][] gridArray2D, float minWeight, float maxWeight, bool inverse = false)
    {

        Texture2D texture = new Texture2D(gridArray2D[0].Length, gridArray2D.Length);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float num = Mathf.InverseLerp(minWeight, maxWeight, gridArray2D[y][x].tileWeight);

                if (inverse)
                    gridArray2D[y][x].color = new Color(1 - num, 1 - num, 1 - num, 1f);
                else
                    gridArray2D[y][x].color = new Color(num, num, num, 1f);


                texture.SetPixel(x, y, gridArray2D[y][x].color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }

    public static int ReturnRandomFromList<T>(List<T> list) 
    {
        return list.Count == 1 ? 0 : Random.Range(0, list.Count);
    }



    public static void SetUpColorBasedOnType(Tile[][] gridArr) 
    {
        for (int y = 0; y < gridArr.Length; y++)
        {
            for (int x = 0; x < gridArr[0].Length; x++)
            {
                switch (gridArr[y][x].tileType)
                {
                    case Tile.TileType.VOID:
                        gridArr[y][x].color = Color.white;
                        break;
                    case Tile.TileType.FLOORROOM:

                        gridArr[y][x].color = Color.grey;
                        break;
                    case Tile.TileType.WALL:

                        gridArr[y][x].color = Color.black;
                        break;
                    case Tile.TileType.WALLCORRIDOR:

                        gridArr[y][x].color = Color.green;
                        break;
                    case Tile.TileType.ROOF:
                        break;
                    case Tile.TileType.FLOORCORRIDOR:

                        gridArr[y][x].color = Color.yellow;
                        break;
                    case Tile.TileType.AVOID:

                        gridArr[y][x].color = Color.red;
                        break;
                    default:
                        break;
                }
            }
        }
    }




    public static  void SaveMap(Tile[][] grid,string saveFileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        // Create a new array to store the data
        SerializableTile[][] serializableMap = new SerializableTile[grid.Length][];
        for (int i = 0; i < grid.Length; i++)
        {
            serializableMap[i] = new SerializableTile[grid[i].Length];
            for (int j = 0; j < grid[i].Length; j++)
            {
                serializableMap[i][j] = new SerializableTile(grid[i][j].position, grid[i][j].tileWeight, grid[i][j].cost, grid[i][j].idx, grid[i][j].visited, (int)grid[i][j].tileType);
            }
        }

        formatter.Serialize(stream, serializableMap);

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.Refresh();
        }


        if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Resources_Algorithms");
            AssetDatabase.Refresh();
        }


        if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/Saved_Gen_Data"))
        {
            AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "Saved_Gen_Data");
            AssetDatabase.Refresh();
        }

        File.WriteAllBytes(Application.dataPath + "/Resources/Resources_Algorithms/Saved_Gen_Data/" + saveFileName, stream.ToArray());
    }


}


[Serializable]
public class SerializableTile 
{
    public SerialiableVector2Int position = new SerialiableVector2Int();
    public float tileWeight;
    public float cost = 0;
    public int idx = 0;
    public bool visited = false;

    public int tileType;

    public SerializableTile(Vector2Int position, float tileWeight, float cost , int idx, bool visited, int tileType) 
    {
        this.position = new SerialiableVector2Int(position.x, position.y);
        this.tileWeight = tileWeight;
        this.cost = cost;
        this.idx = idx;
        this.visited = visited;
        this.tileType = tileType;
    }
}


[Serializable]
public struct SerialiableVector2Int
{
    public int x;
    public int y;

    public SerialiableVector2Int(int rX, int rY)
    {
        x = rX;
        y = rY;
    }
}








