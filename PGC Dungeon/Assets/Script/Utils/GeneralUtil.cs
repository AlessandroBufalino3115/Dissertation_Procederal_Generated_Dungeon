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

    public static void Spaces(int spaceNum)
    {
        for (int i = 0; i < spaceNum; i++)
        {
            EditorGUILayout.Space();
        }
    }



    public static Vector3 CubicBeizier(Vector2Int pos1, Vector2Int pos2, Vector2Int pos3, Vector2Int pos4, float t)
    {
        var correctedPos1 = new Vector3(pos1.x, 0, pos1.y);
        var correctedPos2 = new Vector3(pos2.x, 0, pos2.y);
        var correctedPos3 = new Vector3(pos3.x, 0, pos3.y);
        var correctedPos4 = new Vector3(pos4.x, 0, pos4.y);

        return (Mathf.Pow((1 - t), 3) * correctedPos1) + (3 * (Mathf.Pow((1 - t), 2)) * t * correctedPos2) + (3 * (1 - t) * t * t * correctedPos3) + t * t * t * correctedPos4;
    }

    public static Tuple<Vector2, Vector2> ExtrapolatePos(Vector2 startPos, Vector2 EndPos,int margin)
    {
        float lerpPoint2 = Random.Range(0.15f, 0.40f);
        float lerpPoint3 = Random.Range(0.60f, 0.80f);

        margin = Mathf.Abs(margin);


        Vector2 dir = startPos - EndPos;

        var normalised = Vector2.Perpendicular(dir).normalized;
        var point2 = Vector2.Lerp(startPos, EndPos, lerpPoint2);
        point2 = point2 + normalised * Random.Range(margin *-1, margin);


        normalised = Vector2.Perpendicular(dir).normalized;
        var point3 = Vector2.Lerp(startPos, EndPos, lerpPoint3);
        point3 = point3 + normalised * Random.Range(margin * -1, margin);
 

        return Tuple.Create(point2, point3);
    }
}




