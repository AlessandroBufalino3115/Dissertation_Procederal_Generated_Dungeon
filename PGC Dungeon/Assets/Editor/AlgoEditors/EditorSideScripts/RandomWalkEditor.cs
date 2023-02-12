using System;
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
    bool DjAvoidWalls = false;

    int corridorThickness = 2;

    int selGridConnectionType = 0;
    GUIContent[] selStringsConnectionType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualar path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have many corridors" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };
    int randomAddCorr = 0;

    int selGridPathGenType = 0;
    GUIContent[] selStringPathGenType = { new GUIContent() { text = "A* pathfinding", tooltip = "" }, new GUIContent() { text = "Dijistra", tooltip = "" }, new GUIContent() { text = "BFS (WIP)", tooltip = "" }, new GUIContent() { text = "DFS (WIP)", tooltip = "" }, new GUIContent() { text = "Beizier Curve", tooltip = "Create curved corridors" } };
    int margin = 20;

    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

    bool algoForBezier = false;






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


        GeneralUtil.SpacesUILayout(4);


        #region Main algo region


        mainScript.Iterations = (int)EditorGUILayout.Slider(new GUIContent() { text = "Iterations", tooltip = "This is how many times the head of the algorithm is going to move" }, mainScript.Iterations, (mainScript.PcgManager.gridArray2D.Length * mainScript.PcgManager.gridArray2D[0].Length) * 0.3f, (mainScript.PcgManager.gridArray2D.Length * mainScript.PcgManager.gridArray2D[0].Length) * 0.9f);

        mainScript.StartFromMiddle = EditorGUILayout.Toggle(new GUIContent() { text = "Should The algo start from the middle", tooltip = "Should the head of the algorithm start from the middle of the canvas or a random position?" }, mainScript.StartFromMiddle); ;
        mainScript.AlreadyPassed = EditorGUILayout.Toggle(new GUIContent() { text = "Overlap cells count", tooltip = mainScript.AlreadyPassed == true ? "When the head of the walker goes over an already populated cells the iteration still counts" : "When the head of the walker goes over an already populated cells the iteration does not count" }, mainScript.AlreadyPassed);


        if (GUILayout.Button("Generate RandomWalk Randomisation"))// gen something
        {
            AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
            mainScript.PcgManager.gridArray2D = AlgosUtils.RandomWalk2DCol(mainScript.Iterations, !mainScript.AlreadyPassed, mainScript.PcgManager.gridArray2D[0].Length, mainScript.PcgManager.gridArray2D.Length, randomStart: !mainScript.StartFromMiddle);
            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
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

                mainScript.NeighboursNeeded = (int)EditorGUILayout.Slider(new GUIContent() { text = "Neighbours Needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" }, mainScript.NeighboursNeeded, 3, 5);

                if (GUILayout.Button(new GUIContent() { text = "Clean Up using CA", tooltip = "Run half of the CA algortihm to only take out tiles, to help slim down the result" }))
                {
                    mainScript.PcgManager.CreateBackUpGrid();


                    AlgosUtils.CleanUp2dCA(mainScript.PcgManager.gridArray2D, mainScript.NeighboursNeeded);

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }
                if (GUILayout.Button(new GUIContent() { text = "Use CA algorithm", tooltip = "Run the full CA algorithm on the current iteration of the grid" }))
                {
                    mainScript.PcgManager.CreateBackUpGrid();


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
                mainScript.MinSize = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum size of room to delete", tooltip = "Any room with a lower number of tiles will be deleted" }, mainScript.MinSize, 0, 200);

                if (GUILayout.Button("Generate rooms"))
                {
                    mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);


                    mainScript.PcgManager.CreateBackUpGrid();

                    if (mainScript.MinSize > 0)
                    {

                        for (int i = mainScript.rooms.Count; i-- > 0;)
                        {
                            if (mainScript.rooms[i].Count < mainScript.MinSize)
                            {
                                foreach (var tile in mainScript.rooms[i])
                                {
                                    tile.tileWeight = 0;
                                    tile.tileType = BasicTile.TileType.VOID;
                                }

                                mainScript.rooms.RemoveAt(i);
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


            if (mainScript.rooms.Count == 1)
            {
                GUILayout.Label("Only one room detected, Corridor making is not possible");
            }
            else if (mainScript.rooms.Count == 2)
            {
                GUILayout.Label("Only two rooms detected, triangulation not possible");

                GUILayout.Label("Choose the algorithm to create the corridor");

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

                        margin = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "beizeir curve thing to change" }, margin, 10, 40);
                        algoForBezier = EditorGUILayout.Toggle(new GUIContent() { text = algoForBezier == true ? "A* Pathfinding is selected" : "Dijistra is selected", tooltip = "" }, algoForBezier);

                        if (algoForBezier)
                        {
                            GeneralUtil.SpacesUILayout(1);

                            mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.PathType);
                            useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                        }

                        break;

                    default:
                        break;
                }


                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                {
                    Vector2Int tileA = mainScript.rooms[0][Random.Range(0, mainScript.rooms[0].Count - 1)].position;
                    Vector2Int tileB = mainScript.rooms[1][Random.Range(0, mainScript.rooms[1].Count - 1)].position;

                  

                    //its checking the edges its not meant tot thweiuyieqw

                    switch (selGridPathGenType)
                    {
                        case 0:   //A* pathfingin

                           
                                var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                foreach (var tile in path.Item1)
                                {
                                    if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                        tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                    tile.tileWeight = 0.75f;
                                }
                            

                            break;
                        case 1:  //dijistra
                          

                                var pathD = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);

                                foreach (var tile in pathD)
                                {
                                    if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                        tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                    tile.tileWeight = 0.75f;
                                }
                            

                            break;
                        case 2://   bfs
                            break;
                        case 3://  dfs
                            break;
                        case 4://  beizier curve
                           
                                var startPos = new Vector2Int(tileA.x, tileA.y);
                                var endPos = new Vector2Int(tileB.x, tileB.y);

                                var prevCoord = new Vector2Int(0, 0);

                                var positions = AlgosUtils.ExtrapolatePos(startPos, endPos, margin);

                                var mid1Pos = new Vector2Int((int)MathF.Round(positions.Item1.x), (int)MathF.Round(positions.Item1.y));
                                var mid2Pos = new Vector2Int((int)MathF.Round(positions.Item2.x), (int)MathF.Round(positions.Item2.y));



                                var firstBezierPoint = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, 0);

                                if (algoForBezier)
                                {
                                    var pathB = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)MathF.Round(firstBezierPoint.x), (int)MathF.Round(firstBezierPoint.z)), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                    foreach (var tile in pathB.Item1)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }

                                }
                                else
                                {
                                    var pathB = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)MathF.Round(firstBezierPoint.x), (int)MathF.Round(firstBezierPoint.z)), true);

                                    foreach (var tile in pathB)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }
                                }



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

                                    if (algoForBezier)
                                    {
                                        var pathB = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z)), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);


                                        prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));

                                        foreach (var tile in pathB.Item1)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }

                                    }
                                    else
                                    {
                                        var pathB = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z)), true);

                                        prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));

                                        foreach (var tile in pathB)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }
                                    }
                                }



                                var lastBezierCurvePoint = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, 1);

                                if (algoForBezier)
                                {
                                    var pathB = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)MathF.Round(lastBezierCurvePoint.x), (int)MathF.Round(lastBezierCurvePoint.z)), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                    foreach (var tile in pathB.Item1)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }

                                }
                                else
                                {
                                    var pathB = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)MathF.Round(lastBezierCurvePoint.x), (int)MathF.Round(lastBezierCurvePoint.z)), true);

                                    foreach (var tile in pathB)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }
                                }


                            break;

                        default:
                            break;
                    }



                    AlgosUtils.SetUpTileCorridorTypesUI(mainScript.PcgManager.gridArray2D, corridorThickness);

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                }

            }
            else if (mainScript.rooms.Count > 2)
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
                        case 0:   // prims ran
                           
                            if (mainScript.rooms.Count >= 4)
                            {
                                randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                                GeneralUtil.SpacesUILayout(2);
                            }
                            break;

                        case 2:  

                            if (mainScript.rooms.Count >= 4) 
                            {
                                randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                                GeneralUtil.SpacesUILayout(2);
                            }
                            break;

                        default:
                            break;
                    }


                    GeneralUtil.SpacesUILayout(2);

                    GUILayout.Label("Choose the Thickness of the corridor");

                    corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the corridor", tooltip = "How wide should the corridor be" }, corridorThickness, 2, 5);

                    GeneralUtil.SpacesUILayout(3);


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

                            margin = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, margin, 10, 40);
                            algoForBezier = EditorGUILayout.Toggle(new GUIContent() { text = algoForBezier == true ? "A* Pathfinding is selected" : "Dijistra is selected", tooltip = "" }, algoForBezier);

                            if (algoForBezier) 
                            {
                                GeneralUtil.SpacesUILayout(1);

                                mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "Pathfinding will prioritize the creation of straight corridors" }, mainScript.PathType);
                                useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                            }

                            break;

                        default:
                            break;
                    }


                    if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                    {

                        mainScript.PcgManager.CreateBackUpGrid();

                        mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
                        var centerPoints = new List<Vector2>();
                        var roomDict = new Dictionary<Vector2, List<BasicTile>>();
                        foreach (var room in mainScript.rooms)
                        {
                            roomDict.Add(AlgosUtils.FindMiddlePoint(room), room);
                            centerPoints.Add(AlgosUtils.FindMiddlePoint(room));
                        }


                        switch (selGridConnectionType)
                        {
                            case 0:
                                mainScript.edges = AlgosUtils.PrimAlgoNoDelu(centerPoints);
                                if (randomAddCorr > 0) 
                                {
                                    int len = mainScript.edges.Count - 1;

                                    for (int i = 0; i < randomAddCorr; i++)
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


                                }
                                break;

                            case 1:
                                mainScript.edges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;
                                break;
                            
                            case 2://ran


                                for (int i = 0; i < centerPoints.Count; i++)
                                {
                                    if (i == centerPoints.Count-1) { continue; }
                                    mainScript.edges.Add(new Edge(new Vector3(centerPoints[i].x, centerPoints[i].y, 0), new Vector3(centerPoints[i+1].x, centerPoints[i+1].y, 0)));
                                }

                                if (randomAddCorr > 0) 
                                {
                                    int len = mainScript.edges.Count - 1;

                                    for (int i = 0; i < randomAddCorr; i++)
                                    {
                                       
                                        int ranStarter  = Random.Range(0, len);
                                        int ranEnder  = Random.Range(0, len);


                                        if (ranStarter == ranEnder) { continue; }
                                        else if (Mathf.Abs(ranStarter - ranEnder) == 1) { continue; }
                                        else
                                        {
                                            mainScript.edges.Add(new Edge(new Vector3(centerPoints[ranStarter].x, centerPoints[ranStarter].y, 0), new Vector3(centerPoints[ranEnder].x, centerPoints[ranEnder].y, 0)));
                                        }
                                    }
                                }

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

                                    var mid1Pos = new Vector2Int((int)MathF.Round(positions.Item1.x), (int)MathF.Round(positions.Item1.y));
                                    var mid2Pos = new Vector2Int((int)MathF.Round(positions.Item2.x), (int)MathF.Round(positions.Item2.y));



                                    var firstBezierPoint = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, 0);

                                    if (algoForBezier)
                                    {
            
                                        var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)MathF.Round(firstBezierPoint.x), (int)MathF.Round(firstBezierPoint.z)), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                        foreach (var tile in path.Item1)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }

                                    }
                                    else
                                    {
                                        var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)MathF.Round(firstBezierPoint.x), (int)MathF.Round(firstBezierPoint.z)), true);

                                        foreach (var tile in path)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }
                                    }



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


                                        if (algoForBezier) 
                                        {
                                            var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z)), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);


                                            prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));

                                            foreach (var tile in path.Item1)
                                            {
                                                if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                    tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                                tile.tileWeight = 0.75f;
                                            }

                                        }
                                        else 
                                        {
                                            var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z)), true);

                                            prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));

                                            foreach (var tile in path)
                                            {
                                                if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                    tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                                tile.tileWeight = 0.75f;
                                            }
                                        }
                                    }




                                    var lastBezierCurvePoint = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, 1);

                                    if (algoForBezier)
                                    {
                                        var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)MathF.Round(lastBezierCurvePoint.x), (int)MathF.Round(lastBezierCurvePoint.z)), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);



                                        foreach (var tile in path.Item1)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }

                                    }
                                    else
                                    {
                                        var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)MathF.Round(lastBezierCurvePoint.x), (int)MathF.Round(lastBezierCurvePoint.z)), true);

                                        foreach (var tile in path)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }
                                    }

                                }
                                break;

                            default:
                                break;
                        }

                        AlgosUtils.SetUpTileCorridorTypesUI(mainScript.PcgManager.gridArray2D, corridorThickness);

                        mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
                    }
                }
            }
            else
            {
                GUILayout.Label("To access the corridor making function you need to\nGenerate the rooms first");
            }


            if (!Selection.activeTransform)
            {
                showPath = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            GeneralUtil.SpacesUILayout(4);

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
