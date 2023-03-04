using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonForge
{
    public static class DFGeneralUtil
    {
        public static int[,] childPosArry4Side = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
        public static int[,] childPosArry8Side = { { 0, -1 }, { 1, -1 }, { -1, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 }, { -1, 1 } };

        /// <summary>
        /// from 0 
        /// </summary>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public static Vector2Int RanVector2Int(int maxX, int maxY)
        {
            int ranX = Random.Range(0, maxX);
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


            return new Vector3Int(ranX, ranY, ranZ);
        }

        public static Vector2 RanVector2Float(float maxX, float maxY)
        {
            float ranX = Random.Range(0f, maxX);
            float ranY = Random.Range(0f, maxY);

            return new Vector2(ranX, ranY);
        }

        public static int PerfTimer(bool start, int startTimer = 0)
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
        /// <param name="gridArr"></param>
        /// <returns></returns>
        public static Texture2D SetUpTextSelfCol(Tile[,] gridArr)
        {
            Texture2D texture = new Texture2D(gridArr.GetLength(0), gridArr.GetLength(1));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, gridArr[x, y].color);
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// either black or white, if = 0 white if = 1 black
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="black"></param>
        /// <returns></returns>
        public static Texture2D SetUpTextBiColAnchor(Tile[,] gridArr, bool black = false)
        {
            Texture2D texture = new Texture2D(gridArr.GetLength(0), gridArr.GetLength(1));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color color = new Color();

                    if (black)
                    {
                        color = ((gridArr[x, y].tileWeight) == 0 ? Color.white : Color.black);
                    }
                    else
                    {
                        color = ((gridArr[x, y].tileWeight) == 0 ? Color.white : Color.grey);
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
        /// <param name="gridArr"></param>
        /// <returns></returns>
        public static Texture2D SetUpTextBiColShade(Tile[,] gridArr, float minWeight, float maxWeight, bool inverse = false)
        {
            Texture2D texture = new Texture2D(gridArr.GetLength(0), gridArr.GetLength(1));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float num = Mathf.InverseLerp(minWeight, maxWeight, gridArr[x, y].tileWeight);

                    if (inverse)
                        gridArr[x, y].color = new Color(1 - num, 1 - num, 1 - num, 1f);
                    else
                        gridArr[x, y].color = new Color(num, num, num, 1f);


                    texture.SetPixel(x, y, gridArr[x, y].color);
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

        public static void SetUpColorBasedOnType(Tile[,] gridArr)
        {
            for (int y = 0; y < gridArr.Length; y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    switch (gridArr[x, y].tileType)
                    {
                        case Tile.TileType.VOID:
                            gridArr[x, y].color = Color.white;
                            break;
                        case Tile.TileType.FLOORROOM:

                            // gridArr[x,y].color = new Color(0.5f, 0.5f, 0.5f,1f);
                            gridArr[x, y].color = Color.blue;
                            break;
                        case Tile.TileType.WALL:

                            gridArr[x, y].color = Color.black;
                            break;
                        case Tile.TileType.WALLCORRIDOR:

                            gridArr[x, y].color = Color.green;
                            break;
                        case Tile.TileType.ROOF:
                            break;
                        case Tile.TileType.FLOORCORRIDOR:

                            gridArr[x, y].color = Color.yellow;
                            break;
                        case Tile.TileType.AVOID:

                            gridArr[x, y].color = Color.red;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void SaveMap(Tile[,] grid, string saveFileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            // Create a new array to store the data
            SerializableTile[,] serializableMap = new SerializableTile[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    serializableMap[j, i] = new SerializableTile(grid[j, i].position, grid[j, i].tileWeight, grid[j, i].cost, (int)grid[j, i].tileType);
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


        #region Editor

        public static void CellularAutomataEditorSection(PCGManager pcgManager, int neighbours, out int setNeighbours)
        {
            Tile[,] gridArr = pcgManager.gridArr;

            setNeighbours = (int)EditorGUILayout.Slider(new GUIContent() { text = "Neighbours Needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" }, neighbours, 3, 5);

            if (GUILayout.Button(new GUIContent() { text = "Clean Up using CA", tooltip = "Run half of the CA algortihm to only take out tiles, to help slim down the result" }))
            {
                pcgManager.CreateBackUpGrid();

                DFAlgoBank.CleanUp2dCA(gridArr, neighbours);

                pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(gridArr);
            }
            if (GUILayout.Button(new GUIContent() { text = "Use CA algorithm", tooltip = "Run the full CA algorithm on the current iteration of the grid" }))
            {
                pcgManager.CreateBackUpGrid();

                DFAlgoBank.RunCaIteration2D(gridArr, neighbours);
                pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(gridArr);
            }
        }

        public static bool CalculateRoomsEditorSection(PCGManager pcgManager, int minSize, out List<List<Tile>> rooms, out int setMinSize)
        {
            setMinSize = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum size of room to delete", tooltip = "Any room with a lower number of tiles will be deleted" }, minSize, 0, 200);

            if (GUILayout.Button("Generate rooms"))
            {
                rooms = DFAlgoBank.GetAllRooms(pcgManager.gridArr);

                pcgManager.CreateBackUpGrid();

                if (setMinSize > 0)
                {
                    for (int i = rooms.Count; i-- > 0;)
                    {
                        if (rooms[i].Count <= setMinSize)
                        {
                            foreach (var tile in rooms[i])
                            {
                                tile.tileWeight = 0;
                                tile.tileType = Tile.TileType.VOID;
                            }

                            rooms.RemoveAt(i);
                        }
                    }
                }
                //mainScript.allowedForward = true;
                pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);


                return true;
            }

            rooms = null;
            return false;

        }

        public static void GenerateMeshEditorSection(PCGManager pcgManager, string inSaveMapFileName, out string saveMapFileName)
        {
            saveMapFileName = EditorGUILayout.TextField("Save file name: ", inSaveMapFileName);
            if (GUILayout.Button("save"))
            {
                SaveMap(pcgManager.gridArr, inSaveMapFileName);
            }
        }

        public enum UI_STATE
        {
            MAIN_ALGO,
            CA,
            ROOM_GEN,
            EXTRA_ROOM_GEN,
            PATHING,
            GENERATION
        }

        public enum PathFindingType
        {
            A_STAR,
            DJISTRA,
            BFS,
            DFS
        }

        public static GUIContent[] selStringsConnectionType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualar path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have many corridors" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridor connections" } };

        public static GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

        public static GUIContent[] selStringPathGenType = { new GUIContent() { text = "A* pathfinding", tooltip = "" }, new GUIContent() { text = "Dijistra", tooltip = "" }, new GUIContent() { text = "Beizier Curve", tooltip = "Create curved corridors" } };

        #endregion
    }


    [Serializable]
    public class SerializableTile
    {
        public SerialiableVector2Int position = new SerialiableVector2Int();
        public float tileWeight;
        public float cost = 0;

        public int tileType;

        public SerializableTile(Vector2Int position, float tileWeight, float cost, int tileType)
        {
            this.position = new SerialiableVector2Int(position.x, position.y);
            this.tileWeight = tileWeight;
            this.cost = cost;
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

}