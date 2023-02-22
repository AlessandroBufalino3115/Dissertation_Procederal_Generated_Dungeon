using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(VoronoiMA))]
public class VoronoiEditor : Editor
{
    bool showRules = false;

    int vorPoints = 5;
    int minNumOfRooms = 1;
    bool typeOfVoronoi = false;
    bool voronoiCalculation = false;


    int selGridGenType = 0;
    int selGridPathGenType = 0;
    int selGridConnectionType = 0;

    int corridorThickness = 3;
    int randomAddCorr = 0;
    int deadEndAmount = 0;
    int deadEndCorridorThickness = 3;

    int width = 10;
    int height = 10;
    int radius = 10;


    bool useWeights = false;
    bool DjAvoidWalls = false;
    int margin = 5;


    string saveMapFileName = "";
    bool blockGeneration = false;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        VoronoiMA mainScript = (VoronoiMA)target;



        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have choosen Voronoi");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        GeneralUtil.SpacesUILayout(4);


        #endregion







        switch (mainScript.currUiState)
        {
            case GeneralUtil.UISTATE.MAIN_ALGO:
                {
                    typeOfVoronoi = EditorGUILayout.Toggle(new GUIContent() { text = typeOfVoronoi == true ? "Room to room is selected" : "random deletion room is selected", tooltip = "" }, typeOfVoronoi);

                    GeneralUtil.SpacesUILayout(1);

                    vorPoints = (int)EditorGUILayout.Slider(new GUIContent() { text = "number of voronoi divisions", tooltip = "" }, vorPoints, 5, 40);
                    voronoiCalculation = EditorGUILayout.Toggle(new GUIContent() { text = voronoiCalculation == true ? "euclidian" : "manhattan", tooltip = "" }, voronoiCalculation);



                    if (!typeOfVoronoi)
                        minNumOfRooms = (int)EditorGUILayout.Slider(new GUIContent() { text = "number of rooms", tooltip = "" }, minNumOfRooms, 2, vorPoints - 1);


                    if (GUILayout.Button(new GUIContent() { text = "Generate Voronoi Fracture", tooltip = "" }))// gen something
                    {
                        mainScript.pcgManager.Restart();

                        mainScript.pcgManager.gridArray2D = AlgosUtils.Voronoi2D(mainScript.pcgManager.gridArray2D, vorPoints, voronoiCalculation);

                        mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArray2D, true);


                        mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.pcgManager.gridArray2D, true);


                        if (!typeOfVoronoi)
                        {
                            while (mainScript.rooms.Count > minNumOfRooms)
                            {
                                int roomCount = mainScript.rooms.Count - 1;
                                int ranIndex = Random.Range(0, roomCount);

                                for (int i = 0; i < mainScript.rooms[ranIndex].Count; i++)
                                {
                                    mainScript.rooms[ranIndex][i].tileWeight = 0;
                                    mainScript.rooms[ranIndex][i].tileType = Tile.TileType.VOID;
                                }

                                mainScript.rooms.RemoveAt(ranIndex);
                            }


                            mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.pcgManager.gridArray2D, true);

                        }

                        mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);
                    }


                    if (mainScript.rooms.Count > 1)
                    {
                        mainScript.allowedForward = true;
                    }


                    break;
                }





            case GeneralUtil.UISTATE.CA:
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.currUiState = (GeneralUtil.UISTATE)mainScript.currStateIndex;

                    break;
                }
            case GeneralUtil.UISTATE.ROOM_GEN:
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.currUiState = (GeneralUtil.UISTATE)mainScript.currStateIndex;

                    break;
                }


            case GeneralUtil.UISTATE.EXTRA_ROOM_GEN:
                {
                    if (!typeOfVoronoi)
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = false;

                        radius = (int)EditorGUILayout.Slider(new GUIContent() { text = "Radius of the arena", tooltip = "Creates a circular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }, radius, 10, 40);

                        if (GUILayout.Button(new GUIContent() { text = "Spawn one Arena" }))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, mainScript.pcgManager.gridArray2D[0].Length - radius - 3), Random.Range(0 + radius + 3, mainScript.pcgManager.gridArray2D.Length - radius - 3));

                                var room = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, randomPoint, radius + 2);

                                if (room != null)
                                {
                                    mainScript.pcgManager.CreateBackUpGrid();
                                    room = AlgosUtils.DrawCircle(mainScript.pcgManager.gridArray2D, randomPoint, radius, draw: true);

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);

                                    mainScript.rooms.Add(room);

                                    break;
                                }
                            }
                        }


                        GeneralUtil.SpacesUILayout(2);

                        height = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "" }, height, 10, 40);
                        width = (int)EditorGUILayout.Slider(new GUIContent() { text = "Widht", tooltip = "" }, width, 10, 40);

                        if (GUILayout.Button(new GUIContent() { text = "gen Room" }))
                        {

                            for (int i = 0; i < 5; i++)
                            {
                                var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, mainScript.pcgManager.gridArray2D[0].Length - radius - 3), Random.Range(0 + radius + 3, mainScript.pcgManager.gridArray2D.Length - radius - 3));

                                var squareRoom = AlgosUtils.SpawnRoom(width, height, randomPoint, mainScript.pcgManager.gridArray2D, true);

                                if (squareRoom != null)
                                {
                                    mainScript.pcgManager.CreateBackUpGrid();
                                    squareRoom = AlgosUtils.SpawnRoom(width, height, randomPoint, mainScript.pcgManager.gridArray2D);

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);

                                    mainScript.rooms.Add(squareRoom);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {

                        mainScript.pcgManager.ClearUndos();
                        mainScript.allowedForward = false;
                        mainScript.currStateIndex++;
                        mainScript.currUiState = (GeneralUtil.UISTATE)mainScript.currStateIndex;

                    }
                    break;
                }

            case GeneralUtil.UISTATE.PATHING:
                {
                    #region corridor making region

                    if (typeOfVoronoi)
                    {

                        EditorGUI.BeginDisabledGroup(mainScript.pcgManager.prevGridArray2D.Count == 1);

                        GUILayout.Label("Choose how to order the connection of the rooms");

                        GeneralUtil.SpacesUILayout(2);

                        GUILayout.BeginVertical("Box");
                        selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, GeneralUtil.selStringsConnectionType, 1);
                        GUILayout.EndVertical();

                        if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                        {
                            mainScript.allowedForward = true;

                            mainScript.pcgManager.CreateBackUpGrid();
                            mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.pcgManager.gridArray2D, true);
                            var centerPoints = new List<Vector2>();
                            var roomDict = new Dictionary<Vector2, List<Tile>>();
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

                                            Vector3 pointB;

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
                                        if (i == centerPoints.Count - 1) { continue; }
                                        mainScript.edges.Add(new Edge(new Vector3(centerPoints[i].x, centerPoints[i].y, 0), new Vector3(centerPoints[i + 1].x, centerPoints[i + 1].y, 0)));
                                    }

                                    if (randomAddCorr > 0)
                                    {
                                        int len = mainScript.edges.Count - 1;

                                        for (int i = 0; i < randomAddCorr; i++)
                                        {
                                            int ranStarter = Random.Range(0, len);
                                            int ranEnder = Random.Range(0, len);

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


                            foreach (var edge in mainScript.edges)
                            {
                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;
                                AlgosUtils.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), margin, mainScript.pcgManager.gridArray2D, !mainScript.pathType);
                            }


                            AlgosUtils.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArray2D, corridorThickness);

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);
                        }

                        EditorGUI.EndDisabledGroup();

                        if (mainScript.pcgManager.prevGridArray2D.Count == 1)
                        {
                            GUILayout.Label(new GUIContent() { text = "You have generated your corridors, either click the undo button to try another algorithm or continue" });

                            mainScript.allowedBack = false;
                            mainScript.allowedForward = true;
                        }
                        else
                        {
                            mainScript.allowedBack = true;
                            mainScript.allowedForward = false;
                        }

                    }
                    else
                    {
                        if (mainScript.pcgManager.prevGridArray2D.Count == 0)
                        {
                            mainScript.allowedBack = true;
                            if (mainScript.rooms.Count == 1)
                            {
                                mainScript.allowedForward = true;
                                GUILayout.Label("Only one room detected, Corridor making is not possible");
                            }
                            else if (mainScript.rooms.Count == 2)
                            {
                                GUILayout.Label("Only two rooms detected, triangulation not possible");

                                GUILayout.Label("Choose the algorithm to create the corridor");

                                GeneralUtil.SpacesUILayout(2);

                                GUILayout.BeginVertical("Box");
                                selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, GeneralUtil.selStringPathGenType, 1);
                                GUILayout.EndVertical();

                                GeneralUtil.SpacesUILayout(2);

                                switch (selGridPathGenType)
                                {
                                    case 0:   // A* pathfindind
                                        mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                                        useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                        break;

                                    case 1:   // djistra 
                                        DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                                        break;
                                    case 4:   // beizier 

                                        margin = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "beizeir curve thing to change" }, margin, 10, 40);
                                        GeneralUtil.SpacesUILayout(1);

                                        mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);

                                        break;

                                    default:
                                        break;
                                }

                                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                                {


                                    mainScript.pcgManager.CreateBackUpGrid();

                                    Vector2Int tileA = mainScript.rooms[0][Random.Range(0, mainScript.rooms[0].Count - 1)].position;
                                    Vector2Int tileB = mainScript.rooms[1][Random.Range(0, mainScript.rooms[1].Count - 1)].position;


                                    mainScript.allowedForward = true;
                                    // usedPathing = true;

                                    switch (selGridPathGenType)
                                    {
                                        case 0:   //A* pathfingin

                                            var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.pcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.pathType, useWeights: useWeights, arrWeights: mainScript.pcgManager.tileCosts);

                                            AlgosUtils.SetUpCorridorWithPath(path.Item1);

                                            break;
                                        case 1:  //dijistra

                                            var pathD = AlgosUtils.DijstraPathfinding(mainScript.pcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);

                                            AlgosUtils.SetUpCorridorWithPath(pathD);


                                            break;
                                        case 2://   bfs
                                            break;
                                        case 3://  dfs
                                            break;
                                        case 4://  beizier curve

                                            AlgosUtils.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), margin, mainScript.pcgManager.gridArray2D, !mainScript.pathType);

                                            break;

                                        default:
                                            break;
                                    }

                                    AlgosUtils.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArray2D, corridorThickness);

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);
                                }
                            }
                            else if (mainScript.rooms.Count > 2)
                            {
                                GUILayout.Label("Choose how to order the connection of the rooms");

                                GeneralUtil.SpacesUILayout(2);

                                GUILayout.BeginVertical("Box");
                                selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, GeneralUtil.selStringsConnectionType, 1);
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
                                selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, GeneralUtil.selStringPathGenType, 1);
                                GUILayout.EndVertical();

                                GeneralUtil.SpacesUILayout(2);


                                switch (selGridPathGenType)
                                {
                                    case 0:   // A* pathfindind
                                        mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                                        useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                        break;

                                    case 1:   // djistra 
                                        DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                                        break;
                                    case 4:   // beizier 

                                        margin = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, margin, 10, 40);
                                        GeneralUtil.SpacesUILayout(1);
                                        mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "Pathfinding will prioritize the creation of straight corridors" }, mainScript.pathType);

                                        break;

                                    default:
                                        break;
                                }

                                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                                {
                                    mainScript.allowedForward = true;

                                    mainScript.pcgManager.CreateBackUpGrid();

                                    mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.pcgManager.gridArray2D, true);
                                    var centerPoints = new List<Vector2>();
                                    var roomDict = new Dictionary<Vector2, List<Tile>>();
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

                                                    Vector3 pointB;

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
                                                if (i == centerPoints.Count - 1) { continue; }
                                                mainScript.edges.Add(new Edge(new Vector3(centerPoints[i].x, centerPoints[i].y, 0), new Vector3(centerPoints[i + 1].x, centerPoints[i + 1].y, 0)));
                                            }

                                            if (randomAddCorr > 0)
                                            {
                                                int len = mainScript.edges.Count - 1;

                                                for (int i = 0; i < randomAddCorr; i++)
                                                {
                                                    int ranStarter = Random.Range(0, len);
                                                    int ranEnder = Random.Range(0, len);

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
                                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                                var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.pcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.pathType, useWeights: useWeights, arrWeights: mainScript.pcgManager.tileCosts);

                                                AlgosUtils.SetUpCorridorWithPath(path.Item1);
                                            }

                                            break;
                                        case 1:  //dijistra
                                            foreach (var edge in mainScript.edges)
                                            {
                                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                                var path = AlgosUtils.DijstraPathfinding(mainScript.pcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);

                                                AlgosUtils.SetUpCorridorWithPath(path);
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
                                                AlgosUtils.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), margin, mainScript.pcgManager.gridArray2D, !mainScript.pathType);
                                            }

                                            break;

                                        default:
                                            break;
                                    }

                                    AlgosUtils.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArray2D, corridorThickness);

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);
                                }
                            }
                            else
                            {
                                GUILayout.Label("To access the corridor making function you need to\nGenerate the rooms first");
                            }
                        }
                        else
                        {
                            mainScript.allowedBack = false;
                            GeneralUtil.GenerateDeadEndCorridorEditorSection(mainScript.pcgManager, deadEndAmount, deadEndCorridorThickness, mainScript.rooms, out deadEndCorridorThickness, out deadEndAmount, margin, out margin);
                        }
                    }

                    #endregion

                    break;
                }

            case GeneralUtil.UISTATE.GENERATION:
                {
                    mainScript.allowedBack = false;

                    GeneralUtil.GenerateMeshEditorSection(mainScript.pcgManager, selGridGenType, blockGeneration, saveMapFileName, out selGridGenType, out blockGeneration, out saveMapFileName);
                    break;
                }


            default:
                break;
        }


        if (mainScript.currUiState != GeneralUtil.UISTATE.GENERATION)
        {
            GeneralUtil.SpacesUILayout(4);

            EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

            if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
            {
                mainScript.pcgManager.ClearUndos();
                mainScript.allowedBack = false;
                mainScript.currStateIndex--;
                mainScript.currUiState = (GeneralUtil.UISTATE)mainScript.currStateIndex;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

            if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
            {
                mainScript.pcgManager.ClearUndos();
                mainScript.allowedForward = false;
                mainScript.currStateIndex++;
                mainScript.currUiState = (GeneralUtil.UISTATE)mainScript.currStateIndex;
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
