

namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(RandomWalkMA))]
    public class RandomWalkEditor : Editor
    {
        bool showRules = false;

        bool useWeights = false;
        bool DjAvoidWalls = false;

        int corridorThickness = 2;

        int selGridConnectionType = 0;
        int selGridPathGenType = 0;

        int randomAddCorr = 0;

        int bezierOndulation = 20;
        int deadEndOndulation = 20;

        int deadEndAmount = 0;
        int deadEndCorridorThickness = 3;

        int radius = 10;

        int width = 10;
        int height = 10;

        string saveMapFileName = "";

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

            DFEditorUtil.SpacesUILayout(4);


            switch (mainScript.currUiState)
            {
                case DFEditorUtil.UI_STATE.MAIN_ALGO:
                    {
                        mainScript.allowedBack = false;

                        mainScript.iterations = (int)EditorGUILayout.Slider(new GUIContent() { text = "Iterations", tooltip = "This is how many times the head of the algorithm is going to move" }, mainScript.iterations, (mainScript.pcgManager.gridArr.GetLength(1) * mainScript.pcgManager.gridArr.GetLength(0)) * 0.3f, (mainScript.pcgManager.gridArr.GetLength(1) * mainScript.pcgManager.gridArr.GetLength(0)) * 0.9f);

                        mainScript.startFromMiddle = EditorGUILayout.Toggle(new GUIContent() { text = "Should The algo start from the middle", tooltip = "Should the head of the algorithm start from the middle of the canvas or a random position?" }, mainScript.startFromMiddle); ;
                        mainScript.alreadyPassed = EditorGUILayout.Toggle(new GUIContent() { text = "Overlap cells count", tooltip = mainScript.alreadyPassed == true ? "When the head of the walker goes over an already populated cells the iteration still counts" : "When the head of the walker goes over an already populated cells the iteration does not count" }, mainScript.alreadyPassed);

                        if (GUILayout.Button("Generate RandomWalk Randomisation"))// gen something
                        {
                            DFGeneralUtil.RestartGrid(mainScript.pcgManager.gridArr);
                            DFAlgoBank.RandomWalk(mainScript.pcgManager.gridArr,mainScript.iterations, !mainScript.alreadyPassed, randomStart: !mainScript.startFromMiddle);
                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArr);

                            mainScript.allowedForward = true;
                        }
                    }
                    break;

                case DFEditorUtil.UI_STATE.CA:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = true;

                        DFEditorUtil.CellularAutomataEditorSection(mainScript.pcgManager, mainScript.neighboursNeeded, out mainScript.neighboursNeeded);
                    }
                    break;

                case DFEditorUtil.UI_STATE.ROOM_GEN:
                    {
                        mainScript.allowedBack = true;

                        List<List<DFTile>> rooms;
                        if (DFEditorUtil.CalculateRoomsEditorSection(mainScript.pcgManager, mainScript.minSize, out rooms, out mainScript.minSize))
                        {
                            mainScript.allowedForward = true;
                        }

                        if (rooms != null)
                        {
                            mainScript.rooms = rooms;
                        }
                    }
                    break;

                case DFEditorUtil.UI_STATE.EXTRA_ROOM_GEN:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = false;

                        DFEditorUtil.ExtraRoomEditorSelection(mainScript.pcgManager, mainScript.rooms, radius, height, width, out height, out width, out radius);
                    }
                    break;

                case DFEditorUtil.UI_STATE.PATHING:

                    #region corridor making region

                    if (mainScript.rooms.Count == 1)
                    {
                        mainScript.allowedForward = true;
                        GUILayout.Label("Only one room detected, Corridor making is not possible");
                    }
                    else if (mainScript.rooms.Count == 2)
                    {
                        GUILayout.Label("Only two rooms detected, triangulation not possible");

                        GUILayout.Label("Choose the algorithm to create the corridor");

                        DFEditorUtil.SpacesUILayout(2);

                        GUILayout.BeginVertical("Box");
                        selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, DFEditorUtil.selStringPathGenType, 1);
                        GUILayout.EndVertical();

                        DFEditorUtil.SpacesUILayout(2);

                        switch (selGridPathGenType)
                        {
                            case 0:   // A* pathfindind
                                mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                                useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                break;

                            case 1:   // djistra 
                                DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                                break;
                            case 2:   // beizier 

                                bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "beizeir curve thing to change" }, bezierOndulation, 10, 40);

                                DFEditorUtil.SpacesUILayout(1);

                                mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);

                                break;

                            default:
                                break;
                        }


                        EditorGUI.BeginDisabledGroup(mainScript.pcgManager.prevGridArray2D.Count == 1);

                        if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                        {
                            mainScript.pcgManager.CreateBackUpGrid();

                            Vector2Int tileA = mainScript.rooms[0][Random.Range(0, mainScript.rooms[0].Count - 1)].position;
                            Vector2Int tileB = mainScript.rooms[1][Random.Range(0, mainScript.rooms[1].Count - 1)].position;


                            mainScript.allowedForward = true;

                            switch (selGridPathGenType)
                            {
                                case 0:   //A* pathfingin

                                    var path = DFAlgoBank.A_StarPathfinding2D(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.pathType, useWeights: useWeights, arrWeights: mainScript.pcgManager.tileCosts);

                                    DFAlgoBank.SetUpCorridorWithPath(path.Item1);

                                    break;
                                case 1:  //dijistra

                                    var pathD = DFAlgoBank.DijstraPathfinding(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);

                                    DFAlgoBank.SetUpCorridorWithPath(pathD);


                                    break;
                                case 2://  beizier curve

                                    DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.pcgManager.gridArr, !mainScript.pathType);

                                    break;

                                default:
                                    break;
                            }


                            DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorThickness);

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }

                        EditorGUI.EndDisabledGroup();

                        if (mainScript.pcgManager.prevGridArray2D.Count == 1)
                        {
                            mainScript.allowedForward = true;
                            mainScript.allowedBack = false;
                        }
                        else
                        {
                            mainScript.allowedForward = false;
                            mainScript.allowedBack = true;
                        }
                    }

                    else if (mainScript.rooms.Count > 2)
                    {

                        GUILayout.Label("Choose how to order the connection of the rooms");

                        DFEditorUtil.SpacesUILayout(2);

                        GUILayout.BeginVertical("Box");
                        selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, DFEditorUtil.selStringsConnectionType, 1);
                        GUILayout.EndVertical();

                        DFEditorUtil.SpacesUILayout(2);

                        GUILayout.Label("Choose the Thickness of the corridor");

                        corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the corridor", tooltip = "How wide should the corridor be" }, corridorThickness, 2, 5);

                        DFEditorUtil.SpacesUILayout(3);


                        GUILayout.Label("Choose the algorithm to that creates the corridor");


                        DFEditorUtil.SpacesUILayout(2);

                        GUILayout.BeginVertical("Box");
                        selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, DFEditorUtil.selStringPathGenType, 1);
                        GUILayout.EndVertical();

                        DFEditorUtil.SpacesUILayout(2);


                        switch (selGridPathGenType)
                        {
                            case 0:   // A* pathfindind
                                mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                                useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                break;

                            case 1:   // djistra 
                                DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                                break;
                            case 2:   // beizier 

                                bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, bezierOndulation, 10, 40);

                                break;

                            default:
                                break;
                        }


                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                        DFEditorUtil.SpacesUILayout(3);

                        switch (selGridConnectionType)
                        {
                            case 0:   // prims ran

                                if (mainScript.rooms.Count >= 4)
                                {
                                    randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                                    DFEditorUtil.SpacesUILayout(2);
                                }
                                break;

                            case 2:

                                if (mainScript.rooms.Count >= 4)
                                {
                                    randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                                    DFEditorUtil.SpacesUILayout(2);
                                }
                                break;

                            default:
                                break;
                        }


                        DFEditorUtil.SpacesUILayout(1);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        DFEditorUtil.SpacesUILayout(1);


                        deadEndAmount = (int)EditorGUILayout.Slider(new GUIContent() { text = "Amount of dead end corridors", tooltip = "Dead end corridors start from somewhere in the dungeon and lead to nowhere" }, deadEndAmount, 0, 5);

                        deadEndCorridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the dead end corridor", tooltip = "How wide should the corridor be" }, deadEndCorridorThickness, 3, 6);

                        deadEndOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier for dead end", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, deadEndOndulation, 10, 40);

                        DFEditorUtil.SpacesUILayout(2);


                        EditorGUI.BeginDisabledGroup(mainScript.pcgManager.prevGridArray2D.Count == 1);

                        if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                        {
                            mainScript.pcgManager.CreateBackUpGrid();

                            mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr, true);
                            var centerPoints = new List<Vector2>();
                            var roomDict = new Dictionary<Vector2Int, List<DFTile>>();
                            foreach (var room in mainScript.rooms)
                            {
                                var centerPoint = DFGeneralUtil.FindMiddlePoint(room);

                                roomDict.Add(new Vector2Int(Mathf.FloorToInt(centerPoint.x), Mathf.FloorToInt(centerPoint.y)), room);
                                centerPoints.Add(new Vector2Int(Mathf.FloorToInt(centerPoint.x), Mathf.FloorToInt(centerPoint.y)));
                            }

                            switch (selGridConnectionType)
                            {
                                case 0:
                                    mainScript.edges = DFAlgoBank.PrimAlgoNoDelu(centerPoints);
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
                                                if (DFAlgoBank.LineIsEqual(primEdge, newEdge))
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
                                    mainScript.edges = DFAlgoBank.DelaunayTriangulation(centerPoints).Item2;
                                    break;

                                case 2://ran
                                    {

                                        DFAlgoBank.ShuffleList(mainScript.rooms);

                                        foreach (var item in roomDict.Keys)
                                        {
                                            Debug.Log(item);
                                        }

                                        for (int i = 0; i < centerPoints.Count; i++)
                                        {
                                            if (i == centerPoints.Count - 1) { continue; }
                                            mainScript.edges.Add(new Edge(new Vector3(Mathf.FloorToInt( centerPoints[i].x), Mathf.FloorToInt(centerPoints[i].y), 0), new Vector3(Mathf.FloorToInt(centerPoints[i + 1].x), Mathf.FloorToInt(centerPoints[i + 1].y), 0)));
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
                                    }
                                    break;
                            }

                            //its a roudning error

                            switch (selGridPathGenType)
                            {
                                case 0:   //A* pathfingin

                                    foreach (var edge in mainScript.edges)
                                    {
                                        //use where so we get soemthing its not the wall but not necessary
                                        var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                                        var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                                        var path = DFAlgoBank.A_StarPathfinding2D(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.pathType, useWeights: useWeights, arrWeights: mainScript.pcgManager.tileCosts);

                                        DFAlgoBank.SetUpCorridorWithPath(path.Item1);
                                    }

                                    break;
                                case 1:  //dijistra
                                    foreach (var edge in mainScript.edges)
                                    {
                                        var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                                        var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                                        var path = DFAlgoBank.DijstraPathfinding(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);

                                        DFAlgoBank.SetUpCorridorWithPath(path);
                                    }

                                    break;

                                case 2://  beizier curve
                                    foreach (var edge in mainScript.edges)
                                    {
                                      //  Debug.Log(new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].z));

                                        var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))   ][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                                        var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                                        DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.pcgManager.gridArr, !mainScript.pathType);
                                    }
                                    break;

                                default:
                                    break;
                            }

                            for (int i = 0; i < deadEndAmount; i++)
                            {
                                var room = mainScript.rooms[DFGeneralUtil.ReturnRandomFromList(mainScript.rooms)];

                                var randomTileInRoom = room[DFGeneralUtil.ReturnRandomFromList(room)];

                                DFTile randomTileOutsideOfRoom;

                                while (true)
                                {
                                    var tile = mainScript.pcgManager.gridArr[Random.Range(0, mainScript.pcgManager.gridArr.GetLength(0)), Random.Range(0, mainScript.pcgManager.gridArr.GetLength(1))];

                                    if (tile.tileWeight == 0)
                                    {
                                        randomTileOutsideOfRoom = tile;

                                        var tileA = randomTileOutsideOfRoom.position;
                                        var tileB = randomTileInRoom.position;

                                        DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.pcgManager.gridArr);

                                        break;
                                    }
                                }
                            }

                            DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorThickness);

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }

                        EditorGUI.EndDisabledGroup();


                        if (mainScript.pcgManager.prevGridArray2D.Count == 1)
                        {
                            mainScript.allowedForward = true;

                            mainScript.allowedBack = false;
                        }
                        else
                        {
                            mainScript.allowedForward = false;

                            mainScript.allowedBack = true;
                        }
                    }

                    else
                    {
                        GUILayout.Label("To access the corridor making function you need to\nGenerate the rooms first");
                    }

                    #endregion

                    break;

                case DFEditorUtil.UI_STATE.GENERATION:
                    {
                        mainScript.allowedBack = true;

                        DFEditorUtil.SaveGridDataToGenerate(mainScript.pcgManager, saveMapFileName, out saveMapFileName);
                    }

                    break;

                default:
                    break;
            }



            if (mainScript.currUiState != DFEditorUtil.UI_STATE.GENERATION)
            {
                DFEditorUtil.SpacesUILayout(4);

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedBack = false;
                    mainScript.currStateIndex--;
                    mainScript.currUiState = (DFEditorUtil.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();



                EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

                if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.currUiState = (DFEditorUtil.UI_STATE)mainScript.currStateIndex;

                }

                EditorGUI.EndDisabledGroup();
            }
        }
    }
}