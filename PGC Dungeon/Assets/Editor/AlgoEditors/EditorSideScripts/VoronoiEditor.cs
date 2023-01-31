using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(VoronoiMA))]
public class VoronoiEditor : Editor
{
    int vorPoints = 5;
    int minSize = 0;
    List<List<BasicTile>> rooms = new List<List<BasicTile>>();

    bool pathType = false;

    bool started = false;

    bool showPath = false;
    bool showRooms = false;

    bool showRules = false;

    bool useWeights = false;
    bool DjAvoidWalls = false;

    int corridorThickness = 2;

    int selGridConnectionType = 0;
    GUIContent[] selStringsConnectionType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualar path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have many corridors" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };
    int randomAddCorr = 1;

    int selGridPathGenType = 0;
    GUIContent[] selStringPathGenType = { new GUIContent() { text = "A* pathfinding", tooltip = "" }, new GUIContent() { text = "Dijistra", tooltip = "" }, new GUIContent() { text = "BFS (WIP)", tooltip = "" }, new GUIContent() { text = "DFS (WIP)", tooltip = "" }, new GUIContent() { text = "Beizier Curve", tooltip = "Create curved corridors" } };
    int margin = 20;

    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

    bool algoForBezier = false;
    List<Edge> edges = new List<Edge>();



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







        vorPoints = (int)EditorGUILayout.Slider(new GUIContent() { text = "number of rooms", tooltip = "" }, vorPoints, 5, 40);

        if (GUILayout.Button(new GUIContent() { text = "Generate Voronoi Fracture", tooltip = "" }))// gen something
        {
            mainScript.PcgManager.Restart();

            mainScript.PcgManager.gridArray2D = AlgosUtils.Voronoi2D(mainScript.PcgManager.gridArray2D, vorPoints);

            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);
            started = true;
        }


        if (started)
        {

            GeneralUtil.SpacesUILayout(4);

            #region Room Region

            showRooms = EditorGUILayout.BeginFoldoutHeaderGroup(showRooms, "Rooms section");

            if (showRooms)
            {
                minSize = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum size of room to delete", tooltip = "Any room with a lower number of tiles will be deleted" }, minSize, 0, 200);

                if (GUILayout.Button("Generate rooms"))
                {
                    rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);



                    if (minSize > 0)
                    {
                        for (int i = rooms.Count; i-- > 0;)
                        {
                            if (rooms[i].Count < minSize)
                            {
                                foreach (var tile in rooms[i])
                                {
                                    tile.tileWeight = 0;
                                    tile.tileType = BasicTile.TileType.VOID;
                                }

                                rooms.RemoveAt(i);
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


            if (rooms.Count == 1)
            {
                GUILayout.Label("Only one room detected, Corridor making is not possible");
            }
            else if (rooms.Count == 2)
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
                        pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, pathType);
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

                            pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, pathType);
                            useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                        }

                        break;

                    default:
                        break;
                }


                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                {
                    Vector2Int tileA = rooms[0][Random.Range(0, rooms[0].Count - 1)].position;
                    Vector2Int tileB = rooms[1][Random.Range(0, rooms[1].Count - 1)].position;



                    //its checking the edges its not meant tot thweiuyieqw

                    switch (selGridPathGenType)
                    {
                        case 0:   //A* pathfingin


                            var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

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

                            var mid1Pos = new Vector2Int((int)Mathf.Round(positions.Item1.x), (int)Mathf.Round(positions.Item1.y));
                            var mid2Pos = new Vector2Int((int)Mathf.Round(positions.Item2.x), (int)Mathf.Round(positions.Item2.y));



                            var firstBezierPoint = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, 0);

                            if (algoForBezier)
                            {
                                var pathB = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)Mathf.Round(firstBezierPoint.x), (int)Mathf.Round(firstBezierPoint.z)), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                foreach (var tile in pathB.Item1)
                                {
                                    if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                        tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                    tile.tileWeight = 0.75f;
                                }

                            }
                            else
                            {
                                var pathB = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)Mathf.Round(firstBezierPoint.x), (int)Mathf.Round(firstBezierPoint.z)), true);

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
                                    prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));
                                    continue;
                                }
                                else if (currCord.x < 0 || currCord.y < 0 || currCord.x >= mainScript.PcgManager.gridArray2D[0].Length || currCord.y >= mainScript.PcgManager.gridArray2D.Length)
                                { continue; }

                                if (algoForBezier)
                                {
                                    var pathB = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z)), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);


                                    prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));

                                    foreach (var tile in pathB.Item1)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }

                                }
                                else
                                {
                                    var pathB = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z)), true);

                                    prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));

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
                                var pathB = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)Mathf.Round(lastBezierCurvePoint.x), (int)Mathf.Round(lastBezierCurvePoint.z)), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                foreach (var tile in pathB.Item1)
                                {
                                    if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                        tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                    tile.tileWeight = 0.75f;
                                }

                            }
                            else
                            {
                                var pathB = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)Mathf.Round(lastBezierCurvePoint.x), (int)Mathf.Round(lastBezierCurvePoint.z)), true);

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
            else if (rooms.Count > 2)
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

                            if (rooms.Count >= 4)
                            {
                                randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, rooms.Count / 2);
                                GeneralUtil.SpacesUILayout(2);
                            }
                            break;

                        case 2:

                            if (rooms.Count >= 4)
                            {
                                randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, rooms.Count / 2);
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
                            pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, pathType);
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

                                pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "Pathfinding will prioritize the creation of straight corridors" }, pathType);
                                useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                            }

                            break;

                        default:
                            break;
                    }


                    if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                    {

                        rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
                        var centerPoints = new List<Vector2>();
                        var roomDict = new Dictionary<Vector2, List<BasicTile>>();
                        foreach (var room in rooms)
                        {
                            roomDict.Add(AlgosUtils.FindMiddlePoint(room), room);
                            centerPoints.Add(AlgosUtils.FindMiddlePoint(room));
                        }


                        switch (selGridConnectionType)
                        {
                            case 0:
                                edges = AlgosUtils.PrimAlgoNoDelu(centerPoints);
                                if (randomAddCorr > 0)
                                {
                                    int len = edges.Count - 1;

                                    for (int i = 0; i < randomAddCorr; i++)
                                    {
                                        var pointA = edges[Random.Range(0, len)].edge[0];
                                        var pointBEdgeCheck = edges[Random.Range(0, len)];

                                        var pointB = Vector3.zero;

                                        if (pointA == pointBEdgeCheck.edge[0])
                                            pointB = pointBEdgeCheck.edge[1];
                                        else if (pointA == pointBEdgeCheck.edge[1])
                                            pointB = pointBEdgeCheck.edge[0];
                                        else
                                            pointB = pointBEdgeCheck.edge[1];


                                        Edge newEdge = new Edge(pointA, pointB);

                                        bool toAdd = true;

                                        foreach (var primEdge in edges)
                                        {
                                            if (AlgosUtils.LineIsEqual(primEdge, newEdge))
                                            {
                                                toAdd = false;
                                                break;
                                            }
                                        }


                                        if (toAdd)
                                        {
                                            edges.Add(newEdge);
                                        }
                                    }


                                }
                                break;

                            case 1:
                                edges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;
                                break;

                            case 2://ran


                                for (int i = 0; i < centerPoints.Count; i++)
                                {
                                    if (i == centerPoints.Count - 1) { continue; }
                                        edges.Add(new Edge(new Vector3(centerPoints[i].x, centerPoints[i].y, 0), new Vector3(centerPoints[i + 1].x, centerPoints[i + 1].y, 0)));
                                }

                                if (randomAddCorr > 0)
                                {
                                    int len = edges.Count - 1;

                                    for (int i = 0; i < randomAddCorr; i++)
                                    {

                                        int ranStarter = Random.Range(0, len);
                                        int ranEnder = Random.Range(0, len);


                                        if (ranStarter == ranEnder) { continue; }
                                        else if (Mathf.Abs(ranStarter - ranEnder) == 1) { continue; }
                                        else
                                        {
                                            edges.Add(new Edge(new Vector3(centerPoints[ranStarter].x, centerPoints[ranStarter].y, 0), new Vector3(centerPoints[ranEnder].x, centerPoints[ranEnder].y, 0)));
                                        }
                                    }
                                }

                                break;
                        }


                        switch (selGridPathGenType)
                        {
                            case 0:   //A* pathfingin

                                foreach (var edge in edges)
                                {
                                    var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                    var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;


                                    var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);


                                    foreach (var tile in path.Item1)
                                    {
                                        if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                        tile.tileWeight = 0.75f;
                                    }
                                }

                                break;
                            case 1:  //dijistra
                                foreach (var edge in edges)
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
                                foreach (var edge in edges)
                                {

                                    var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                    var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                    var startPos = new Vector2Int(tileA.x, tileA.y);
                                    var endPos = new Vector2Int(tileB.x, tileB.y);

                                    var prevCoord = new Vector2Int(0, 0);
                                    var positions = AlgosUtils.ExtrapolatePos(startPos, endPos, margin);

                                    var mid1Pos = new Vector2Int((int)Mathf.Round(positions.Item1.x), (int)Mathf.Round(positions.Item1.y));
                                    var mid2Pos = new Vector2Int((int)Mathf.Round(positions.Item2.x), (int)Mathf.Round(positions.Item2.y));



                                    var firstBezierPoint = AlgosUtils.CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, 0);

                                    if (algoForBezier)
                                    {
                                        var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)Mathf.Round(firstBezierPoint.x), (int)Mathf.Round(firstBezierPoint.z)), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                        foreach (var tile in path.Item1)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }
                                    }
                                    else
                                    {
                                        var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, startPos, new Vector2Int((int)Mathf.Round(firstBezierPoint.x), (int)Mathf.Round(firstBezierPoint.z)), true);

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
                                            prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));
                                            continue;
                                        }
                                        else if (currCord.x < 0 || currCord.y < 0 || currCord.x >= mainScript.PcgManager.gridArray2D[0].Length || currCord.y >= mainScript.PcgManager.gridArray2D.Length)
                                        { continue; }


                                        if (algoForBezier)
                                        {
                                            var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z)), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);


                                            prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));

                                            foreach (var tile in path.Item1)
                                            {
                                                if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                    tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                                tile.tileWeight = 0.75f;
                                            }

                                        }
                                        else
                                        {
                                            var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, prevCoord, new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z)), true);

                                            prevCoord = new Vector2Int((int)Mathf.Round(currCord.x), (int)Mathf.Round(currCord.z));

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
                                        var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)Mathf.Round(lastBezierCurvePoint.x), (int)Mathf.Round(lastBezierCurvePoint.z)), !pathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                        foreach (var tile in path.Item1)
                                        {
                                            if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                                tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                            tile.tileWeight = 0.75f;
                                        }
                                    }
                                    else
                                    {
                                        var path = AlgosUtils.DijstraPathfinding(mainScript.PcgManager.gridArray2D, endPos, new Vector2Int((int)Mathf.Round(lastBezierCurvePoint.x), (int)Mathf.Round(lastBezierCurvePoint.z)), true);

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
