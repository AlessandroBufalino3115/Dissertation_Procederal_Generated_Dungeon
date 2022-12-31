using System;
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


    public TileOBJ[][][] gridArray3D = new TileOBJ[1][][];


    public bool working;

    public TileOBJ[][] gridArrayObj2D = new TileOBJ[1][];
    public BasicTile[][] gridArray2D = new BasicTile[1][];

    [SerializeField] GameObject emptyBlock;
    [SerializeField] GameObject CubeBlock;
    [SerializeField] GameObject Plane;

    public int width;
    public int ySize;
    public int height;

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

    /*
    public void Gen3DVolume(int widhtZ, int heightY, int lengthX, bool clearBlock = false, bool scaleToggle = false)
    {

        zSize = widhtZ;
        ySize = heightY;
        xSize = lengthX;



        dimension = Dimension.THREED;
        int timerStart = Environment.TickCount & Int32.MaxValue;
        int blockNum = 0;

        gridArray3D = new TileOBJ[widhtZ][][];

        for (int z = 0; z < gridArray3D.Length; z++)
        {

            gridArray3D[z] = new TileOBJ[heightY][];
            for (int y = 0; y < gridArray3D[z].Length; y++)
            {

                gridArray3D[z][y] = new TileOBJ[lengthX];

                for (int x = 0; x < gridArray3D[z][y].Length; x++)
                {
                    Vector3 position = new Vector3(x, y, z);

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

                    gridArray3D[z][y][x] = new TileOBJ(newRef, x, y, z);
                    gridArray3D[z][y][x].tileType = TileOBJ.TileType.VOID;

                    blockNum++;
                }
            }
        }


        int half_x = (lengthX - 1) / 2;
        int half_y = (heightY - 1) / 2;
        int half_z = (widhtZ - 1) / 2;



        gridArray3D[half_z][half_y][half_x].tileObj.GetComponent<MeshRenderer>().material.color = Color.yellow;


        int timerEnd = Environment.TickCount & Int32.MaxValue;
        int totalTicks = timerEnd - timerStart;
        Debug.Log($"<color=yellow>Performance: The total time this has taken was {totalTicks} Ticks, to generate {blockNum} positions</color>");

    }
    */

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


        plane.transform.localScale = new Vector3(width/4, 1, height/4);

        gridArray2D = new BasicTile[height][];

        for (int y = 0; y < height; y++)
        {
            gridArray2D[y] = new BasicTile[width];

            for (int x = 0; x < width; x++)
            {
                gridArray2D[y][x] = new BasicTile();
                gridArray2D[y][x].position = new Vector3Int(x, 0, y);
            }
        }


        //Texture2D texture = new Texture2D(width, height);


        // given something from 0 to 1 seems to also be faster to just do ti via color array
        //Color[] colourMap = new Color[width * height];
        //for (int y = 0; y < height; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
        //    }
        //}
        //texture.SetPixels(colourMap);
        //texture.Apply();


        //plane.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
        //plane.transform.localScale = new Vector3(width, height);
    }
    public void DestroyAllTiles()
    {


        if (plane != null)
            Destroy(plane);


        dimension = Dimension.NONE;

        int timerStart = Environment.TickCount & Int32.MaxValue;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        gridArray3D = new TileOBJ[1][][];

        gridArrayObj2D = new TileOBJ[1][];

        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;


        Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks}</color>");
    }




}
