using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public static class AlgosUtils 
{


    #region A*pathFinding



    /// <summary>
    /// Given a Start and End, run A* pathfinding
    /// </summary>
    /// <param name="tileArray2D"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="perf"></param>
    /// <param name="colorDebug"></param>
    /// <param name="diagonalTiles"></param>
    /// <returns>Item 1 is the path of the AI, Item 2 is the checked tiles</returns>
    public static Tuple<List<AStar_Node>, List<AStar_Node>> A_StarPathfinding2DNorm(Tile[][] tileArray2D, Vector2Int start, Vector2Int end, bool euclideanDis = true, bool perf = false, bool colorDebug = false, bool diagonalTiles = false)
    {
        int timerStart = Environment.TickCount & Int32.MaxValue;

        // here we need a way to turn the whatever given tileset into nodes prob inheritance is best here
        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();


        AStar_Node start_node = new AStar_Node(tileArray2D[start.y][start.x]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[end.y][end.x]);


        int[,] childPosArry = new int[0, 0];

        if (diagonalTiles)
            childPosArry = GeneralUtil.childPosArry8Side;
        else
            childPosArry = GeneralUtil.childPosArry4Side;

        openList.Add(start_node);

        while (openList.Count > 0)
        {

            AStar_Node currNode = openList[0];
            int currIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < currNode.f)
                {
                    currNode = openList[i];
                    currIndex = i;
                }
            }

            openList.RemoveAt(currIndex);

            closedList.Add(currNode);

            if (currNode.refToGameObj.position.x == end_node.refToGameObj.position.x && currNode.refToGameObj.position.z == end_node.refToGameObj.position.z)
            {

                List<AStar_Node> path = new List<AStar_Node>();

                AStar_Node current = currNode;

                while (current.parent != null)
                {
                    path.Add(current);
                    current = current.parent;
                }



                if (colorDebug)
                {
                    tileArray2D[start.y][start.x].tileObj.GetComponent<MeshRenderer>().material.color = Color.red;


                    for (int i = 0; i < path.Count; i++)
                    {
                        if (i == path.Count - 1)
                            path[i].refToGameObj.tileObj.GetComponent<MeshRenderer>().material.color = Color.green;
                        else
                            path[i].refToGameObj.tileObj.GetComponent<MeshRenderer>().material.color = Color.blue;
                    }
                }

                int timerEnd_ = Environment.TickCount & Int32.MaxValue;
                int totalTicks_ = timerEnd_ - timerStart;

                if (perf) { Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks_}</color>"); }


                return new Tuple<List<AStar_Node>, List<AStar_Node>>(path, openList);

            }
            else
            {
                List<AStar_Node> children = new List<AStar_Node>();


                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { currNode.refToGameObj.position.x + x_buff, currNode.refToGameObj.position.z + y_buff };


                    if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= TileVolumeGenerator.Instance.x_Length || node_position[1] >= TileVolumeGenerator.Instance.y_Height)
                    {
                        continue;
                    }
                    else
                    {
                        //here an if statment also saying that walkable 
                        AStar_Node new_node = new AStar_Node(tileArray2D[node_position[1]][node_position[0]]);
                        children.Add(new_node);
                    }
                }

                foreach (var child in children)
                {
                    foreach (var closedListItem in closedList)
                    {
                        if (child.refToGameObj.position.x == closedListItem.refToGameObj.position.x && child.refToGameObj.position.z == closedListItem.refToGameObj.position.z)
                        {
                            continue;
                        }
                    }


                    child.g = currNode.g + 0.5f;

                    if (euclideanDis)
                        child.h = GeneralUtil.EuclideanDistance2D(new Vector2(end_node.refToGameObj.position.x, end_node.refToGameObj.position.z), new Vector2(child.refToGameObj.position.x, child.refToGameObj.position.z));
                    else
                        child.h = GeneralUtil.ManhattanDistance2D(new Vector2(end_node.refToGameObj.position.x, end_node.refToGameObj.position.z), new Vector2(child.refToGameObj.position.x, child.refToGameObj.position.z));

                    child.f = child.g + child.h;
                    child.parent = currNode;


                    foreach (var openListItem in openList)
                    {
                        if (child.refToGameObj.position.x == openListItem.refToGameObj.position.x && child.refToGameObj.position.z == openListItem.refToGameObj.position.z && child.g > openListItem.g)// 
                        {
                            continue;
                        }
                    }

                    openList.Add(child);

                }
            }
        }



        int timerEnd = Environment.TickCount & Int32.MaxValue;
        int totalTicks = timerEnd - timerStart;

        if (perf) { Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks}</color>"); }


        return null;

    }

    public static Tuple<List<AStar_Node>, List<AStar_Node>> A_StarPathfinding2DWeight(Tile[][] tileArray2D, Vector2Int start, Vector2Int end, Dictionary<int, float> rulesDict, bool euclideanDis = true, bool perf = false, bool colorDebug = false, bool diagonalTiles = false)
    {
        int timerStart = Environment.TickCount & Int32.MaxValue;

        // here we need a way to turn the whatever given tileset into nodes prob inheritance is best here
        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();


        AStar_Node start_node = new AStar_Node(tileArray2D[start.y][start.x]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[end.y][end.x]);


        int[,] childPosArry = new int[0, 0];

        if (diagonalTiles)
            childPosArry = GeneralUtil.childPosArry8Side;
        else
            childPosArry = GeneralUtil.childPosArry4Side;

        openList.Add(start_node);

        while (openList.Count > 0)
        {

            AStar_Node currNode = openList[0];
            int currIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < currNode.f)
                {
                    currNode = openList[i];
                    currIndex = i;
                }
            }

            openList.RemoveAt(currIndex);

            closedList.Add(currNode);

            if (currNode.refToGameObj.position.x == end_node.refToGameObj.position.x && currNode.refToGameObj.position.z == end_node.refToGameObj.position.z)
            {

                List<AStar_Node> path = new List<AStar_Node>();

                AStar_Node current = currNode;

                while (current.parent != null)
                {
                    path.Add(current);
                    current = current.parent;
                }



                if (colorDebug)
                {
                    tileArray2D[start.y][start.x].tileObj.GetComponent<MeshRenderer>().material.color = Color.red;


                    for (int i = 0; i < path.Count; i++)
                    {
                        if (i == path.Count - 1)
                            path[i].refToGameObj.tileObj.GetComponent<MeshRenderer>().material.color = Color.green;
                        else
                            path[i].refToGameObj.tileObj.GetComponent<MeshRenderer>().material.color = Color.blue;
                    }
                }

                int timerEnd_ = Environment.TickCount & Int32.MaxValue;
                int totalTicks_ = timerEnd_ - timerStart;

                if (perf) { Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks_}</color>"); }


                return new Tuple<List<AStar_Node>, List<AStar_Node>>(path, openList);

            }
            else
            {
                List<AStar_Node> children = new List<AStar_Node>();


                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { currNode.refToGameObj.position.x + x_buff, currNode.refToGameObj.position.z + y_buff };


                    if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= TileVolumeGenerator.Instance.x_Length || node_position[1] >= TileVolumeGenerator.Instance.y_Height)
                    {
                        continue;
                    }
                    else
                    {
                        //here an if statment also saying that walkable 
                        AStar_Node new_node = new AStar_Node(tileArray2D[node_position[1]][node_position[0]]);
                        children.Add(new_node);
                    }
                }

                foreach (var child in children)
                {
                    foreach (var closedListItem in closedList)
                    {
                        if (child.refToGameObj.position.x == closedListItem.refToGameObj.position.x && child.refToGameObj.position.z == closedListItem.refToGameObj.position.z)
                        {
                            continue;
                        }
                    }


                    float addedValue = 0;
                    int tileType = (int)tileArray2D[child.refToGameObj.position.z][child.refToGameObj.position.x].tileType;

                    if (rulesDict.ContainsKey(tileType))
                        addedValue = rulesDict[tileType];


                    child.g = currNode.g + 0.5f;

                    if (euclideanDis)
                        child.h = GeneralUtil.EuclideanDistance2D(new Vector2(end_node.refToGameObj.position.x, end_node.refToGameObj.position.z), new Vector2(child.refToGameObj.position.x, child.refToGameObj.position.z));
                    else
                        child.h = GeneralUtil.ManhattanDistance2D(new Vector2(end_node.refToGameObj.position.x, end_node.refToGameObj.position.z), new Vector2(child.refToGameObj.position.x, child.refToGameObj.position.z));

                    child.f = child.g + child.h + addedValue;
                    child.parent = currNode;


                    foreach (var openListItem in openList)
                    {
                        if (child.refToGameObj.position.x == openListItem.refToGameObj.position.x && child.refToGameObj.position.z == openListItem.refToGameObj.position.z && child.g > openListItem.g)
                        {
                            continue;
                        }
                    }

                    openList.Add(child);

                }
            }
        }



        int timerEnd = Environment.TickCount & Int32.MaxValue;
        int totalTicks = timerEnd - timerStart;

        if (perf) { Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks}</color>"); }


        return null;

    }

    #endregion




    #region DrunkWalk


    public static void DrunkWalk2DCol(Tile[][] _gridarray2D, int iterations, bool alreadyPassed) 
    {
        int iterationsLeft = iterations;

        Vector2Int currentHead = GeneralUtil.RanVector2Int(_gridarray2D[0].Length, _gridarray2D.Length);


        int maxIter = (int)((_gridarray2D.Length * _gridarray2D[0].Length) * 1.4f);

        int iterCount = 0;

        while (iterationsLeft > 0)
        {

            iterCount++;

            if (iterCount >= maxIter)
            {
                Debug.Log($"<color=red>Safety break point reached for the Drunk Walk Algo</color>");
              
                break;
            }

            //currentHead = new Vector2Int(startX, startY);

            int ranDir = Random.Range(0, 4);

            switch (ranDir)
            {
                case 0:    //for

                    if (currentHead.y + 1 >= _gridarray2D.Length)
                    { }
                    else
                    {
                        currentHead.y++;
                    }

                    break;

                case 1:    //back
                    if (currentHead.y - 1 < 0)
                    { }
                    else
                    {
                        currentHead.y--;
                    }
                    break;

                case 2:    //left
                    if (currentHead.x - 1 < 0)
                    { }
                    else
                    {
                        currentHead.x--;
                    }
                    break;

                case 3:   //rigth
                    if (currentHead.x + 1 >= _gridarray2D[0].Length)
                    { }
                    else
                    {
                        currentHead.x++;
                    }
                    break;

                default:
                    break;
            }


            if (alreadyPassed)
            {
                if (_gridarray2D[(int)currentHead.y][(int)currentHead.x].tileObj.GetComponent<MeshRenderer>().material.color != Color.grey)
                {
                    _gridarray2D[(int)currentHead.y][(int)currentHead.x].tileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
                    iterationsLeft--;
                }
            }
            else
            {
                _gridarray2D[(int)currentHead.y][(int)currentHead.x].tileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
                iterationsLeft--;
            }
        }


    }


    public static void DrunkWalk3DCol(Tile[][][] _gridarray3D, int iterations, bool alreadyPassed ) 
    {
        int iterationsLeft = iterations;

        Vector3 currentHead = GeneralUtil.RanVector3Int(_gridarray3D[0][0].Length, _gridarray3D[0].Length, _gridarray3D.Length);

        int maxIter = (int)((_gridarray3D.Length * _gridarray3D[0].Length * _gridarray3D[0][0].Length) * 1.4f);

        int iterCount = 0;

        while (iterationsLeft > 0)
        {
            iterCount++;

            if (iterCount >= maxIter)
            {
                Debug.Log($"<color=red>Safety break point reached for the Drunk Walk Algo</color>");
                
                break;
            }

            int ranDir = Random.Range(0, 6);

            switch (ranDir)
            {
                case 0:    //for

                    if (currentHead.y + 1 >= _gridarray3D[0].Length)
                    { }
                    else
                    {
                        currentHead.y++;
                    }

                    break;

                case 1:    //back
                    if (currentHead.y - 1 < 0)
                    { }
                    else
                    {
                        currentHead.y--;
                    }
                    break;

                case 2:    //left
                    if (currentHead.x - 1 < 0)
                    { }
                    else
                    {
                        currentHead.x--;
                    }
                    break;

                case 3:   //rigth
                    if (currentHead.x + 1 >= _gridarray3D[0][0].Length)
                    { }
                    else
                    {
                        currentHead.x++;
                    }
                    break;

                case 4:   //top
                    if (currentHead.z + 1 >= _gridarray3D.Length)
                    { }
                    else
                    {
                        currentHead.z++;
                    }
                    break;

                case 5:   //bot
                    if (currentHead.z - 1 < 0)
                    { }
                    else
                    {
                        currentHead.z--;
                    }
                    break;


                default:
                    break;
            }


            if (alreadyPassed)
            {
                if (_gridarray3D[(int)currentHead.z][(int)currentHead.y][(int)currentHead.x].tileObj.GetComponent<MeshRenderer>().material.color != Color.grey)
                {
                    _gridarray3D[(int)currentHead.z][(int)currentHead.y][(int)currentHead.x].tileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
                    iterationsLeft--;
                }
            }
            else
            {
                _gridarray3D[(int)currentHead.z][(int)currentHead.y][(int)currentHead.x].tileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
                iterationsLeft--;
            }
        }
    }

    #endregion





    #region PerlinNoise

    public static float[,] PerlinNoise2DTileSet(Tile[][] _gridArray2D, float scale, int octaves, float persistance, float lacu)
    {
        float[,] noiseMap = new float[_gridArray2D[0].Length, _gridArray2D.Length];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxN = float.MinValue;
        float minN = float.MaxValue;


        for (int y = 0; y < _gridArray2D.Length; y++)
        {
            for (int x = 0; x < _gridArray2D[0].Length; x++)
            {

                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = x / scale * freq;
                    float sampleY = y / scale * freq;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;

                    freq *= lacu;

                }


                if (noiseHeight > maxN) { maxN = noiseHeight; }
                else if (noiseHeight < minN) { minN = noiseHeight; }

                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < _gridArray2D.Length; y++)
        {
            for (int x = 0; x < _gridArray2D[0].Length; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minN, maxN, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }



    public static float[,,] PerlinNoise3DTileSet(Tile[][][] _gridArray3D, float scale)
    {
        float[,,] noiseMap = new float[_gridArray3D[0][0].Length, _gridArray3D[0].Length, _gridArray3D.Length];


        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int z = 0; z < _gridArray3D.Length; z++)
        {
            for (int y = 0; y < _gridArray3D[0].Length; y++)
            {
                for (int x = 0; x < _gridArray3D[0][0].Length; x++)
                {
                    float perlinValue = Perlin3D(z * scale, y * scale, x * scale);


                    noiseMap[x, y, z] = perlinValue;
                }
            }
        }


        return noiseMap;

    }
    public static float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }


    public static void DrawNoiseMap(Renderer meshRenderer, int widthX, int lengthY, float scale, int octaves, float persistance, float lacu)
    {

        var noiseMap = PerlinNoise2DPlane(widthX, lengthY, scale,octaves,persistance,lacu);

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();

      
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(width, 1, height);


    }
    public static float[,] PerlinNoise2DPlane(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacu)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxN = float.MinValue;
        float minN = float.MaxValue;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = x / scale * freq;
                    float sampleY = y / scale * freq;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;

                    freq *= lacu;

                }


                if (noiseHeight > maxN) { maxN = noiseHeight; }
                else if (noiseHeight < minN) { minN = noiseHeight; }

                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minN, maxN, noiseMap[x, y]);
            }
        }




        return noiseMap;
    }














    #endregion

}
