using Codice.Client.BaseCommands.Changelist;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(PerlinNoiseMA))]
public class PerlinNoiseEditor : Editor
{
    bool showCA = false;
    string status = "Use Cellular Automata to tidy up";

    bool showPath = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PerlinNoiseMA mainScript = (PerlinNoiseMA)target;


        //specific to Main algo

        mainScript.OffsetX = (int)EditorGUILayout.Slider(mainScript.OffsetX, 0, 10000);
        mainScript.OffsetY = (int)EditorGUILayout.Slider(mainScript.OffsetY, 0, 10000);
        
        
        mainScript.Scale = EditorGUILayout.Slider(mainScript.Scale, 3f, 35f);
        mainScript.Octaves = (int)EditorGUILayout.Slider(mainScript.Octaves, 1, 8);

        mainScript.Persistance = EditorGUILayout.Slider(mainScript.Persistance, 0.1f, 0.9f);
        mainScript.Lacunarity = EditorGUILayout.Slider(mainScript.Lacunarity, 0.5f, 10f);

        mainScript.Threshold = EditorGUILayout.Slider(mainScript.Threshold, 0.1f, 0.9f);

        if (GUILayout.Button("Generate Noise"))
        {
            mainScript.PcgManager.gridArray2D = AlgosUtils.PerlinNoise2D(mainScript.PcgManager.gridArray2D, mainScript.Scale, mainScript.Octaves, mainScript.Persistance, mainScript.Lacunarity, mainScript.OffsetX, mainScript.OffsetY, mainScript.Threshold);

            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D, true);

            mainScript.Started = true;
        }






        //general




        if (mainScript.Started)
        {
            //stuff for the Cellular automata

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
                mainScript.NeighboursNeeded = (int)EditorGUILayout.Slider(mainScript.NeighboursNeeded, 3, 5);


                if (GUILayout.Button("Clean Up CA"))
                {
                    AlgosUtils.CleanUp2dCA(mainScript.PcgManager.gridArray2D, mainScript.NeighboursNeeded);

                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(mainScript.PcgManager.gridArray2D);
                }
                if (GUILayout.Button("CA iteration"))
                {
                    AlgosUtils.RunCaIteration2D(mainScript.PcgManager.gridArray2D, mainScript.NeighboursNeeded);
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



            mainScript.MinSize = (int)EditorGUILayout.Slider(mainScript.MinSize, 30, 200);

            GUILayout.Label($"Delete all the rooms beneath {mainScript.MinSize} tiles big");

            if (GUILayout.Button("Delete small rooms"))
            {
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








            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();



            if (mainScript.rooms.Count > 1)
            {
                showPath = EditorGUILayout.BeginFoldoutHeaderGroup(showPath, "Pathfinding Settings");

                if (showPath)
                {
                    GUILayout.Label("Decide What Pathfinding Algo to use");

                    mainScript.TypeOfTri = EditorGUILayout.Toggle("Should it use prim's also", mainScript.TypeOfTri);
                    mainScript.PathType = EditorGUILayout.Toggle("Use straight Lines", mainScript.PathType);


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


                        if (mainScript.TypeOfTri)
                            mainScript.edges = AlgosUtils.PrimAlgoNoDelu(centerPoints);
                        else
                            mainScript.edges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;


                        foreach (var edge in mainScript.edges)
                        {

                            //use where so we get soemthing its not the wall but not necessary
                            var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                            var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;


                            var path = AlgosUtils.A_StarPathfinding2DNorm(mainScript.PcgManager.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), mainScript.PathType);


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


}
