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

    bool PathType;
    bool TypeOfTri;

    int MinSize;

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



        mainScript.Additive = EditorGUILayout.Toggle("additive", mainScript.Additive);



        specialAlgo = EditorGUILayout.Toggle("special algo toggle", specialAlgo);
        if (specialAlgo) 
        {

            CAselected = EditorGUILayout.Toggle(CAselected == true ? "CA selected for gen" : "drunk walk selected", CAselected);
        }


        if (mainScript.BPSg) 
        {
            if (GUILayout.Button("Use BPS algo"))// gen something
            {
                mainScript.RoomList.Clear();


                if (!mainScript.Additive)
                {
                    mainScript.RoomList.Clear();
                    mainScript.PcgManager.gridArray2D = AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
                }


                mainScript.BPSRoomGen(mainScript.PcgManager.gridArray2D);
                mainScript.SetUpWeights(mainScript.PcgManager.gridArray2D);
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);
                if (specialAlgo)
                    mainScript.RanRoom(mainScript.BPSg, CAselected);
            }



        }
        else 
        {
            if (GUILayout.Button("Use random Room gen"))// gen something
            {
                mainScript.roomList.Clear();

                if (!mainScript.Additive)
                {
                    mainScript.PcgManager.gridArray2D = AlgosUtils.RestartArr(mainScript.PcgManager.gridArray2D);
                }

                mainScript.RandomRoomGen(mainScript.PcgManager.gridArray2D);
                mainScript.SetUpWeights(mainScript.PcgManager.gridArray2D);
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);

                if (specialAlgo)
                    mainScript.RanRoom(mainScript.BPSg, CAselected);
            }
        }









        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        showCA = EditorGUILayout.BeginFoldoutHeaderGroup(showCA, status);

        if (showCA)
        {
            if (Selection.activeTransform)
            {
                status = "Cellular Automata Settings";
            }


            GUILayout.Label("How many neighbours");
            NeighboursNeeded = (int)EditorGUILayout.Slider(NeighboursNeeded, 3, 5);


            if (GUILayout.Button("Clean Up CA"))
            {
                AlgosUtils.CleanUp2dCA(mainScript.PcgManager.gridArray2D, NeighboursNeeded);

                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
            }
            if (GUILayout.Button("CA iteration"))
            {
                AlgosUtils.RunCaIteration2D(mainScript.PcgManager.gridArray2D, NeighboursNeeded);
                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
            }

        }





        if (!Selection.activeTransform)
        {
            status = "Use Cellular Automata to tidy up";
            showCA = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();









        if (GUILayout.Button("Get all rooms"))
        {
            mainScript.rooms = AlgosUtils.GetAllRooms(mainScript.PcgManager.gridArray2D, true);
            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextSelfCol(mainScript.PcgManager.gridArray2D);
        }



        MinSize = (int)EditorGUILayout.Slider(MinSize, 30, 200);

        GUILayout.Label($"Delete all the rooms beneath {MinSize} tiles big");

        if (GUILayout.Button("Delete small rooms"))
        {
            foreach (var room in mainScript.rooms)
            {
                if (room.Count < MinSize)
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


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();









        if (mainScript.rooms.Count > 1)
        {
            showPath = EditorGUILayout.BeginFoldoutHeaderGroup(showPath, "Pathfinding Settings");

            if (showPath)
            {
                GUILayout.Label("Decide What Pathfinding Algo to use");

                TypeOfTri = EditorGUILayout.Toggle("Should it use prim's also", TypeOfTri);
                PathType = EditorGUILayout.Toggle("Use straight Lines", PathType);


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


                    if (TypeOfTri)
                        mainScript.edges = AlgosUtils.PrimAlgoNoDelu(centerPoints);
                    else
                        mainScript.edges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;


                    foreach (var edge in mainScript.edges)
                    {

                        //use where so we get soemthing its not the wall but not necessary
                        var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                        var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;


                        var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), PathType);


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
            status = "Pathfinding Settings";
            showPath = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();
        EditorGUILayout.Space();



        if (GUILayout.Button("Generate Mesh"))
        {
            mainScript.PcgManager.FormObject(AlgosUtils.MarchingCubesAlgo(AlgosUtils.ExtrapolateMarchingCubes(mainScript.PcgManager.gridArray2D, mainScript.PcgManager.RoomHeight), false));
        }










    }
}
