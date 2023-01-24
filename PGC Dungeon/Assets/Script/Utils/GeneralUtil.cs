using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GeneralUtil 
{

    public static int[,] childPosArry4Side = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
    public static int[,] childPosArry8Side = { { 0, -1 }, { 1, -1 }, { -1, -1 },  { -1, 0 }, { 1, 0 },    { 0, 1 }, { 1, 1 }, { -1, 1 } };





    /// <summary>
    /// from 0 
    /// </summary>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <returns></returns>
    public static Vector2Int RanVector2Int(int maxX,int maxY) 
    {
        int ranX = Random.Range(0,maxX);
        int ranY = Random.Range(0, maxY);


        return new Vector2Int(ranX, ranY);
    }


    /// <summary>
    /// from 0 
    /// </summary>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    /// <returns></returns>
    public static Vector3Int RanVector3Int(int maxX, int maxY, int maxZ)
    {
        int ranX = Random.Range(0, maxX);
        int ranY = Random.Range(0, maxY);
        int ranZ = Random.Range(0, maxZ);


        return new Vector3Int(ranX, ranY,ranZ);
    }

    public static Vector2 RanVector2Float(float maxX, float maxY)
    {
        float ranX = Random.Range(0f, maxX);
        float ranY = Random.Range(0f, maxY);

        return new Vector2(ranX, ranY);
    }

    public static int PerfTimer(bool start, int startTimer =0) 
    {
        if (start) 
        {
            return Environment.TickCount & Int32.MaxValue;
        }
        else 
        {
            int timerEnd = Environment.TickCount & Int32.MaxValue;

            Debug.Log($"<color=yellow>Performance: This operation took {timerEnd - startTimer} ticks</color>");

            return timerEnd - startTimer;
        }
    }



    public static float EuclideanDistance2D(Vector2 point1, Vector2 point2)
    {
        return MathF.Sqrt(MathF.Pow((point1.x - point2.x), 2) + MathF.Pow((point1.y - point2.y), 2));
    }

    public static float ManhattanDistance2D(Vector2 point1, Vector2 point2)
    {
        return Mathf.Abs((point1.x - point2.x)) + Mathf.Abs((point1.y - point2.y));
    }

    public static void SpacesUILayout(int spaceNum)
    {
        for (int i = 0; i < spaceNum; i++)
        {
            EditorGUILayout.Space();
        }
    }





    /// <summary>
    /// Sets the colour of the pixel that is saved in the class instance
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="gridArray2D"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextSelfCol(BasicTile[][] gridArray2D)
    {

        Texture2D texture = new Texture2D(gridArray2D[0].Length, gridArray2D.Length);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, gridArray2D[y][x].color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();


        return texture;
    }

    /// <summary>
    /// either black or white, if = 0 white if = 1 black
    /// </summary>
    /// <param name="gridArray2D"></param>
    /// <param name="black"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextBiColAnchor(BasicTile[][] gridArray2D, bool black = false)
    {

        Texture2D texture = new Texture2D(gridArray2D[0].Length, gridArray2D.Length);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = new Color();

                if (black)
                {
                    color = ((gridArray2D[y][x].tileWeight) == 0 ? Color.white : Color.black);
                }
                else
                {
                    color = ((gridArray2D[y][x].tileWeight) == 0 ? Color.white : Color.grey);
                }

                texture.SetPixel(x, y, color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();


        return texture;
    }

    /// <summary>
    /// Set the shade of black and white with a given max and min weight then weight
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="gridArray2D"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextBiColShade(BasicTile[][] gridArray2D, float minWeight, float maxWeight, bool inverse = false)
    {

        Texture2D texture = new Texture2D(gridArray2D[0].Length, gridArray2D.Length);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float num = Mathf.InverseLerp(minWeight, maxWeight, gridArray2D[y][x].tileWeight);

                if (inverse)
                    gridArray2D[y][x].color = new Color(1 - num, 1 - num, 1 - num, 1f);
                else
                    gridArray2D[y][x].color = new Color(num, num, num, 1f);


                texture.SetPixel(x, y, gridArray2D[y][x].color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }


}




