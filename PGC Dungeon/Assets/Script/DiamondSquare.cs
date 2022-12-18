using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class DiamondSquare : MonoBehaviour
{

    public static DiamondSquare instance;

    // TO DLETE
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }








    public void RunDiamondSquare() 
    {


        /*

        int maxHeight = 8;
        int minHeight = -8;

        int roughness = 2;

        // get the size
        var mapSize = TileVolumeGenerator.Instance.y_Height;

        // start the grid
        float[,] grid2D = new float[mapSize, mapSize];

        //need to check for 2n + 1
        if (TileVolumeGenerator.Instance.y_Height != TileVolumeGenerator.Instance.x_Length || TileVolumeGenerator.Instance.x_Length % 2 == 0)
        {
            GeneralUitlInstance.instance.SpawnMessagePrefab("This size is not good soz", false);
            return;
        }
        else
        {
 
            //set the 4 random corners
            grid2D[0, 0] = Ran(minHeight, maxHeight);   // top left
            grid2D[mapSize -1, mapSize -1] = Ran(minHeight, maxHeight);    // bot right
            grid2D[0, mapSize -1 ] = Ran(minHeight, maxHeight); // top right
            grid2D[mapSize - 1, 0] = Ran(minHeight, maxHeight); // bot left

            var chunkSize = mapSize - 1;  //size of square in current iter of algo

            int iter = 0;

            while (chunkSize > 1)
            {
               

                if (iter >= 8000) 
                {
                    break;
                }
                iter++;



                int halfChunk = chunkSize / 2;



                //square step

                for (int y = 0; y < mapSize - 1; y = y + chunkSize)
                {
                    for (int x = 0; x < mapSize - 1; x = x + chunkSize)
                    {
                        grid2D[y + halfChunk, x + halfChunk] = (grid2D[y, x] + grid2D[y, x + chunkSize] + grid2D[y + chunkSize, x] + grid2D[y + chunkSize, x + chunkSize]) / 4 + Ran(-roughness, roughness);
                    }
                }



                //diamond step

                for (int y = 0; y < mapSize; y = y + halfChunk)
                {
                    for (int x = (y + halfChunk) % chunkSize; x < mapSize; x = x + chunkSize)
                    {
                        grid2D[y, x] =
                            (grid2D[(y - halfChunk + mapSize) % mapSize, x] +
                                  grid2D[(y + halfChunk) % mapSize, x] +
                                  grid2D[y, (x + halfChunk) % mapSize] +
                                  grid2D[y, (x - halfChunk + mapSize) % mapSize]) / 4 + Ran(-roughness, roughness);
                    }
                }



                chunkSize = chunkSize / 2;
                //roughness = roughness / 2;
            }
        }
        */
        var grid2D = AlgosUtils.DiamondSquare(16, -16, 2,TileVolumeGenerator.Instance.gridArray2D);

        
        for (int y = 0; y < TileVolumeGenerator.Instance.y_Height; y++)
        {
            for (int x = 0; x < TileVolumeGenerator.Instance.x_Length; x++)
            {
                //the blacker the more minus

                //if (grid2D[y, x] > maxHeight / 2)
                //{

                //    TileVolumeGenerator.Instance.gridArray2D[y][x].tileObj.GetComponent<MeshRenderer>().material.color = nonCalcColour(maxHeight, true);
                //}
                //else
                //{

                //    TileVolumeGenerator.Instance.gridArray2D[y][x].tileObj.GetComponent<MeshRenderer>().material.color = nonCalcColour(maxHeight, false);

                //}

                TileVolumeGenerator.Instance.gridArray2D[y][x].tileObj.GetComponent<MeshRenderer>().material.color = CalcColour(8, grid2D[y, x]);

            }
        }
    }



    private Color CalcColour(int maxHeight, float val)
    {
        float value = Mathf.InverseLerp(-maxHeight, maxHeight, val);

        //floats
        return new Color(value, value, value);

    }


    private Color nonCalcColour(int maxHeight, bool val)
    {
        if (val)
        {
            return new Color(1, 1, 1);
        }
        else
        {

            //floats
            return new Color(0, 0, 0);
        }

    }




    private float Ran(int min,int max) 
    {

        return Random.Range(min,max);
    }


}
