using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class APathUtils
{


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
    public static Tuple<List<AStar_Node>, List<AStar_Node>>  A_StarPathfinding2DNorm(Tile[][] tileArray2D, Vector2Int start, Vector2Int end,bool euclideanDis = true, bool perf = false, bool colorDebug = false, bool diagonalTiles = false)
    {
        int timerStart = Environment.TickCount & Int32.MaxValue;

        // here we need a way to turn the whatever given tileset into nodes prob inheritance is best here
        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();


        AStar_Node start_node = new AStar_Node(tileArray2D[start.y][start.x]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[end.y][end.x]);


        int[,] childPosArry = new int[0,0];

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






}
