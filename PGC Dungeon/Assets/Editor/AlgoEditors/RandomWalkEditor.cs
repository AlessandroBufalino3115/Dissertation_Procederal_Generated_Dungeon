using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(RandomWalkMA))]
public class RandomWalkEditor : Editor
{
    bool showCA = false;

    bool showPath = false;
    bool showRooms = false;

    bool showRules = false;



    int selGridPathType = 0;
    GUIContent[] selStringsPathType = { new GUIContent() {text = "Prims's algo", tooltip=  "Create a singualr path that traverses the whole dungeon"}, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have manu corridors" }, new GUIContent() { text = "Prim's algo + random", tooltip = "Create a singualr path that traverses the whole dungeon, with some random diviation" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };


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


        Spaces(4);


        #region Main algo region



        GUILayout.Label("How many iterations");


        mainScript.Iterations = (int)EditorGUILayout.Slider(mainScript.Iterations, (mainScript.PcgManager.gridArray2D.Length * mainScript.PcgManager.gridArray2D[0].Length) * 0.3f, (mainScript.PcgManager.gridArray2D.Length * mainScript.PcgManager.gridArray2D[0].Length) * 0.9f);

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

            Spaces(4);


            #region showCA region

            showCA = EditorGUILayout.BeginFoldoutHeaderGroup(showCA, "Use Cellular Automata(CA) to tidy up");

            if (showCA)
            {
               
                GUILayout.Label(new GUIContent() { text = "Neighbours needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" });
                mainScript.NeighboursNeeded = (int)EditorGUILayout.Slider(mainScript.NeighboursNeeded, 3, 5);

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



                mainScript.MinSize = (int)EditorGUILayout.Slider(mainScript.MinSize, 30, 200);

                GUILayout.Label($"Delete all the rooms beneath {mainScript.MinSize} tiles big");

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

            if (GUILayout.Button(new GUIContent() { text = "Generate YOUR Dungeon!"}))
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
