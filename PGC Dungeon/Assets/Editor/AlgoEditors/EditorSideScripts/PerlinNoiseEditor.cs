using Codice.Client.BaseCommands.Changelist;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(PerlinNoiseMA))]
public class PerlinNoiseEditor : Editor
{

    bool showCA = false;

    bool showPath = false;
    bool showRooms = false;

    bool showRules = false;


    int selGridPathType = 0;
    GUIContent[] selStringsPathType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualr path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have manu corridors" }, new GUIContent() { text = "Prim's algo + random", tooltip = "Create a singualr path that traverses the whole dungeon, with some random diviation" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };


    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algortihm Marhcing cubes create a mesh object whihc can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tielset provided" } };







    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerlinNoiseMA mainScript = (PerlinNoiseMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have choosen Perlin noise Generation  ---   to write");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion



        GeneralUtil.SpacesUILayout(4);

        #region Main Algo


        mainScript.OffsetX = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset X", tooltip = "" }, mainScript.OffsetX, 0, 10000);
        mainScript.OffsetY = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset Y", tooltip = "" }, mainScript.OffsetY, 0, 10000);


        mainScript.Scale = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Scale", tooltip = "" }, mainScript.Scale, 3f, 35f);
        mainScript.Octaves = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Octaves", tooltip = "" }, mainScript.Octaves, 1, 8);

        mainScript.Persistance = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Persitance", tooltip = "" }, mainScript.Persistance, 0.1f, 0.9f);
        mainScript.Lacunarity = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Lacunarity", tooltip = "" }, mainScript.Lacunarity, 0.5f, 10f);

        mainScript.Threshold = EditorGUILayout.Slider(new GUIContent() { text = "Max Threshold", tooltip = "" }, mainScript.Threshold, 0.1f, 0.9f);


        if (GUILayout.Button("Generate Noise"))
        {
            mainScript.PcgManager.gridArray2D = AlgosUtils.PerlinNoise2D(mainScript.PcgManager.gridArray2D, mainScript.Scale, mainScript.Octaves, mainScript.Persistance, mainScript.Lacunarity, mainScript.OffsetX, mainScript.OffsetY, mainScript.Threshold);

            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D,true);

            mainScript.Started = true;
        }


        #endregion



        if (mainScript.Started)
        {

            GeneralUtil.SpacesUILayout(4);


            #region showCA region

            showCA = EditorGUILayout.BeginFoldoutHeaderGroup(showCA, "Use Cellular Automata(CA) to tidy up");

            if (showCA)
            {

                GUILayout.Label(new GUIContent() { text = "Neighbours needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" });
                mainScript.NeighboursNeeded = (int)EditorGUILayout.Slider(mainScript.NeighboursNeeded, 3, 5);

                if (GUILayout.Button(new GUIContent() { text = "Clean Up using CA", tooltip = "Run half of the CA algortihm to only take out tiles, to help slim down the result" }))
                {
                    AlgosUtils.CleanUp2dCA(mainScript.PcgManager.gridArray2D, mainScript.NeighboursNeeded);

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }
                if (GUILayout.Button(new GUIContent() { text = "Use CA algorithm", tooltip = "Run the full CA algorithm on the current iteration of the grid" }))
                {
                    AlgosUtils.RunCaIteration2D(mainScript.PcgManager.gridArray2D, mainScript.NeighboursNeeded);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }

            }

            if (!Selection.activeTransform)
            {
                showCA = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion


            GeneralUtil.SpacesUILayout(4);


            #region Room Region

            showRooms = EditorGUILayout.BeginFoldoutHeaderGroup(showRooms, "Rooms section");

            if (showRooms)
            {

                if (GUILayout.Button("Get all rooms"))
                {
                    mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextSelfCol(mainScript.PcgManager.gridArray2D);
                }


                mainScript.MinSize = (int)EditorGUILayout.Slider(mainScript.MinSize, 30, 200);

                GUILayout.Label($"Delete all the rooms beneath {mainScript.MinSize} tiles big");

                if (GUILayout.Button("Delete small rooms"))
                {

                    mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);

                    foreach (var room in mainScript.rooms)
                    {
                        if (room.Count < mainScript.MinSize)
                        {
                            foreach (var tile in room)
                            {
                                tile.tileWeight = 0;
                                tile.tileType = Tile.TileType.VOID;
                            }
                        }
                    }

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                }

            }

            if (!Selection.activeTransform)
            {
                showRooms = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            #endregion


            GeneralUtil.SpacesUILayout(4);


            #region corridor making region

            if (mainScript.rooms.Count > 1)
            {
                showPath = EditorGUILayout.BeginFoldoutHeaderGroup(showPath, "Pathfinding Settings");

                if (showPath)
                {
                    GUILayout.Label("Decide What Pathfinding Algorithm to use");

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    GUILayout.BeginVertical("Box");
                    selGridPathType = GUILayout.SelectionGrid(selGridPathType, selStringsPathType, 1);
                    GUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.PathType);


                    if (GUILayout.Button("Connect all the rooms"))// gen something
                    {

                        mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
                        var centerPoints = new List<Vector2>();
                        var roomDict = new Dictionary<Vector2, List<Tile>>();
                        foreach (var room in mainScript.rooms)
                        {
                            roomDict.Add(AlgosUtils.FindMiddlePoint(room), room);
                            centerPoints.Add(AlgosUtils.FindMiddlePoint(room));
                        }

                        //there is 4 ways 

                        switch (selGridPathType)
                        {
                            case 0:
                                mainScript.edges = AlgosUtils.PrimAlgoNoDelu(centerPoints);
                                break;
                            case 1:
                                mainScript.edges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;
                                break;
                            case 2://prim ran
                                break;
                            case 3://ran
                                break;
                        }



                        foreach (var edge in mainScript.edges)
                        {

                            //use where so we get soemthing its not the wall but not necessary
                            var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                            var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;


                            var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.PathType);


                            foreach (var tile in path.Item1)
                            {
                                if (tile.tileType != Tile.TileType.FLOORROOM)
                                    tile.tileType = Tile.TileType.FLOORCORRIDOR;

                                tile.tileWeight = 0.75f;
                            }
                        }




                        AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);
                        AlgosUtils.SetUpTileTypesFloorWall(mainScript.PcgManager.gridArray2D);

                        mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Generate walls"))
                {
                    AlgosUtils.SetUpTileTypesFloorWall(mainScript.PcgManager.gridArray2D);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                }
            }


            if (!Selection.activeTransform)
            {
                showPath = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            #endregion


            GeneralUtil.SpacesUILayout(4);


            #region Dungeon Generation region


            GUILayout.BeginVertical("Box");
            selGridGenType = GUILayout.SelectionGrid(selGridGenType, selStringsGenType, 1);
            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent() { text = "Generate YOUR Dungeon!" }))
            {
                switch (selGridGenType)
                {
                    case 0:
                        mainScript.PcgManager.FormObject(AlgosUtils.MarchingCubesAlgo(AlgosUtils.ExtrapolateMarchingCubes(mainScript.PcgManager.gridArray2D, mainScript.PcgManager.RoomHeight), false));
                        break;

                    case 1:
                        mainScript.PcgManager.DrawTileMapDirectionalWalls();
                        break;
                }
            }


            #endregion
        }
    }
}
