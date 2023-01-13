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





    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RanRoomGenMA mainScript = (RanRoomGenMA)target;


        mainScript.BPSg = EditorGUILayout.Toggle(mainScript.BPSg == true ? "BPS selected" : "Random room allocation selected", mainScript.BPSg);
        if (!mainScript.BPSg)
        {
            GUILayout.Label($"Max heihgt of the room {mainScript.MaxHeight}");
            mainScript.MaxHeight = (int)EditorGUILayout.Slider(mainScript.MaxHeight, 15, 125);

            if (mainScript.MaxHeight <= mainScript.MinHeight)
            {
                mainScript.MaxHeight = mainScript.MinHeight + 1;
            }
        }
       

        GUILayout.Label($"Min Height of the room {mainScript.MinHeight}");
        mainScript.MinHeight = (int)EditorGUILayout.Slider(mainScript.MinHeight, 10, 75);




        if (!mainScript.BPSg)
        {
            GUILayout.Label($"Max width of the room {mainScript.MaxWidth}");
            mainScript.MaxWidth = (int)EditorGUILayout.Slider(mainScript.MaxWidth, 15, 125);
            if (mainScript.MaxWidth <= mainScript.MinWidth)
            {
                mainScript.MaxWidth = mainScript.MinWidth + 1;
            }

        }


        GUILayout.Label($"Min Width of the room {mainScript.MinWidth}");
        mainScript.MinWidth = (int)EditorGUILayout.Slider(mainScript.MinWidth, 10, 75);





        GUILayout.Label($"maximum number of rooms to try and spawn {mainScript.NumOfRoom}");
        mainScript.NumOfRoom = (int)EditorGUILayout.Slider(mainScript.NumOfRoom, 3, 30);




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
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);
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
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);

                if (specialAlgo)
                    mainScript.RanRoom(mainScript.BPSg, CAselected);


                mainScript.Started = true;
            }
        }





        if (mainScript.Started)
        {

            Spaces(4);


            #region showCA region

            showCA = EditorGUILayout.BeginFoldoutHeaderGroup(showCA, "Use Cellular Automata(CA) to tidy up");

            if (showCA)
            {

                GUILayout.Label(new GUIContent() { text = "Neighbours needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" });
                NeighboursNeeded = (int)EditorGUILayout.Slider(NeighboursNeeded, 3, 5);

                if (GUILayout.Button(new GUIContent() { text = "Clean Up using CA", tooltip = "Run half of the CA algortihm to only take out tiles, to help slim down the result" }))
                {
                    AlgosUtils.CleanUp2dCA(mainScript.PcgManager.gridArray2D, NeighboursNeeded);

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }
                if (GUILayout.Button(new GUIContent() { text = "Use CA algorithm", tooltip = "Run the full CA algorithm on the current iteration of the grid" }))
                {
                    AlgosUtils.RunCaIteration2D(mainScript.PcgManager.gridArray2D, NeighboursNeeded);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }

            }

            if (!Selection.activeTransform)
            {
                showCA = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion


            Spaces(4);



            #region Room Region

            showRooms = EditorGUILayout.BeginFoldoutHeaderGroup(showRooms, "Rooms section");

            if (showRooms)
            {



                if (GUILayout.Button("Get all rooms"))
                {
                    mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextSelfCol(mainScript.PcgManager.gridArray2D);
                }

                mainScript.MinSizeRoom = (int)EditorGUILayout.Slider(mainScript.MinSizeRoom, 30, 200);

                GUILayout.Label($"Delete all the rooms beneath {mainScript.MinSizeRoom} tiles big");

                if (GUILayout.Button("Delete small rooms"))
                {

                    mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);

                    foreach (var room in mainScript.rooms)
                    {
                        if (room.Count < mainScript.MinSizeRoom)
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



            Spaces(4);


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
                        var roomDict = new Dictionary<Vector2, List<BasicTile>>();
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
                                if (tile.tileType != BasicTile.TileType.FLOORROOM)
                                    tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                                tile.tileWeight = 0.75f;
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
                if (GUILayout.Button("Generate walls"))
                {
                    AlgosUtils.SetUpTileTypesFloorWall(mainScript.PcgManager.gridArray2D);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
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




            Spaces(4);



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
                        mainScript.PcgManager.DrawTileMap();
                        break;
                }
            }


            #endregion
        }

    }



    private void Spaces(int spaceNum)
    {
        for (int i = 0; i < spaceNum; i++)
        {
            EditorGUILayout.Space();
        }
    }

}
