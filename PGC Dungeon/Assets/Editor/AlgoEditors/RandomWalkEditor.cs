using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(RandomWalkMA))]
public class RandomWalkEditor : Editor
{
    bool showCA = false;

    bool showPath = false;
    bool showRooms = false;

    bool showRules = false;

    bool useWeights = false;


    int selGridConnectionType = 0;
    GUIContent[] selStringsConnectionType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualr path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have manu corridors" }, new GUIContent() { text = "Prim's algo + random", tooltip = "Create a singualr path that traverses the whole dungeon, with some random diviation" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };
    int primRan = 1;

    int selGridPathGenType = 0;
    GUIContent[] selStringPathGenType = { new GUIContent() { text = "A* pathfinding", tooltip = "" }, new GUIContent() { text = "Dijistra", tooltip = "" }, new GUIContent() { text = "BFS", tooltip = "" }, new GUIContent() { text = "DFS", tooltip = "" }, new GUIContent() { text = "Beizier Curve", tooltip = "" } };
    int margin = 20;


    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algortihm Marhcing cubes create a mesh object whihc can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tielset provided" } };


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RandomWalkMA mainScript = (RandomWalkMA)target;

        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have choosen RandomWalk algorithm as your initial algorithm\n\nExplenation: a \"Head\" moves in a random direction at each step\n\nStep 1: Decide how many iterations the algorithm will have to generate the base map and its sub parameters." +
                "\n\nStep 2: To round up the rough edges you can decide to use Cellular Automata to help smooth things out." +
                "\n\nStep 3: It is possible small rooms that are not your what you are looking for have been geenrated, delete them using by setting up the minimum amount of tiles a room should have." +
                "\n\nStep 4: Depending on the amount of rooms you are able to create corridors by choosing the wanted pathFinding algorithm and the algortihm which decideds which room is connected to which." +
                "\n\nStep 5: Generate the algorithm using the tileSet provided or create the blank gameobject whihc can then be exported and manipulated");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion


        GeneralUtil.Spaces(4);


        #region Main algo region


        mainScript.Iterations = (int)EditorGUILayout.Slider(new GUIContent() { text = "Iterations", tooltip = "" }, mainScript.Iterations, (mainScript.PcgManager.gridArray2D.Length * mainScript.PcgManager.gridArray2D[0].Length) * 0.3f, (mainScript.PcgManager.gridArray2D.Length * mainScript.PcgManager.gridArray2D[0].Length) * 0.9f);

        mainScript.StartFromMiddle = EditorGUILayout.Toggle(new GUIContent() { text = "Should The algo start from the middle", tooltip = mainScript.StartFromMiddle == true ? "Start the head from the middle" : "Start the head from a random place on the Canvas" }, mainScript.StartFromMiddle); ;
        mainScript.AlreadyPassed = EditorGUILayout.Toggle(new GUIContent() { text = "Overlap cells count", tooltip = mainScript.AlreadyPassed == true ? "When the head of the walker goes over an already populated cells the iteration still counts" : "When the head of the walker goes over an already populated cells the iteration does not count" }, mainScript.AlreadyPassed);


        if (GUILayout.Button("Generate RandomWalk Randomisation"))// gen something
        {
            AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
            mainScript.PcgManager.gridArray2D = AlgosUtils.RandomWalk2DCol(mainScript.Iterations, !mainScript.AlreadyPassed, mainScript.PcgManager.gridArray2D[0].Length, mainScript.PcgManager.gridArray2D.Length, randomStart: !mainScript.StartFromMiddle);
            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
            mainScript.Started = true;
        }



        #endregion


        if (mainScript.Started)
        {

            GeneralUtil.Spaces(4);


            #region showCA region

            showCA = EditorGUILayout.BeginFoldoutHeaderGroup(showCA, "Use Cellular Automata(CA) to tidy up");

            if (showCA)
            {

                mainScript.NeighboursNeeded = (int)EditorGUILayout.Slider(new GUIContent() { text = "Neighbours Needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" }, mainScript.NeighboursNeeded, 3, 5);

                if (GUILayout.Button(new GUIContent() { text = "Clean Up using CA", tooltip = "Run half of the CA algortihm to only take out tiles, to help slim down the result" }))
                {
                    AlgosUtils.CleanUp2dCA(mainScript.PcgManager.gridArray2D, mainScript.NeighboursNeeded);

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }
                if (GUILayout.Button(new GUIContent() { text = "Use CA algorithm", tooltip = "Run the full CA algorithm on the current iteration of the grid" }))
                {
                    AlgosUtils.RunCaIteration2D(mainScript.PcgManager.gridArray2D, mainScript.NeighboursNeeded);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }

            }

            if (!Selection.activeTransform)
            {
                showCA = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion


            GeneralUtil.Spaces(4);



            #region Room Region

            showRooms = EditorGUILayout.BeginFoldoutHeaderGroup(showRooms, "Rooms section");

            if (showRooms)
            {



                if (GUILayout.Button("Get all rooms"))
                {
                    mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextSelfCol(mainScript.PcgManager.gridArray2D);
                }



                mainScript.MinSize = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum size of room to delete", tooltip = "" }, mainScript.MinSize, 30, 200);

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
                                tile.tileType = BasicTile.TileType.VOID;
                            }
                        }
                    }

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                }

            }

            if (!Selection.activeTransform)
            {
                showRooms = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            #endregion



            GeneralUtil.Spaces(4);


            #region corridor making region

            if (mainScript.rooms.Count > 1)
            {
                showPath = EditorGUILayout.BeginFoldoutHeaderGroup(showPath, "Pathfinding Settings");

                if (showPath)
                {
                    GUILayout.Label("Choose how to order the connection of the rooms");

                    GeneralUtil.Spaces(2);

                    GUILayout.BeginVertical("Box");
                    selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, selStringsConnectionType, 1);
                    GUILayout.EndVertical();

                    GeneralUtil.Spaces(3);

                    switch (selGridConnectionType)
                    {

                        case 2:   // prims ran

                            primRan = (int)EditorGUILayout.Slider(new GUIContent() { text = "prim random rooms", tooltip = "To add /// TO FINISH" }, primRan, 1, mainScript.rooms.Count/2);
                            GeneralUtil.Spaces(2);
                            break;

                        case 3:   // beizier 

                            break;

                        default:
                            break;
                    }




                    

                    GUILayout.Label("Choose the algorithm to that creates the corridor");

                    GeneralUtil.Spaces(2);

                    GUILayout.BeginVertical("Box");
                    selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, selStringPathGenType, 1);
                    GUILayout.EndVertical();

                    GeneralUtil.Spaces(2);




                    switch (selGridPathGenType)
                    {
                        case 0:   // A* pathfindind
                            mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.PathType);
                            useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                            break;

                        case 4:   // beizier 

                            margin = (int)EditorGUILayout.Slider(new GUIContent() { text = "Margin", tooltip = "beizeir curve thing to change" }, margin, 10, 50);
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

                                    var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y));

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

                                    var mid1Pos = new Vector2Int((int)MathF.Round(positions.Item1.x), (int)MathF.Round(positions.Item1.y));
                                    var mid2Pos = new Vector2Int((int)MathF.Round(positions.Item2.x), (int)MathF.Round(positions.Item2.y));

                                    for (float t = 0; t < 1; t += 0.05f)
                                    {
                                        float currT = t;
                                        float prevT = t - 0.05f;

                                        var currCord = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, currT);

                                        if (prevT < 0)
                                        {
                                            prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));
                                            continue;
                                        }
                                        else if (currCord.x < 0 || currCord.y < 0 || currCord.x >= mainScript.PcgManager.gridArray2D[0].Length || currCord.y >= mainScript.PcgManager.gridArray2D.Length)
                                        { continue; }

                                        var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z)), true);

                                        prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));

                                        foreach (var tile in path.Item1)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }
                                    }
                                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);

                                }
                                break;

                            default:
                                break;
                        }








                        AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);


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
                        AlgosUtils.SetUpTileTypesFloorWall(mainScript.PcgManager.gridArray2D);

                        mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
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
            //    var startpos = new Vector2Int(10, 10);
            //    var endpos = new Vector2Int(76, 34);

            //    var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, startpos, endpos);

            //    Debug.Log(path.Count);

            //    foreach (var tile in path)
            //    {
            //        if (tile.tileType != BasicTile.TileType.FLOORROOM)
            //            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

            //        tile.tileWeight = 0.75f;
            //    }

            //    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
            //}



            GeneralUtil.Spaces(4);



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
