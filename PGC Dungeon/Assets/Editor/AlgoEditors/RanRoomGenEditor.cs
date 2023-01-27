using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RanRoomGenMA))]
public class RanRoomGenEditor : Editor
{
    


    bool specialAlgo;
    bool CAselected;


    bool showCA = false;
    string status = "Use Cellular Automata to tidy up";
    int NeighboursNeeded;

    bool showPath = false;

    bool started;
    int MinSize;

    bool showRooms = false;
    bool showRules = false;

    int selGridPathType = 0;
    GUIContent[] selStringsPathType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualr path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have manu corridors" }, new GUIContent() { text = "Prim's algo + random", tooltip = "Create a singualr path that traverses the whole dungeon, with some random diviation" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };


    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algortihm Marhcing cubes create a mesh object whihc can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tielset provided" } };


    bool useWeights = false;
    bool DjAvoidWalls = false;

    int corridorThickness = 2;

    int selGridConnectionType = 0;
    GUIContent[] selStringsConnectionType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualr path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have manu corridors" }, new GUIContent() { text = "Prim's algo + random", tooltip = "Create a singualr path that traverses the whole dungeon, with some random diviation" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };
    int primRan = 1;

    int selGridPathGenType = 0;
    GUIContent[] selStringPathGenType = { new GUIContent() { text = "A* pathfinding", tooltip = "" }, new GUIContent() { text = "Dijistra", tooltip = "" }, new GUIContent() { text = "BFS", tooltip = "" }, new GUIContent() { text = "DFS", tooltip = "" }, new GUIContent() { text = "Beizier Curve", tooltip = "" } };
    int margin = 20;




    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RanRoomGenMA mainScript = (RanRoomGenMA)target;


        mainScript.BPSg = EditorGUILayout.Toggle(mainScript.BPSg == true ? "BPS selected" : "Random room allocation selected", mainScript.BPSg);
        if (!mainScript.BPSg)
        {
            mainScript.MaxHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Max heihgt of the room", tooltip = "" }, mainScript.MaxHeight, 15, 125);

            if (mainScript.MaxHeight <= mainScript.MinHeight)
            {
                mainScript.MaxHeight = mainScript.MinHeight + 1;
            }
        }
       
        mainScript.MinHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min Height of the room", tooltip = "" }, mainScript.MinHeight, 10, 75);




        if (!mainScript.BPSg)
        {
            mainScript.MaxWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Max width of the room", tooltip = "" }, mainScript.MaxWidth, 15, 125);
            if (mainScript.MaxWidth <= mainScript.MinWidth)
            {
                mainScript.MaxWidth = mainScript.MinWidth + 1;
            }

        }

        mainScript.MinWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min Width of the room", tooltip = "" }, mainScript.MinWidth, 10, 75);

        mainScript.NumOfRoom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum number of rooms to spawn", tooltip = "" }, mainScript.NumOfRoom, 3, 30);


        specialAlgo = EditorGUILayout.Toggle("special algo toggle", specialAlgo);
        if (specialAlgo) 
        {
            CAselected = EditorGUILayout.Toggle(CAselected == true ? "CA selected for gen" : "drunk walk selected", CAselected);
            if(CAselected) 
            {

                NeighboursNeeded = (int)EditorGUILayout.Slider(NeighboursNeeded, 3, 5);
            }
        }


        if (mainScript.BPSg) 
        {
            if (GUILayout.Button("Use BPS algo"))// gen something
            {
                mainScript.RoomList.Clear();

                mainScript.rooms.Clear();

                mainScript.PcgManager.gridArray2D = AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
                

                mainScript.BPSRoomGen(mainScript.PcgManager.gridArray2D);
                mainScript.SetUpWeights(mainScript.PcgManager.gridArray2D);
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);
                if (specialAlgo)
                    mainScript.RanRoom(mainScript.BPSg, CAselected);

                mainScript.Started = true;
            }
        }
        else 
        {
            if (GUILayout.Button("Use random Room gen"))// gen something
            {
                mainScript.roomList.Clear();
                mainScript.rooms.Clear();

                    mainScript.PcgManager.gridArray2D = AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
                

                mainScript.RandomRoomGen(mainScript.PcgManager.gridArray2D);
                mainScript.SetUpWeights(mainScript.PcgManager.gridArray2D);
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);

                if (specialAlgo)
                    mainScript.RanRoom(mainScript.BPSg, CAselected);


                mainScript.Started = true;
            }
        }


        if (mainScript.Started)
        {

            GeneralUtil.SpacesUILayout(4);


            #region showCA region

            showCA = EditorGUILayout.BeginFoldoutHeaderGroup(showCA, "Use Cellular Automata(CA) to tidy up");

            if (showCA)
            {

                NeighboursNeeded = (int)EditorGUILayout.Slider(new GUIContent() { text = "Neighbours Needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" }, NeighboursNeeded, 3, 5);

                if (GUILayout.Button(new GUIContent() { text = "Clean Up using CA", tooltip = "Run half of the CA algortihm to only take out tiles, to help slim down the result" }))
                {
                    AlgosUtils.CleanUp2dCA(mainScript.PcgManager.gridArray2D, NeighboursNeeded);

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }
                if (GUILayout.Button(new GUIContent() { text = "Use CA algorithm", tooltip = "Run the full CA algorithm on the current iteration of the grid" }))
                {
                    AlgosUtils.RunCaIteration2D(mainScript.PcgManager.gridArray2D, NeighboursNeeded);
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
                    GUILayout.Label("Choose how to order the connection of the rooms");

                    GeneralUtil.SpacesUILayout(2);

                    GUILayout.BeginVertical("Box");
                    selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, selStringsConnectionType, 1);
                    GUILayout.EndVertical();

                    GeneralUtil.SpacesUILayout(3);

                    switch (selGridConnectionType)
                    {

                        case 2:   // prims ran

                            primRan = (int)EditorGUILayout.Slider(new GUIContent() { text = "prim random rooms", tooltip = "To add /// TO FINISH" }, primRan, 1, mainScript.rooms.Count / 2);
                            GeneralUtil.SpacesUILayout(2);
                            break;

                        case 3:   // beizier 

                            break;

                        default:
                            break;
                    }


                    GeneralUtil.SpacesUILayout(2);

                    GUILayout.Label("Choose the ThickNess of the corridor");

                    corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the corridor", tooltip = "" }, corridorThickness, 2, 5);

                    GeneralUtil.SpacesUILayout(2);



                    GUILayout.Label("Choose the algorithm to that creates the corridor");

                    GeneralUtil.SpacesUILayout(2);

                    GUILayout.BeginVertical("Box");
                    selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, selStringPathGenType, 1);
                    GUILayout.EndVertical();

                    GeneralUtil.SpacesUILayout(2);




                    switch (selGridPathGenType)
                    {
                        case 0:   // A* pathfindind
                            mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.PathType);
                            useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                            break;

                        case 1:   // djistra 
                            DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                            break;
                        case 4:   // beizier 

                            margin = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "beizeir curve thing to change" }, margin, 10, 50);
                            break;

                        default:
                            break;
                    }




                    if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                    {

                        mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
                        var centerPoints = new List<Vector2>();
                        var roomDict = new Dictionary<Vector2, List<BasicTile>>();
                        foreach (var room in mainScript.rooms)
                        {
                            roomDict.Add(AlgosUtils.FindMiddlePoint(room), room);
                            centerPoints.Add(AlgosUtils.FindMiddlePoint(room));
                        }

                        //there is 4 ways 

                        switch (selGridConnectionType)
                        {
                            case 0:
                                mainScript.edges = AlgosUtils.PrimAlgoNoDelu(centerPoints);
                                break;
                            case 1:
                                mainScript.edges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;
                                break;
                            case 2://prim ran
                                mainScript.edges = AlgosUtils.PrimAlgoNoDelu(centerPoints);

                                int len = mainScript.edges.Count - 1;

                                for (int i = 0; i < primRan; i++)
                                {
                                    var pointA = mainScript.edges[Random.Range(0, len)].edge[0];
                                    var pointBEdgeCheck = mainScript.edges[Random.Range(0, len)];

                                    var pointB = Vector3.zero;

                                    if (pointA == pointBEdgeCheck.edge[0])
                                        pointB = pointBEdgeCheck.edge[1];
                                    else if (pointA == pointBEdgeCheck.edge[1])
                                        pointB = pointBEdgeCheck.edge[0];
                                    else
                                        pointB = pointBEdgeCheck.edge[1];


                                    Edge newEdge = new Edge(pointA, pointB);

                                    bool toAdd = true;

                                    foreach (var primEdge in mainScript.edges)
                                    {
                                        if (AlgosUtils.LineIsEqual(primEdge, newEdge))
                                        {
                                            toAdd = false;
                                            break;
                                        }
                                    }


                                    if (toAdd)
                                    {
                                        mainScript.edges.Add(newEdge);
                                    }
                                }

                                break;
                            case 3://ran
                                break;
                        }





                        switch (selGridPathGenType)
                        {
                            case 0:   //A* pathfingin

                                foreach (var edge in mainScript.edges)
                                {

                                    //use where so we get soemthing its not the wall but not necessary
                                    var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                    var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;


                                    var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);


                                    foreach (var tile in path.Item1)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }
                                }

                                break;
                            case 1:  //dijistra
                                foreach (var edge in mainScript.edges)
                                {
                                    var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                    var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                    var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);

                                    foreach (var tile in path)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }
                                }

                                break;
                            case 2://   bfs
                                break;
                            case 3://  dfs
                                break;
                            case 4://  beizier curve
                                foreach (var edge in mainScript.edges)
                                {

                                    var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                    var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                    var startPos = new Vector2Int(tileA.x, tileA.y);
                                    var endPos = new Vector2Int(tileB.x, tileB.y);

                                    var prevCoord = new Vector2Int(0, 0);

                                    var positions = AlgosUtils.ExtrapolatePos(startPos, endPos, margin);

                                    var mid1Pos = new Vector2Int((int)Mathf.Round(positions.Item1.x), (int)Mathf.Round(positions.Item1.y));
                                    var mid2Pos = new Vector2Int((int)Mathf.Round(positions.Item2.x), (int)Mathf.Round(positions.Item2.y));

                                    for (float t = 0; t < 1; t += 0.05f)
                                    {
                                        float currT = t;
                                        float prevT = t - 0.05f;

                                        var currCord = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, currT);

                                        if (prevT < 0)
                                        {
                                            prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));
                                            continue;
                                        }
                                        else if (currCord.x < 0 || currCord.y < 0 || currCord.x >= mainScript.PcgManager.gridArray2D[0].Length || currCord.y >= mainScript.PcgManager.gridArray2D.Length)
                                        { continue; }

                                        var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z)), true);

                                        prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));

                                        foreach (var tile in path.Item1)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }
                                    }
                                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);

                                }
                                break;

                            default:
                                break;
                        }





                        AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);


                        for (int i = 0; i < corridorThickness - 1; i++)
                        {
                            for (int y = 0; y < mainScript.PcgManager.gridArray2D.Length; y++)
                            {
                                for (int x = 0; x < mainScript.PcgManager.gridArray2D[0].Length; x++)
                                {
                                    if (mainScript.PcgManager.gridArray2D[y][x].tileType == BasicTile.TileType.WALLCORRIDOR)
                                    {
                                        mainScript.PcgManager.gridArray2D[y][x].tileType = BasicTile.TileType.FLOORCORRIDOR;
                                    }
                                    if (mainScript.PcgManager.gridArray2D[y][x].tileType == BasicTile.TileType.FLOORCORRIDOR)
                                    {
                                    }
                                }
                            }

                            AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);
                        }


                        AlgosUtils.SetUpTileTypesFloorWall(mainScript.PcgManager.gridArray2D);

                        mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                    }
                }
            }
            else
            {

                GUILayout.Label("to Access the corridor making function you need to\nclick on one of the room buttons first");
                //if (GUILayout.Button("Generate walls"))
                //{
                //    AlgosUtils.SetUpTileTypesFloorWall(mainScript.PcgManager.gridArray2D);
                //    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                //}
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



            //if (GUILayout.Button(new GUIContent() { text = "Test buton" }))
            //{
            //}



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


                        for (int y = 0; y < mainScript.PcgManager.gridArray2D.Length; y++)
                        {
                            for (int x = 0; x < mainScript.PcgManager.gridArray2D[0].Length; x++)
                            {
                                if (mainScript.PcgManager.gridArray2D[y][x].tileType == BasicTile.TileType.WALLCORRIDOR)
                                {
                                    mainScript.PcgManager.gridArray2D[y][x].tileType = BasicTile.TileType.FLOORCORRIDOR;
                                }
                            }
                        }

                        AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);


                        mainScript.PcgManager.FormObject(AlgosUtils.MarchingCubesAlgo(AlgosUtils.ExtrapolateMarchingCubes(mainScript.PcgManager.gridArray2D, mainScript.PcgManager.RoomHeight), false));
                        break;

                    case 1:
                        mainScript.PcgManager.DrawTileMap();
                        break;
                }
            }

            if (selGridGenType == 1)
            {
                mainScript.PcgManager.ChunkHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "This is for the chunk height", tooltip = "" }, mainScript.PcgManager.ChunkHeight, 10, 40);
                mainScript.PcgManager.ChunkWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "This is for the chunk width", tooltip = "" }, mainScript.PcgManager.ChunkWidth, 10, 40);
            }



            #endregion
        }

    }



}
