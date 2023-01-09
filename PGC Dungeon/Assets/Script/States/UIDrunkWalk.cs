using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class UIDrunkWalk : UiBaseState
{
   // add tge other things 
   // a nice order
   // diags for path




    private int iterations;
    private int iterationsLeft;

    private bool startFromMiddle = false;
    private bool pathType = false;

    private bool alreadyPassed;

    private int neighboursNeeded = 3;

    private bool typeOfTri;
    private int minSize;

    private List<List<BasicTile>> rooms;

    //private bool hardStop;
    private List<Edge> primEdges = new List<Edge>();

    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {


        if (currentMenu.working)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "LOADING");
        }
        else
        {
            GUI.Box(new Rect(5, 10, 230, 650), "");   //background 


            if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                iterations = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), iterations, 20, (int)((currentMenu.gridArrayObj2D.Length * currentMenu.gridArrayObj2D[0].Length)) * 0.9f);

            //else if (currentMenu.dimension == StateUIManager.Dimension.THREED)
            //    iterations = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), iterations, 20, (int)((currentMenu.gridArray3D.Length * currentMenu.gridArray3D[0].Length * currentMenu.gridArray3D[0][0].Length)) * 0.9f);

            else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                iterations = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), iterations, 20, (int)((currentMenu.gridArray2D.Length * currentMenu.gridArray2D[0].Length)) * 0.9f);



            GUI.Label(new Rect(130, 45, 100, 30), "iterations: " + iterations);

            GUI.Label(new Rect(130, 70, 100, 30), "Iterations Left: " + iterationsLeft);



            startFromMiddle = GUI.Toggle(new Rect(120, 100, 150, 30), startFromMiddle, startFromMiddle == false ? "starting from mid" : "starting randomly");

            alreadyPassed = GUI.Toggle(new Rect(10, 100, 120, 30), alreadyPassed, alreadyPassed != true ? "overlap cells" : "dont overlap");

            if (GUI.Button(new Rect(10, 140, 120, 30), "Run Drunk Walk"))
            {
                currentMenu.working = true;

                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    DrunkWalkObj2D(currentMenu);

                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                    DrunkWalk2D(currentMenu);


                currentMenu.working = false;
            }

            if (GUI.Button(new Rect(10, 180, 120, 30), "Run CA cleanup"))
            {
                currentMenu.working = true;
                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    AlgosUtils.CleanUp2dCA(currentMenu.gridArrayObj2D, neighboursNeeded);

                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE) 
                {
                    AlgosUtils.CleanUp2dCA(currentMenu.gridArray2D, neighboursNeeded);

                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D);
                }

                currentMenu.working = false;

            }

            if (GUI.Button(new Rect(10, 220, 120, 30), "Run CA iteration"))
            {

                currentMenu.working = true;

                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    AlgosUtils.RunCaIteration2D(currentMenu.gridArrayObj2D, neighboursNeeded);

                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE) 
                {
                    AlgosUtils.RunCaIteration2D(currentMenu.gridArray2D, neighboursNeeded);
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D);
                }


                currentMenu.working = false;
            }

            neighboursNeeded = (int)GUI.HorizontalSlider(new Rect(130, 210, 100, 20), neighboursNeeded, 3, 5);

            GUI.Label(new Rect(140, 220, 100, 40), "Needed neighbours: " + neighboursNeeded);




            if (GUI.Button(new Rect(10, 280, 120, 30), "Run Wall finding"))
            {

                currentMenu.working = true;
                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    AlgosUtils.SetUpTileTypesFloorWall(currentMenu.gridArrayObj2D);

                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE) 
                {
                    AlgosUtils.SetUpTileTypesFloorWall(currentMenu.gridArray2D);
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColShade(currentMenu.gridArray2D, 0, 1,true);
                }


                currentMenu.working = false;

            }


            if (GUI.Button(new Rect(10, 320, 120, 30), "Set up corridors"))
            {

                currentMenu.working = true;

                if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                {
                    AlgosUtils.SetUpTileTypesCorridor(currentMenu.gridArray2D);
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColShade(currentMenu.gridArray2D, 0, 1, true);
                }


                currentMenu.working = false;
            }



            if (GUI.Button(new Rect(10, 360, 120, 30), "Get All rooms"))
            {


                currentMenu.working = true;

                if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    AlgosUtils.SetUpTileTypesFloorWall(currentMenu.gridArrayObj2D);

                else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                {
                    rooms = AlgosUtils.GetAllRooms(currentMenu.gridArray2D,true);
                    currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextSelfCol(currentMenu.gridArray2D);
                }


                currentMenu.working = false;
            }



            typeOfTri = GUI.Toggle(new Rect(140, 415, 100, 30), typeOfTri, typeOfTri != true ? "delu" : "prims");
            pathType = GUI.Toggle(new Rect(140, 395, 120, 30), pathType, pathType != true ? "straight" : "diagonal");

            if (GUI.Button(new Rect(10, 400, 120, 30), "Connect all the rooms"))
            {

                currentMenu.working = true;

                var centerPoints = new List<Vector2>();
                var roomDict = new Dictionary<Vector2, List<BasicTile>>();
                foreach (var room in rooms)
                {
                    roomDict.Add(AlgosUtils.FindMiddlePoint(room), room);
                    centerPoints.Add(AlgosUtils.FindMiddlePoint(room));
                }


                

                if (typeOfTri)
                    primEdges = AlgosUtils.PrimAlgoNoDelu(centerPoints);
                else
                    primEdges = AlgosUtils.DelunayTriangulation2D(centerPoints).Item2;


                foreach (var edge in primEdges)
                {

                    //use where so we get soemthing its not the wall but not necessary
                    var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                    var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;


                    var path = AlgosUtils.A_StarPathfinding2DNorm(currentMenu.gridArray2D, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), pathType);


                    foreach (var tile in path.Item1)
                    {
                        if (tile.tileType != BasicTile.TileType.FLOORROOM) 
                            tile.tileType = BasicTile.TileType.FLOORCORRIDOR;

                        tile.tileWeight = 0.75f;
                    }
                }

                currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColShade(currentMenu.gridArray2D, 0, 1, true);


                currentMenu.working = false;
            }

            if (GUI.Button(new Rect(10, 500, 120, 30), "Gen mesh"))
            {


                currentMenu.working = true;

                currentMenu.FormObject(AlgosUtils.MarchingCubesAlgo(AlgosUtils.ExtrapolateMarchingCubes(currentMenu.gridArray2D), false));


                currentMenu.working = false;
            }




            if (GUI.Button(new Rect(10, 440, 120, 30), "check size"))
            {

                currentMenu.working = true;

                foreach (var room in rooms)
                {
                    if(room.Count < minSize) 
                    {
                        foreach (var tile in room)
                        {
                            tile.tileWeight = 0;
                            tile.tileType = BasicTile.TileType.VOID;
                        }
                    }
                }

                currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColShade(currentMenu.gridArray2D, 0, 1, true);

                currentMenu.working = false;

            }



            GUI.Label(new Rect(130, 475, 110, 40), "Min room size: " + minSize);
            minSize = (int)GUI.HorizontalSlider(new Rect(10, 480, 100, 20), minSize, 20, 100);

            if (GUI.Button(new Rect(10, 540, 100, 30), "Go back"))
                currentMenu.ChangeState(0);
        }

    }

    public override void onStart(StateUIManager currentMenu)
    {
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }


   

    public override void onGizmos(StateUIManager currentMenu)
    {

        foreach (var edge in primEdges)
        {
            Debug.DrawLine(new Vector3(edge.edge[0].x, edge.edge[0].y, edge.edge[0].z), new Vector3(edge.edge[1].x, edge.edge[1].y, edge.edge[1].z), Color.green);
        }
    }


    private void DrunkWalkObj2D(StateUIManager currentMenu) 
    {
        if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
        {
            currentMenu.working = true;
            AlgosUtils.RandomWalk2DCol(currentMenu.gridArrayObj2D, iterations, alreadyPassed);    // might need to change this  

            currentMenu.working = false;
        }
    }


    private void DrunkWalk2D(StateUIManager currentMenu)
    {
        if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
        {
            currentMenu.working = true;

            currentMenu.gridArray2D = AlgosUtils.RandomWalk2DCol(iterations, alreadyPassed, currentMenu.width, currentMenu.height, randomStart: startFromMiddle);

            currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D);

            currentMenu.working = false;
        }
    }


}
