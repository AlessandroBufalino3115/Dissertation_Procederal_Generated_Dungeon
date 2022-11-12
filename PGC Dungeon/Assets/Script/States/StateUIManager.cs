using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class StateUIManager : MonoBehaviour
{

    UiBaseState currState;
    public UiBaseState CurrState { get { return CurrState; } }
    public int changingToState;
    private UiBaseState[] statesList = new UiBaseState[7]
    {
       new UiHomeState(),
       new UIRoomBased(),
       new UIVoronoiState(),
       new UIPerlinState(),
       new UIDiamonSquare(),
       new UILSystemState(),
       new UICellularAutomataState()
    };


    public Tile[][][] gridArray3D = new Tile[1][][];
    public Tile[][] gridArray2D = new Tile[1][];

    [SerializeField] GameObject emptyBlock;
    [SerializeField] GameObject CubeBlock;


    public enum Dimension 
    {
        NONE,
        TWOD,
        THREED,
        NOT_A
    }

    public Dimension dimension;


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


    public void Gen3DVolume(int widhtZ, int heightY, int lengthX, bool clearBlock = false, bool scaleToggle = false)
    {
        dimension = Dimension.THREED;
        int timerStart = Environment.TickCount & Int32.MaxValue;
        int blockNum = 0;

        gridArray3D = new Tile[widhtZ][][];

        for (int z = 0; z < gridArray3D.Length; z++)
        {

            gridArray3D[z] = new Tile[heightY][];
            for (int y = 0; y < gridArray3D[z].Length; y++)
            {

                gridArray3D[z][y] = new Tile[lengthX];

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

                    gridArray3D[z][y][x] = new Tile(newRef, x, y, z);
                    gridArray3D[z][y][x].tileType = Tile.TileType.VOID;

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

    public void Gen2DVolume( int heightY, int lengthX, bool clearBlock = false, bool scaleToggle = false)
    {
        dimension = Dimension.TWOD;
        int timerStart = Environment.TickCount & Int32.MaxValue;
        int blockNum = 0;

        gridArray2D = new Tile[heightY][];

        for (int y = 0; y < gridArray2D.Length; y++)
        {
            gridArray2D[y] = new Tile[lengthX];

            for (int x = 0; x < gridArray2D[y].Length; x++)
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

                gridArray2D[y][x] = new Tile(newRef, x, y);
                gridArray2D[y][x].tileType = Tile.TileType.VOID;

                blockNum++;
            }
        }


        int half_x = (lengthX - 1) / 2;
        int half_y = (heightY - 1) / 2;

        gridArray2D[half_y][half_x].tileObj.GetComponent<MeshRenderer>().material.color = Color.yellow;

        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;
        Debug.Log($"<color=yellow>Performance: The total time this has taken was {totalTicks} Ticks, to generate {blockNum} positions</color>");
        // 1 tick seems to be 1 millisecond
    }

    public void DestroyAllTiles()
    {
        dimension = Dimension.NONE;

        int timerStart = Environment.TickCount & Int32.MaxValue;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        gridArray3D = new Tile[1][][];

        gridArray2D = new Tile[1][];

        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;


        Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks}</color>");
    }




}
