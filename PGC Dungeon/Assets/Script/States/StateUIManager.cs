using System;
using System.Collections.Generic;
using UnityEngine;

public class StateUIManager : MonoBehaviour
{


    //differnet maximas for the plane and obj

    UiBaseState currState;
    public UiBaseState CurrState { get { return CurrState; } }
    public int changingToState;
    private UiBaseState[] statesList = new UiBaseState[8]
    {
       new UiHomeState(),
       new UIRoomBased(),
       new UIVoronoiState(),
       new UIPerlinState(),
       new UIDiamonSquare(),
       new UILSystemState(),
       new UICellularAutomataState(),
       new UIDrunkWalk()
    };

    public Material mat;

    //public TileOBJ[][][] gridArray3D = new TileOBJ[1][][];
    public Dictionary<BasicTile.TileType, float> tileTypeToCostDict = new Dictionary<BasicTile.TileType, float>();

    public bool working;
    public bool gizmo;

    public TileOBJ[][] gridArrayObj2D = new TileOBJ[1][];
    public BasicTile[][] gridArray2D = new BasicTile[1][];

    [SerializeField] GameObject emptyBlock;
    [SerializeField] GameObject CubeBlock;
    [SerializeField] GameObject Plane;

    public int width;
    public int ySize;
    public int height;

    public GameObject wall;
    public GameObject floor;

    public enum Dimension 
    {
        NONE,
        TWOD,
        THREED,
        PLANE
    }

    public Dimension dimension;

    public GameObject plane;


    void Start()
    {
        currState = statesList[0];

        currState.onStart(this);

        var objRef = Instantiate(wall, this.transform);

        objRef.transform.position = new Vector3(0, 0, 0);
        objRef.transform.Rotate(0, 0, 0);
        objRef.transform.name = "0";



         objRef = Instantiate(wall, this.transform);

        objRef.transform.position = new Vector3(0, 0, 0);
        objRef.transform.Rotate(0, 90, 0);
        objRef.transform.name = "90";



         objRef = Instantiate(wall, this.transform);

        objRef.transform.position = new Vector3(0, 0, 0);
        objRef.transform.Rotate(0, 180, 0);
        objRef.transform.name = "180";


         objRef = Instantiate(wall, this.transform);

        objRef.transform.position = new Vector3(0, 0, 0);
        objRef.transform.Rotate(0, 270, 0);
        objRef.transform.name = "270";


    }
    void Update()
    {
        currState.onUpdate(this);
    }
    public void ChangeState(int state)
    {
        changingToState = state;

        currState.onExit(this);
        currState = statesList[state];

        currState.onStart(this);
    }
    private void OnGUI()
    {

        currState.onGUI(this);
    }

    private void OnDrawGizmos()
    {
        if (gizmo)
            currState.onGizmos(this);
    }



    public void Gen2DVolume( int heightY, int lengthX, bool clearBlock = false, bool scaleToggle = false)
    {
        width = lengthX;
        height = heightY;

        dimension = Dimension.TWOD;
        int timerStart = Environment.TickCount & Int32.MaxValue;
        int blockNum = 0;

        gridArrayObj2D = new TileOBJ[heightY][];

        for (int y = 0; y < gridArrayObj2D.Length; y++)
        {
            gridArrayObj2D[y] = new TileOBJ[lengthX];

            for (int x = 0; x < gridArrayObj2D[y].Length; x++)
            {
                Vector3 position = new Vector3(x, 0, y);



                GameObject newRef = null;

                if (clearBlock)
                {
                    newRef = Instantiate(emptyBlock, this.gameObject.transform);
                }
                else
                {
                    newRef = Instantiate(CubeBlock, this.gameObject.transform);
                }

                newRef.transform.position = position;

                if (scaleToggle)
                {
                    newRef.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                }
                newRef.transform.name = x.ToString() + " " + y.ToString();

                gridArrayObj2D[y][x] = new TileOBJ(newRef, x, y);
                gridArrayObj2D[y][x].tileType = TileOBJ.TileType.VOID;

                blockNum++;
            }
        }


        int half_x = (lengthX - 1) / 2;
        int half_y = (heightY - 1) / 2;

        gridArrayObj2D[half_y][half_x].tileObj.GetComponent<MeshRenderer>().material.color = Color.yellow;

        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;
        Debug.Log($"<color=yellow>Performance: The total time this has taken was {totalTicks} Ticks, to generate {blockNum} positions</color>");
        // 1 tick seems to be 1 millisecond
    }



    public void CreatePlane(int width, int height) 
    {
        dimension = Dimension.PLANE;

        plane = Instantiate(Plane, this.transform);


        this.width = width;
        this.height = height;

        plane.transform.Rotate(0, 90, 0);

        plane.transform.localScale = new Vector3(width/4, 1, height/4);

        gridArray2D = new BasicTile[height][];

        for (int y = 0; y < height; y++)
        {
            gridArray2D[y] = new BasicTile[width];

            for (int x = 0; x < width; x++)
            {
                gridArray2D[y][x] = new BasicTile();
                gridArray2D[y][x].position = new Vector2Int(x, y);
                gridArray2D[y][x].tileType = BasicTile.TileType.VOID;
            }
        }
    }
    public void DestroyAllTiles()
    {


        if (plane != null)
            Destroy(plane);


        dimension = Dimension.NONE;

        int timerStart = Environment.TickCount & Int32.MaxValue;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        //gridArray3D = new TileOBJ[1][][];

        gridArrayObj2D = new TileOBJ[1][];

        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;


        Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks}</color>");
    }

    public void FormObject(Mesh mesh)
    {
        GameObject newPart = new GameObject();
        newPart.transform.position = this.transform.position;
        newPart.transform.rotation = this.transform.rotation;
        newPart.transform.localScale = this.transform.localScale;

        var renderer = newPart.AddComponent<MeshRenderer>();
        renderer.material = mat;

        var filter = newPart.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        var collider = newPart.AddComponent<MeshCollider>();
        collider.convex = false;
    }



    public void DrawTileMap()
    {
        int iter = 0;

        for (int z = 0; z < 3; z++)  // this is the heihgt of the room
        {
            for (int y = 0; y < gridArray2D.Length; y++)
            {
                for (int x = 0; x < gridArray2D[0].Length; x++)
                {


                    if (z == 0 || z == 3 - 1) //we draw everything as this is the ceiling and the floor
                    {
                        if (gridArray2D[y][x].tileType != BasicTile.TileType.VOID)
                        {
                            var objRef = Instantiate(floor, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            iter++;
                        }
                    }
                    

                    if (gridArray2D[y][x].tileType == BasicTile.TileType.WALL)
                    {
                        var checkVector = new Vector2Int(x, y);

                        checkVector = new Vector2Int(x + 1, y);// riht check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(wall, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 90, 0);

                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(wall, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 90, 0);

                                iter++;
                            }
                        }



                        checkVector = new Vector2Int(x - 1, y);// left check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(wall, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 270, 0);

                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(wall, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 270, 0);

                                iter++;

                            }
                        }




                        checkVector = new Vector2Int(x, y + 1);// above check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(wall, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.name = "0";

                            objRef.transform.Rotate(0, 0, 0);
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(wall, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.name = "0";

                                objRef.transform.Rotate(0, 0, 0);

                                iter++;
                            }
                        }





                        checkVector = new Vector2Int(x, y - 1);// down check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(wall, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 180, 0);
                            objRef.transform.name = "180";
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(wall, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 180, 0);
                                objRef.transform.name = "180";


                                iter++;
                            }
                        }

                    }






                }
            }
        }

        Debug.Log($"{iter}");

    }

}
