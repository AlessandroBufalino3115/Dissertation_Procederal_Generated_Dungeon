using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(RandomWalkMA))]
public class RandomWalkEditor : Editor
{

    bool usedPathing = false;

    bool showRules = false;

    bool useWeights = false;
    bool DjAvoidWalls = false;

    int corridorThickness = 2;

    int selGridConnectionType = 0;
    GUIContent[] selStringsConnectionType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualar path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have many corridors" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridor connections" } };
    int randomAddCorr = 0;

    int selGridPathGenType = 0;
    GUIContent[] selStringPathGenType = { new GUIContent() { text = "A* pathfinding", tooltip = "" }, new GUIContent() { text = "Dijistra", tooltip = "" }, new GUIContent() { text = "BFS (WIP)", tooltip = "" }, new GUIContent() { text = "DFS (WIP)", tooltip = "" }, new GUIContent() { text = "Beizier Curve", tooltip = "Create curved corridors" } };
    int margin = 20;

    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

    bool algoForBezier = false;


    int deadEndAmount = 0;
    int deadEndCorridorThickness = 3;

    int radius = 10;

    string saveMapFileName = "";

    int width = 0;
    int height = 0;


    private bool blockGeneration = false;

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


        switch (mainScript.currUiState)
        {
            case RandomWalkMA.UISTATE.MAIN_ALGO:

                mainScript.allowedBack = false;

                mainScript.iterations = (int)EditorGUILayout.Slider(new GUIContent() { text = "Iterations", tooltip = "This is how many times the head of the algorithm is going to move" }, mainScript.iterations, (mainScript.pcgManager.gridArray2D.Length * mainScript.pcgManager.gridArray2D[0].Length) * 0.3f, (mainScript.pcgManager.gridArray2D.Length * mainScript.pcgManager.gridArray2D[0].Length) * 0.9f);

                mainScript.startFromMiddle = EditorGUILayout.Toggle(new GUIContent() { text = "Should The algo start from the middle", tooltip = "Should the head of the algorithm start from the middle of the canvas or a random position?" }, mainScript.startFromMiddle); ;
                mainScript.alreadyPassed = EditorGUILayout.Toggle(new GUIContent() { text = "Overlap cells count", tooltip = mainScript.alreadyPassed == true ? "When the head of the walker goes over an already populated cells the iteration still counts" : "When the head of the walker goes over an already populated cells the iteration does not count" }, mainScript.alreadyPassed);

                if (GUILayout.Button("Generate RandomWalk Randomisation"))// gen something
                {
                    AlgosUtils.RestartArr(mainScript.pcgManager.gridArray2D);
                    mainScript.pcgManager.gridArray2D = AlgosUtils.RandomWalk2DCol(mainScript.iterations, !mainScript.alreadyPassed, mainScript.pcgManager.gridArray2D[0].Length, mainScript.pcgManager.gridArray2D.Length, randomStart: !mainScript.startFromMiddle);
                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArray2D);

                    mainScript.allowedForward = true;
                }

                break;

            case RandomWalkMA.UISTATE.CA:

                mainScript.allowedForward = true;
                mainScript.allowedBack = true;

                GeneralUtil.CellularAutomataEditorSection(mainScript.pcgManager, mainScript.neighboursNeeded, out mainScript.neighboursNeeded);


                break;
            case RandomWalkMA.UISTATE.ROOM_GEN:


                mainScript.allowedBack = true;


                List<List<Tile>> rooms;
                if (GeneralUtil.CalculateRoomsEditorSection(mainScript.pcgManager, mainScript.minSize, out rooms, out mainScript.minSize)) 
                {
                    mainScript.allowedForward = true;
                }

                if (rooms != null)
                    mainScript.rooms = rooms;



                break;
            case RandomWalkMA.UISTATE.EXTRA_ROOM_GEN:

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

                        var squareRoom = AlgosUtils.SpawnRoom(width, height, randomPoint, mainScript.pcgManager.gridArray2D,true);

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



                break;
            case RandomWalkMA.UISTATE.PATHING:


                #region corridor making region

                if (!usedPathing)
                {

                    mainScript.allowedBack = true;
                    Debug.Log(mainScript.rooms.Count);
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
                        selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, selStringPathGenType, 1);
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
                                algoForBezier = EditorGUILayout.Toggle(new GUIContent() { text = algoForBezier == true ? "A* Pathfinding is selected" : "Dijistra is selected", tooltip = "" }, algoForBezier);

                                if (algoForBezier)
                                {
                                    GeneralUtil.SpacesUILayout(1);

                                    mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                                    useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                }

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
                            usedPathing = true;

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

                                    AlgosUtils.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), margin, algoForBezier, mainScript.pcgManager.gridArray2D, !mainScript.pathType, useWeights: useWeights, tileCosts: mainScript.pcgManager.tileCosts);

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
                                mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
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

                                    mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "Pathfinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                                    useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                }

                                break;

                            default:
                                break;
                        }


                        if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                        {
                            mainScript.allowedForward = true;
                            usedPathing = true;

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
                                        //use where so we get soemthing its not the wall but not necessary
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


                                        AlgosUtils.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), margin, algoForBezier, mainScript.pcgManager.gridArray2D, !mainScript.pathType, useWeights: useWeights, tileCosts: mainScript.pcgManager.tileCosts);

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





                    //deadEndAmount = (int)EditorGUILayout.Slider(new GUIContent() { text = "Amount of dead end corridors", tooltip = "Dead end corridors start from somewhere in the dungeon and lead to nowhere" }, deadEndAmount, 0, 5);

                    //deadEndCorridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the dead end corridor", tooltip = "How wide should the corridor be" }, deadEndCorridorThickness, 3, 6);

                    //if (GUILayout.Button(new GUIContent() { text = "Generate dead end corridor" }))
                    //{
                    //    for (int i = 0; i < deadEndAmount; i++)
                    //    {
                    //        var room = mainScript.rooms[GeneralUtil.ReturnRandomFromList(mainScript.rooms)];

                    //        var randomTileInRoom = room[GeneralUtil.ReturnRandomFromList(room)];

                    //        Tile randomTileOutsideOfRoom;


                    //        while (true)
                    //        {
                    //            var tile = mainScript.pcgManager.gridArray2D[Random.Range(0, mainScript.pcgManager.gridArray2D.Length)][Random.Range(0, mainScript.pcgManager.gridArray2D[0].Length)];

                    //            if (tile.tileWeight == 0)
                    //            {
                    //                mainScript.pcgManager.CreateBackUpGrid();

                    //                randomTileOutsideOfRoom = tile;

                    //                var tileA = randomTileOutsideOfRoom.position;
                    //                var tileB = randomTileInRoom.position;

                    //                AlgosUtils.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), margin, algoForBezier, mainScript.pcgManager.gridArray2D, !mainScript.pathType, useWeights: useWeights, tileCosts: mainScript.pcgManager.tileCosts);

                    //                break;
                    //            }
                    //        }
                    //    }

                    //    AlgosUtils.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArray2D, deadEndCorridorThickness);

                    //    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArray2D, 0, 1, true);
                    //}

                }

                #endregion

                break;

            case RandomWalkMA.UISTATE.GENERATION:


                mainScript.allowedBack = true;

                GeneralUtil.GenerateMeshEditorSection(mainScript.pcgManager, selGridGenType, blockGeneration, saveMapFileName, out selGridGenType, out blockGeneration, out saveMapFileName);



                break;

            default:
                break;
        }



        if (mainScript.currUiState != RandomWalkMA.UISTATE.GENERATION)
        {


           
            GeneralUtil.SpacesUILayout(4);

            EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

            if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
            {
                mainScript.pcgManager.ClearUndos();
                mainScript.allowedBack = false;
                mainScript.currStateIndex--;
                mainScript.currUiState = (RandomWalkMA.UISTATE)mainScript.currStateIndex;
            }

            EditorGUI.EndDisabledGroup();



            EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

            if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
            {
                mainScript.pcgManager.ClearUndos();
                mainScript.allowedForward = false;
                mainScript.currStateIndex++;
                mainScript.currUiState = (RandomWalkMA.UISTATE)mainScript.currStateIndex;
            }

            EditorGUI.EndDisabledGroup();
        }
    }








}


