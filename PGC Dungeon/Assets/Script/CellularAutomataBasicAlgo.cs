using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

public class CellularAutomataBasicAlgo : MonoBehaviour
{

    public int text_x;
    public int text_y;



    public class CAtiles
    {
        public TileVolumeGenerator.Tile tileCA;

        public bool empty;



        // when empty is true means white unliving
        // when balck means full living

        public CAtiles(float perc, TileVolumeGenerator.Tile normTile)
        {
            var ran = Random.Range(0f, 1f);

            tileCA = normTile;
            if (ran <= perc)
            {
                empty = false;
            }
            else
            {
                empty = true;
            }


            SetState();


        }



        public void SetState()
        {
            if (empty)
            {

                tileCA.arrayTileObj.transform.GetComponent<MeshRenderer>().material.color = Color.white;
            }
            else
            {
                tileCA.arrayTileObj.transform.GetComponent<MeshRenderer>().material.color = Color.black;

            }

        }

    }


    public float fillDelay = 0.2f;


    public static CellularAutomataBasicAlgo instance;

    [Range(0.2f, 0.8f)]
    public float startingPercentageOfTaken;


    [Range(3, 6)]
    public int neighbours_var = 4;
    //public List<CAtiles> caTiles = new List<CAtiles>();

    CAtiles[][][] gridArray3D = new CAtiles[1][][];

    CAtiles[][] gridArray2D = new CAtiles[1][];

    [SerializeField] int length;
    [SerializeField] int width;
    [SerializeField] int height;


    private void Awake()
    {
        instance = this;
    }

    public void Init2DCallAuto(TileVolumeGenerator.Tile[][] _gridArray2D, int _length, int _height )
    {
        int timerStart = Environment.TickCount & Int32.MaxValue;

        length = _length;
        height = _height;


        gridArray2D = new CAtiles[_gridArray2D.Length][];

        for (int y = 0; y < _gridArray2D.Length; y++)
        {
            gridArray2D[y] = new CAtiles[_gridArray2D[y].Length];

            for (int x = 0; x < _gridArray2D[y].Length; x++)
            {
                gridArray2D[y][x] = new CAtiles(startingPercentageOfTaken, _gridArray2D[y][x]);
            }
        }


        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;
        Debug.Log($"<color=yellow>Performance: To run the iteration of CA it took {totalTicks}</color>");


        CalcRatio2D(gridArray2D);

    }

    public void CalcRatio2D(CAtiles[][] _gridArray2D)
    {

        float black = 0;
        float white = 0;



        for (int y = 0; y < _gridArray2D.Length; y++)
        {
            for (int x = 0; x < _gridArray2D[y].Length; x++)
            {
                if (gridArray2D[y][x].empty)
                {
                    white++;
                }
                else
                {
                    black++;
                }
            }
        }



        float truePerc = (black / (black + white)) * 100;


        Debug.Log($"<color=orange>Debug: There are {black} black tiles and {white} white tiles, the true % is {truePerc}</color>");





    }

    public void Drawiteration()
    {
        for (int y = 0; y < gridArray2D.Length; y++)
        {
            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                gridArray2D[y][x].SetState();
            }
        }
    }

    public void RunIterationCelAuto2D() 
    {

        int timerStart = Environment.TickCount & Int32.MaxValue;


        bool[][] copyArrayStorage = new bool[gridArray2D.Length][];

        for (int y = 0; y < gridArray2D.Length; y++)
        {
            copyArrayStorage[y] = new bool[gridArray2D[y].Length];

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                copyArrayStorage[y][x] = gridArray2D[y][x].empty;
            }
        }





        for (int y = 0; y < gridArray2D.Length; y++)
        {

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                int neighbours = 0;



                for (int col_offset = -1; col_offset < 2; col_offset++)
                {
                    for (int row_offset = -1; row_offset < 2; row_offset++)
                    {

                        if (y + col_offset < 0 || x + row_offset < 0 || y + col_offset >= gridArray2D.Length || x + row_offset >= gridArray2D[y].Length)
                        {


                        }
                        else if (col_offset == 0 && row_offset == 0)
                        {

                        }
                        else
                        {
                            //Debug.Log($"needs to go in here {y + col_offset}   and for the x {x + row_offset}");
                            if (!gridArray2D[y + col_offset][x+row_offset].empty)
                            {
                                neighbours++;
                            }
                        }
                    }
                }


                //Debug.Log($"{neighbours}");

                if (neighbours >= neighbours_var)
                {
                    //Debug.Log($"At {y * width + x} ");
                    gridArray2D[y ][x ].empty = false;
                }
                else
                {

                    //Debug.Log($"not at At {y * width + x} ");
                    gridArray2D[y][x].empty = true;
                }
            }
        }

       

        Drawiteration();

        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;
        Debug.Log($"<color=yellow>Performance: To run the iteration of CA it took {totalTicks}</color>");


        CalcRatio2D(gridArray2D);

    }

    public void CallFilling() 
    {

        int ran_x_s = Random.Range(1, TileVolumeGenerator.Instance.x_Length);
        int ran_y_e = Random.Range(1, TileVolumeGenerator.Instance.y_Height);

        StartCoroutine(Flood2D(text_x, text_y));
    }








    private IEnumerator Flood2D(int x, int y) 
    {
        WaitForSeconds wait = new WaitForSeconds(fillDelay);

        if (y >= 0 && x  >= 0 && y  < gridArray2D.Length && x < gridArray2D[y].Length)
        {

            yield return wait;

            if (gridArray2D[y][x].empty && gridArray2D[y][x].tileCA.arrayTileObj.GetComponent<MeshRenderer>().material.color != Color.red) 
            {
                gridArray2D[y][x].tileCA.arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.red;

                StartCoroutine(Flood2D(x + 1, y));
                StartCoroutine(Flood2D(x - 1, y));
                StartCoroutine(Flood2D(x , y+1));
                StartCoroutine(Flood2D(x, y-1));
            }
        }

    }






}
