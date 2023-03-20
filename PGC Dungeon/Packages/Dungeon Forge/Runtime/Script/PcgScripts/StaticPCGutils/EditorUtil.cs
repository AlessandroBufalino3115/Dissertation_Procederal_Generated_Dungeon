using DungeonForge.AlgoScript;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;



namespace DungeonForge.Utils
{

    public static class DFEditorUtil
    {

        public static void SpacesUILayout(int spaceNum)
        {
            for (int i = 0; i < spaceNum; i++)
            {
                EditorGUILayout.Space();
            }
        }

        public static void CellularAutomataEditorSection(PCGManager pcgManager, int neighbours, out int setNeighbours)
        {
            DFTile[,] gridArr = pcgManager.gridArr;

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

        public static void ExtraRoomEditorSelection(PCGManager pcgManager,List<List<DFTile>> rooms, int radius, int height, int width, out int setHeight, out int setWidth, out int setRadius)
        {

            setRadius = (int)EditorGUILayout.Slider(new GUIContent() { text = "Radius of the room" }, radius, 10, 40);

            if (GUILayout.Button(new GUIContent() { text = "Spawn circular room", tooltip = "Creates a circular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }))
            {
                bool success = false;

                for (int i = 0; i < 5; i++)
                {
                    var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(0) - radius - 3), Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(1) - radius - 3));

                    var room = DFAlgoBank.CreateCircleRoom(pcgManager.gridArr, randomPoint, radius + 2, checkForFitting: true);

                    if (room != null)
                    {
                        pcgManager.CreateBackUpGrid();
                        room = DFAlgoBank.CreateCircleRoom(pcgManager.gridArr, randomPoint, radius, actuallyDraw: true);

                        pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);

                        rooms.Add(room);

                        success = true;

                        break;
                    }
                }

                if (!success)
                    Debug.Log($"<color=red>I tried to spawn the Room as requested 5 times but couldnt find any free space either try again or lower the size</color>");
            }

            SpacesUILayout(2);

            setHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "" }, height, 10, 40);
            setWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Widht", tooltip = "" }, width, 10, 40);

            if (GUILayout.Button(new GUIContent() { text = "Generate random rectangular room", tooltip = "Creates a rectangular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }))
            {
                bool success = false;
                for (int i = 0; i < 5; i++)
                {
                    var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(0) - radius - 3), Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(1) - radius - 3));

                    var squareRoom = DFAlgoBank.CreateSquareRoom(width, height, randomPoint, pcgManager.gridArr, checkForFitting: true);

                    if (squareRoom != null)
                    {
                        pcgManager.CreateBackUpGrid();
                        squareRoom = DFAlgoBank.CreateSquareRoom(width, height, randomPoint, pcgManager.gridArr,actuallyDraw: true);

                        pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);

                        rooms.Add(squareRoom);

                        success = true;
                        break;
                    }
                }

                if (!success)
                    Debug.Log($"<color=red>I tried to spawn the Room as requested 5 times but couldnt find any free space either try again or lower the size</color>");

            }
        }

        public static bool CalculateRoomsEditorSection(PCGManager pcgManager, int minSize, out List<List<DFTile>> rooms, out int setMinSize)
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
                                tile.tileType = DFTile.TileType.VOID;
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

        public static void SaveGridDataToGenerate(PCGManager pcgManager, string inSaveMapFileName, out string saveMapFileName)
        {
            saveMapFileName = EditorGUILayout.TextField("Save file name: ", inSaveMapFileName);
            if (GUILayout.Button("save"))
            {
                SaveMap(pcgManager.gridArr, inSaveMapFileName);
            }
            SpacesUILayout(2);

            GUILayout.Label("Once saved you can access this data later on.\nTo Generate your dungeon switch to the Generate component (in the main algo selection) and give this file name");
        }

        public static void SaveMap(DFTile[,] grid, string saveFileName)
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

    }
}