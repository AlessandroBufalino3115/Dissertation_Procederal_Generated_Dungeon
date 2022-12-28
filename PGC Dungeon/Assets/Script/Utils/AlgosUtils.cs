using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using System.Drawing;
using Color = UnityEngine.Color;




//new standard for tiles, 
/*
 * 
 * when gicing an array to a function it has to be of class tile which is just a float arr float arr   everything else can inherit from that with a float arr we can say if bigger than 0.8 just to be sure for any issue its taken
 * 
 * 
 */



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
    public static Tuple<List<AStar_Node>, List<AStar_Node>> A_StarPathfinding2DNorm(TileOBJ[][] tileArray2D, Vector2Int start, Vector2Int end, bool euclideanDis = true, bool perf = false, bool colorDebug = false, bool diagonalTiles = false)
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

    public static Tuple<List<AStar_Node>, List<AStar_Node>> A_StarPathfinding2DWeight(TileOBJ[][] tileArray2D, Vector2Int start, Vector2Int end, Dictionary<int, float> rulesDict, bool euclideanDis = true, bool perf = false, bool colorDebug = false, bool diagonalTiles = false)
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

    #region Random Walk

    // new addition on basic tile 
    public static BasicTile[][] RandomWalk2DCol(int iterations, bool alreadyPassed, int maxX, int maxY, float maxIterMultiplier = 1.4f)
    {
        int iterationsLeft = iterations;


        BasicTile[][] _gridarray2D = new BasicTile[maxY][];

        for (int y = 0; y < maxY; y++)
        {
            _gridarray2D[y] = new BasicTile[maxX];

            for (int x = 0; x < maxX; x++)
            {
                _gridarray2D[y][x] = new BasicTile();
                _gridarray2D[y][x].position = new Vector3Int(x, 0, y);
            }
        }


        Vector2Int currentHead = GeneralUtil.RanVector2Int(_gridarray2D[0].Length, _gridarray2D.Length);

        int maxIter = (int)((_gridarray2D.Length * _gridarray2D[0].Length) * maxIterMultiplier);

        int iterCount = 0;

        while (iterationsLeft > 0)
        {

            iterCount++;

            if (iterCount >= maxIter)
            {
                Debug.Log($"<color=red>Safety break point reached for the Drunk Walk Algo</color>");

                break;
            }

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
                if (_gridarray2D[(int)currentHead.y][(int)currentHead.x].tileWeight != 1)
                {
                    _gridarray2D[(int)currentHead.y][(int)currentHead.x].tileWeight = 1;
                    iterationsLeft--;
                }
            }
            else
            {
                _gridarray2D[(int)currentHead.y][(int)currentHead.x].tileWeight = 1;
                iterationsLeft--;
            }
        }

        return _gridarray2D;

    }

    //also ask if it wants to start from the middle or no
    public static void RandomWalk2DCol(TileOBJ[][] _gridarray2D, int iterations, bool alreadyPassed)
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
    /*
    public static void RandomWalk3DCol(TileOBJ[][][] _gridarray3D, int iterations, bool alreadyPassed ) 
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
    */


    #endregion

    #region PerlinNoise

    public static TileOBJ[][] PerlinNoise2DTileSet(TileOBJ[][] _gridArray2D, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float threashold = 0)
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

                    float sampleX = x / scale * freq + offsetX;
                    float sampleY = y / scale * freq + offsetY;

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
                _gridArray2D[y][x].tileWeight = Mathf.InverseLerp(minN, maxN, noiseMap[x, y]);

                if (threashold != 0)
                {
                    if (threashold > _gridArray2D[y][x].tileWeight)
                        _gridArray2D[y][x].tileWeight=1;
                    else
                        _gridArray2D[y][x].tileWeight=0;
                }
               
            }
        }

        return _gridArray2D;
    }


    /*
    public static float[,,] PerlinNoise3DTileSet(TileOBJ[][][] _gridArray3D, float scale)
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
    */



    public static BasicTile[][] PerlinNoise2D(BasicTile[][] _gridArray2D, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float threashold = 0)
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

                    float sampleX = x / scale * freq + offsetX;
                    float sampleY = y / scale * freq + offsetY;

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
                _gridArray2D[y][x].tileWeight = Mathf.InverseLerp(minN, maxN, noiseMap[x, y]);

                if (threashold != 0)
                {
                    if (threashold > _gridArray2D[y][x].tileWeight)
                        _gridArray2D[y][x].tileWeight = 1;
                    else
                        _gridArray2D[y][x].tileWeight = 0;
                }
            }
        }

        return _gridArray2D;
    }






    public static void DrawNoiseMap(Renderer meshRenderer, int widthX, int lengthY, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float threshold, bool threshBool)
    {

        var noiseMap = PerlinNoise2DPlane(widthX, lengthY, scale, octaves, persistance, lacu, offsetX, offsetY);

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (threshBool)
                {
                    if (threshold > noiseMap[x, y])
                        colourMap[y * width + x] = Color.white;
                    else
                        colourMap[y * width + x] = Color.black;
                }
                else
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();


        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(width, 1, height);


    }

    public static float[,] PerlinNoise2DPlane(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY)
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

                    float sampleX = x / scale * freq + offsetX;
                    float sampleY = y / scale * freq + offsetY;

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

    #region Triangulation

    public static List<Edge> PrimAlgoNoDelu(List<Vector3> points)
    {
        var triangulation = DelunayTriangulation2D(points);

        return PrimAlgo(points, triangulation);

    }

    public static List<Edge> PrimAlgo(List<Vector3> points, List<Triangle> triangulation)
    {
        List<Edge> primsAlgo = new List<Edge>();

        HashSet<Vector3> visitedVertices = new HashSet<Vector3>();

        var ran = Random.Range(0, points.Count);
        var vertex = points[ran];

        visitedVertices.Add(vertex);

        while (visitedVertices.Count != points.Count)
        {

            HashSet<Edge> edgesWithPoint = new HashSet<Edge>();

            foreach (var trig in triangulation)    // we get all the edges
            {
                foreach (var edge in trig.edges)
                {
                    foreach (var point in visitedVertices)
                    {
                        if (visitedVertices.Contains(edge.edge[0]) && visitedVertices.Contains(edge.edge[0]))
                        {
                            // do nothing
                        }
                        else if (visitedVertices.Contains(edge.edge[0]))
                        {
                            edgesWithPoint.Add(edge);
                        }
                        else if (visitedVertices.Contains(edge.edge[1]))
                        {
                            edgesWithPoint.Add(edge);
                        }
                    }
                }
            }

            var edgesWithPointSort = edgesWithPoint.OrderBy(c => c.length).ToArray();   // we sort all the edges by the smallest to biggest

            visitedVertices.Add(edgesWithPointSort[0].edge[0]);
            visitedVertices.Add(edgesWithPointSort[0].edge[1]);
            primsAlgo.Add(edgesWithPointSort[0]);
        }

        return primsAlgo;
    }

    public static List<Triangle> DelunayTriangulation2D(List<Vector3> points)
    {
        var triangulation = new List<Triangle>();

        Vector3 superTriangleA = new Vector3(10000, 10000, 10000);
        Vector3 superTriangleB = new Vector3(10000, 0, 10000);
        Vector3 superTriangleC = new Vector3(0, 10000, 10000);

        triangulation.Add(new Triangle(superTriangleA, superTriangleB, superTriangleC));

        foreach (Vector3 point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (Triangle triangle in triangulation)
            {
                if (IspointInCircumcircle(triangle.a, triangle.b, triangle.c, point))
                {
                    badTriangles.Add(triangle);
                }
            }

            List<Edge> polygon = new List<Edge>();

            foreach (Triangle triangle in badTriangles)
            {
                foreach (Edge triangleEdge in triangle.edges)
                {
                    bool isShared = false;

                    foreach (Triangle otherTri in badTriangles)
                    {
                        if (otherTri == triangle) { continue; }

                        foreach (Edge otherEdge in otherTri.edges)
                        {
                            if (LineIsEqual(triangleEdge, otherEdge))
                            {
                                isShared = true;
                            }
                        }
                    }

                    if (isShared == false)
                    {
                        polygon.Add(triangleEdge);
                    }

                }
            }

            foreach (Triangle badTriangle in badTriangles)
            {
                triangulation.Remove(badTriangle);   // i think this is the issue here
            }

            foreach (Edge edge in polygon)
            {
                Triangle newTriangle = new Triangle(edge.edge[0], edge.edge[1], point);
                triangulation.Add(newTriangle);
            }
        }

        for (int i = triangulation.Count - 1; i >= 0; i--)
        {
            if (triangulation[i].HasVertex(superTriangleA) || triangulation[i].HasVertex(superTriangleB) || triangulation[i].HasVertex(superTriangleC))
            {
                triangulation.Remove(triangulation[i]);
            }
        }

        return triangulation;

    }

    public static bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }

    public static bool IspointInCircumcircle(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {


        float ax_ = A[0] - D[0];
        float ay_ = A[1] - D[1];
        float bx_ = B[0] - D[0];
        float by_ = B[1] - D[1];
        float cx_ = C[0] - D[0];
        float cy_ = C[1] - D[1];



        if ((
            (ax_ * ax_ + ay_ * ay_) * (bx_ * cy_ - cx_ * by_) -
            (bx_ * bx_ + by_ * by_) * (ax_ * cy_ - cx_ * ay_) +
            (cx_ * cx_ + cy_ * cy_) * (ax_ * by_ - bx_ * ay_)
        ) < 0)
        {
            return true;
        }

        else { return false; }

    }

    #endregion

    #region Binary Partition System

    public static List<BoundsInt> BSPAlgo(BoundsInt toSplit, int minHeight, int minWidth)
    {
        var startTimer = GeneralUtil.PerfTimer(true);

        List<BoundsInt> roomList = new List<BoundsInt>();
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

        roomsQueue.Enqueue(toSplit);   // enque add to que
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();   // take out and split this

            // this room can either contain a room or split  room
            if (room.size.y >= minHeight && room.size.x >= minWidth)   // all rooms should at least be big enough
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2 + 1)
                    {
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else if (room.size.x >= minWidth * 2 + 1)
                    {
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else
                    {
                        roomList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2 + 1)
                    {
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else if (room.size.y >= minHeight * 2 + 1)
                    {
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else
                    {
                        roomList.Add(room);
                    }
                }
            }
        }

        var endTimer = GeneralUtil.PerfTimer(false, startTimer);
        return roomList;
    }

    private static void SplitVert(int minWidth, BoundsInt room, Queue<BoundsInt> roomQue)
    {

        int minX = room.min.x;
        int maxX = room.max.x;

        int adjustedMinX = minX + minWidth;
        int adjustedMaxX = maxX - minWidth;

        var ranPosition = Random.Range(adjustedMinX, adjustedMaxX);

        BoundsInt roomLeft = new BoundsInt();

        roomLeft.min = new Vector3Int(room.min.x, room.min.y, 0);
        roomLeft.max = new Vector3Int(ranPosition, room.max.y, 0);

        BoundsInt roomRight = new BoundsInt();

        roomRight.min = new Vector3Int(ranPosition, room.min.y, 0);
        roomRight.max = new Vector3Int(room.max.x, room.max.y, 0);

        roomQue.Enqueue(roomRight);
        roomQue.Enqueue(roomLeft);
    }

    private static void SplitHori(int minHeight, BoundsInt room, Queue<BoundsInt> roomQue)
    {
        int minY = room.min.y;
        int maxY = room.max.y;

        int adjustedMinY = minY + minHeight;
        int adjustedMaxY = maxY - minHeight;

        var ranPosition = Random.Range(adjustedMinY, adjustedMaxY);

        BoundsInt roomTop = new BoundsInt();

        roomTop.min = new Vector3Int(room.min.x, ranPosition, 0);
        roomTop.max = new Vector3Int(room.max.x, room.max.y, 0);

        BoundsInt roomBot = new BoundsInt();

        roomBot.min = new Vector3Int(room.min.x, room.min.y, 0);
        roomBot.max = new Vector3Int(room.max.x, ranPosition, 0);

        roomQue.Enqueue(roomBot);
        roomQue.Enqueue(roomTop);
    }

    #endregion

    #region Flood Fill

    public static void ResetVisited(BasicTile[][] gridArray2D) 
    {
        for (int y = 0; y < gridArray2D.Length; y++)
        {
            for (int x = 0; x < gridArray2D[0].Length; x++)
            {
                gridArray2D[y][x].visited = false;
            }
        }
    }


    /// <summary>
    /// for now this is looking only for the white or wieght = 0 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="list"></param>
    /// <param name="gridArray2D"></param>
    /// <returns>the returned list holds the coords of everything that was true</returns>
    public static List<Vector2> Flood2DText(int x, int y, List<Vector2> list, BasicTile[][] gridArray2D)
    {
        
        if (y >= 0 && x >= 0 && y < gridArray2D.Length && x < gridArray2D[y].Length)
        {
            if (gridArray2D[y][x].tileWeight == 0 && gridArray2D[y][x].visited == false)
            {
                list.Add(new Vector2(x, y));
                gridArray2D[y][x].visited=true;

                Flood2DText(x + 1, y, list, gridArray2D);
                Flood2DText(x - 1, y, list, gridArray2D);
                Flood2DText(x, y + 1, list, gridArray2D);
                Flood2DText(x, y - 1, list, gridArray2D);
            }
        }

        return list;

    }


    #endregion

    #region Cellular Automata



    /// <summary>
    /// mainly used for CA given a % fill up with wieght 1
    /// </summary>
    /// <param name="gridArr"></param>
    /// <param name="ranValue"></param>
    public static void SpawnRandomPointsObj2D(BasicTile[][] gridArr, float ranValue) 
    {
        for (int y = 0; y < gridArr.Length; y++)
        {
            for (int x = 0; x < gridArr[0].Length; x++)
            {
                if (Random.value > ranValue) 
                {
                    gridArr[y][x].tileWeight = 0;

                }
                else 
                {
                    gridArr[y][x].tileWeight = 1;
                }
            }
        }
    }


    public static void RunCaIteration2D(BasicTile[][] gridArray2D, int neighboursNeeded) 
    {

        float[][] copyArrayStorage = new float[gridArray2D.Length][];

        for (int y = 0; y < gridArray2D.Length; y++)
        {
            copyArrayStorage[y] = new float[gridArray2D[y].Length];

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                copyArrayStorage[y][x] = gridArray2D[y][x].tileWeight;
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

                        if (y + col_offset < 0 || x + row_offset < 0 || y + col_offset >= gridArray2D.Length - 1 || x + row_offset >= gridArray2D[y].Length - 1)
                        {

                        }
                        else if (col_offset == 0 && row_offset == 0)
                        {

                        }
                        else
                        {
                            // this was !
                            if (copyArrayStorage[y + col_offset][x + row_offset] == 1)
                            {
                                neighbours++;
                            }
                        }
                    }
                }

                if (neighbours >= neighboursNeeded)
                {   //empty is = false therefore weight is there
                    gridArray2D[y][x].tileWeight = 1;
                }
                else
                {   //true
                    gridArray2D[y][x].tileWeight = 0;
                }
            }
        }
    }

    public static void CleanUp2dCA(BasicTile[][] gridArray2D, int neighboursNeeded)
    {

        float[][] copyArrayStorage = new float[gridArray2D.Length][];

        for (int y = 0; y < gridArray2D.Length; y++)
        {
            copyArrayStorage[y] = new float[gridArray2D[y].Length];

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                copyArrayStorage[y][x] = gridArray2D[y][x].tileWeight;
            }
        }



        for (int y = 0; y < gridArray2D.Length; y++)
        {

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                int neighbours = 0;
                if (copyArrayStorage[y][x] == 1)
                {
                    for (int col_offset = -1; col_offset < 2; col_offset++)
                    {
                        for (int row_offset = -1; row_offset < 2; row_offset++)
                        {

                            if (y + col_offset < 0 || x + row_offset < 0 || y + col_offset >= gridArray2D.Length - 1 || x + row_offset >= gridArray2D[y].Length - 1)
                            {

                            }
                            else if (col_offset == 0 && row_offset == 0)
                            {

                            }
                            else
                            {
                                // this was !
                                if (copyArrayStorage[y + col_offset][x + row_offset] == 1)
                                {
                                    neighbours++;
                                }
                            }
                        }
                    }

                    if (neighbours >= neighboursNeeded)
                    {   //empty is = false therefore weight is there
                        gridArray2D[y][x].tileWeight = 1;
                    }
                    else
                    {   //true
                        gridArray2D[y][x].tileWeight = 0;
                    }
                }
                
            }
        }
    }





    #endregion

    #region DiamondSquare algo

    public static void DiamondSquare(int maxHeight, int minHeight, float roughness, BasicTile[][] gridArr)
    {

        int timerStart = GeneralUtil.PerfTimer(true);

        // get the size
        var mapSize = gridArr.Length;

        // start the grid
        float[,] grid2D = new float[mapSize, mapSize];

        //need to check for 2n + 1
        if (gridArr.Length != gridArr[0].Length || gridArr[0].Length % 2 == 0)
        {
            GeneralUitlInstance.instance.SpawnMessagePrefab("This size is not good soz", false);
            return;
        }
        else
        {

            //set the 4 random corners
            grid2D[0, 0] = Random.Range(minHeight, maxHeight);   // top left
            grid2D[mapSize - 1, mapSize - 1] = Random.Range(minHeight, maxHeight);    // bot right
            grid2D[0, mapSize - 1] = Random.Range(minHeight, maxHeight); // top right
            grid2D[mapSize - 1, 0] = Random.Range(minHeight, maxHeight); // bot left

            var chunkSize = mapSize - 1;  //size of square in current iter of algo

            while (chunkSize > 1)
            {

                int halfChunk = chunkSize / 2;

                for (int y = 0; y < mapSize - 1; y = y + chunkSize)
                {
                    for (int x = 0; x < mapSize - 1; x = x + chunkSize)
                    {
                        grid2D[y + halfChunk, x + halfChunk] = (grid2D[y, x] + grid2D[y, x + chunkSize] + grid2D[y + chunkSize, x] + grid2D[y + chunkSize, x + chunkSize]) / 4 + Random.Range(-roughness, roughness);
                    }
                }

                for (int y = 0; y < mapSize; y = y + halfChunk)
                {
                    for (int x = (y + halfChunk) % chunkSize; x < mapSize; x = x + chunkSize)
                    {
                        grid2D[y, x] =
                            (grid2D[(y - halfChunk + mapSize) % mapSize, x] +
                                  grid2D[(y + halfChunk) % mapSize, x] +
                                  grid2D[y, (x + halfChunk) % mapSize] +
                                  grid2D[y, (x - halfChunk + mapSize) % mapSize]) / 4 + Random.Range(-roughness, roughness);
                    }
                }

                chunkSize = chunkSize / 2;
                roughness = roughness / 2;
            }
        }




        for (int y = 0; y < gridArr.Length; y++)
        {
            for (int x = 0; x < gridArr[0].Length; x++)
            {
                gridArr[y][x].tileWeight = grid2D[y,x];
            }
        }


        var end = GeneralUtil.PerfTimer(false, timerStart);


    }


    #endregion

    #region Voronoi


    public static BasicTile[][] Voronoi2D(BasicTile[][] gridArray2D, List<Vector3> pointsArr)
    {
     



        for (int y = 0; y < gridArray2D.Length; y++)
        {
            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                int closestIndex = 0;
                float closestDistance = -1;

                for (int i = 0; i < pointsArr.Count; i++)
                {
                    if (closestDistance < 0)
                    {
                        closestDistance = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArray2D[y][x].position.x, gridArray2D[y][x].position.z));
                    }
                    else
                    {
                        float newDist = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArray2D[y][x].position.x, gridArray2D[y][x].position.z));

                        if (closestDistance > newDist)
                        {
                            closestDistance = newDist;
                            closestIndex = i;
                        }
                    }
                }

                gridArray2D[y][x].idx = closestIndex;

            }
        }

        return gridArray2D;
    }

    public static TileOBJ[][] VoronoiObj2D(TileOBJ[][] gridArrayObj2D, List<Vector3> pointsArr)
    {

        for (int y = 0; y < gridArrayObj2D.Length; y++)
        {
            for (int x = 0; x < gridArrayObj2D[y].Length; x++)
            {
                int closestIndex = 0;
                float closestDistance = -1;

                for (int i = 0; i < pointsArr.Count; i++)
                {
                    if (closestDistance < 0)  //therefore minus therefoe we just started
                    {
                        closestDistance = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArrayObj2D[y][x].tileObj.transform.position.x, gridArrayObj2D[y][x].tileObj.transform.position.z));
                    }
                    else
                    {
                        float newDist = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArrayObj2D[y][x].tileObj.transform.position.x, gridArrayObj2D[y][x].tileObj.transform.position.z));

                        if (closestDistance > newDist)
                        {
                            closestDistance = newDist;
                            closestIndex = i;

                        }
                    }
                }
                gridArrayObj2D[y][x].idx = closestIndex;

            }
        }

        return gridArrayObj2D;
    }


    #endregion

    #region Random Util
    // the widht and height should be relative to the size of thearra not a given input
    /// <summary>
    /// Set the shade of black and white with a given max and min weight then weight
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="gridArray2D"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextBiColShade(BasicTile[][] gridArray2D, float minWeight, float maxWeight)
    {

        Texture2D texture = new Texture2D(gridArray2D[0].Length, gridArray2D.Length);
        //plane.GetComponent<Renderer>().material.mainTexture = texture;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float num = Mathf.InverseLerp(minWeight, maxWeight, gridArray2D[y][x].tileWeight);
                gridArray2D[y][x].color = new Color(num, num, num, 1f);


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
    /// Sets the colour of the pixel that is saved in the class instance
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="gridArray2D"></param>
    /// <returns></returns>
    public static Texture2D SetUpTextSelfCol( BasicTile[][] gridArray2D)
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



    public static void SetColorAllObjAnchor(TileOBJ[][] gridArr) 
    {
        for (int y = 0; y < gridArr.Length; y++)
        {
            for (int x = 0; x < gridArr[0].Length; x++)
            {
                gridArr[y][x].SetColorBiAncor();
            }
        }
    }


    public static void SetColorAllObjBi(TileOBJ[][] gridArr, float minWeight, float maxWeight)
    {
        for (int y = 0; y < gridArr.Length; y++)
        {
            for (int x = 0; x < gridArr[0].Length; x++)
            {
                gridArr[y][x].SetColorBi(minWeight, maxWeight);
            }
        }
    }


    #endregion

}



/// <summary>
/// This is the base tile which hopefully everything inherits from
/// </summary>
public class BasicTile
{

    public Color32 color;
    public Vector3Int position = Vector3Int.zero;
    public float tileWeight;
    public float cost = 0;
    public int idx = 0;
    public bool visited = false;

    public enum TileType
    {
        VOID,
        FLOORROOM,
        WALL,
        ROOF,
        FLOORCORRIDOR,
        AVOID
    }

    public TileType tileType = 0;
  
}







#region triangulation

public class Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public Edge[] edges = new Edge[3];

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;


        this.edges[0] = new Edge(a, b);
        this.edges[1] = new Edge(b, c);
        this.edges[2] = new Edge(c, a);
    }


    public bool HasVertex(Vector3 point)
    {
        if (a == point || b == point || c == point) { return true; }
        else { return false; }
    }

}

public class Edge
{
    public Vector3[] edge = new Vector3[2];
    public float length;
    public Edge(Vector3 a, Vector3 b)
    {
        edge[0] = a;
        edge[1] = b;

        length = Mathf.Abs(Vector3.Distance(a, b));
    }

}


#endregion




/// <summary>
/// this class inherits from the basic Tile class and the only thing it has more is that it contains an object in 3D space
/// </summary>
public class TileOBJ : BasicTile
{
    public GameObject tileObj;

    public TileOBJ(GameObject _arrayTileObj, Vector2Int _pos)
    {
        tileObj = _arrayTileObj;
        position = new Vector3Int(_pos.x, 0, _pos.y);
    }
    public TileOBJ(GameObject _arrayTileObj, int _x, int _y, int _z)
    {
        tileObj = _arrayTileObj;
        position = new Vector3Int(_x, _y, _z);
    }
    public TileOBJ(GameObject _arrayTileObj, int _x, int _y)
    {
        tileObj = _arrayTileObj;
        position = new Vector3Int(_x, 0, _y);
    }

 
    public void SetColorBi(float minWeight, float maxWeight) 
    {
        float num = Mathf.InverseLerp(minWeight, maxWeight, tileWeight);
        color = new Color(num, num, num,1f);

        tileObj.GetComponent<MeshRenderer>().material.color = color;
    }

    public void SetColor(Color _color) 
    {
        this.color = _color;
        tileObj.GetComponent<MeshRenderer>().material.color = _color;
    }


    public void SetColorBiAncor(bool black = true)
    {

        if (black)
            this.color = this.tileWeight == 0 ? Color.white : Color.black;
        else
            this.color = this.tileWeight == 0 ? Color.white : Color.grey;

        tileObj.GetComponent<MeshRenderer>().material.color = this.color;
    }
}




/// <summary>
/// to prob delete
/// </summary>
[System.Serializable]
public class Identifier
{
    public Color color = Color.black;

    public enum TileType
    {
        VOID,
        FLOORROOM,
        WALL,
        ROOF,
        FLOORCORRIDOR,
        AVOID
    }

    public TileType tileType = 0;
    public float cost = 0;
}

/// <summary>
/// to most liely delete
/// </summary>
public class CAtiles
{
    public TileOBJ tileCA;

    public bool empty;

    // when empty is true means white unliving
    // when balck means full living

    public CAtiles(float perc, TileOBJ normTile)
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
    }

    public CAtiles(bool _empty, TileOBJ normTile)
    {
        tileCA = normTile;
        empty = _empty;
    }


}

/// <summary>
///  to delete
/// </summary>
public class AStar_Node
{

    public TileOBJ refToGameObj;
    public AStar_Node parent;

    public float g = 0;
    public float f = 0;
    public float h = 0;

    public AStar_Node(TileOBJ gameobject)
    {
        refToGameObj = gameobject;
    }

}

