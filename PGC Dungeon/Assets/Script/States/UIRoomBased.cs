using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class UIRoomBased : UiBaseState
{

    //bsp

    private int maxWidth;
    private int minWidth;

    private int maxHeight;
    private int minHeight;

    private int numOfRoom;

    private int xAxis;
    private int yAxis;

    private bool BPS;

    private int[,] grid2d;
    private List<RoomsClass> roomList = new List<RoomsClass>();
    List<BoundsInt> roomsListBPSAlgo = new List<BoundsInt>();

    public override void onExit(StateUIManager currentMenu)
    {
    }
    public override void onGizmos(StateUIManager currentMenu)
    {
    }
    public override void onGUI(StateUIManager currentMenu)
    {



        if (currentMenu.working) 
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "LOADING");
        }
        else 
        {

            GUI.Box(new Rect(5, 10, 260, 650), "");

            minHeight = (int)GUI.HorizontalSlider(new Rect(10, 50, 100, 20), minHeight, 5, 20);
            GUI.Label(new Rect(130, 45, 150, 30), "min height of room: " + minHeight);
            maxHeight = (int)GUI.HorizontalSlider(new Rect(10, 75, 100, 20), maxHeight, 5, 20);
            GUI.Label(new Rect(130, 70, 150, 30), "max height of room: " + maxHeight);


            minWidth = (int)GUI.HorizontalSlider(new Rect(10, 100, 100, 20), minWidth, 5, 20);
            GUI.Label(new Rect(130, 95, 150, 30), "min width of room: " + minWidth);
            maxWidth = (int)GUI.HorizontalSlider(new Rect(10, 125, 100, 20), maxWidth, 5, 20);
            GUI.Label(new Rect(130, 120, 150, 30), "max width of room: " + maxWidth);


            numOfRoom = (int)GUI.HorizontalSlider(new Rect(10, 150, 100, 20), numOfRoom, 3, 10);
            GUI.Label(new Rect(130, 145, 150, 30), "max num of rooms: " + numOfRoom);


            xAxis = (int)GUI.HorizontalSlider(new Rect(10, 175, 100, 20), xAxis, 30, 200);
            GUI.Label(new Rect(130, 170, 150, 30), "width of the map: " + xAxis);
            yAxis = (int)GUI.HorizontalSlider(new Rect(10, 200, 100, 20), yAxis, 30, 200);
            GUI.Label(new Rect(130, 195, 150, 30), "height of the map: " + yAxis);


            BPS = GUI.Toggle(new Rect(10, 230, 120, 30), BPS, "use BPS");




            if (BPS) 
            {
                if (GUI.Button(new Rect(10, 260, 120, 30), "BPS algo"))
                {
                    currentMenu.working = true;
                    roomsListBPSAlgo.Clear();

                    if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    {
                        BPSRoomGen();
                        SetUpWeights(currentMenu.gridArrayObj2D);
                        AlgosUtils.SetColorAllObjAnchor(currentMenu.gridArrayObj2D);
                    }

                    else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                    {
                        BPSRoomGen();
                        SetUpWeights(currentMenu.gridArray2D);
                        currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D, true);
                    }
                    currentMenu.working = false;

                }
            }
            else
            {

                if (GUI.Button(new Rect(10, 260, 120, 30), "GenRooms"))
                {
                    currentMenu.working = true;
                    grid2d = new int[yAxis, xAxis];
                    roomList.Clear();

                    if (currentMenu.dimension == StateUIManager.Dimension.TWOD)
                    {
                        RandomRoomGen();
                        SetUpWeights(currentMenu.gridArrayObj2D);
                        AlgosUtils.SetColorAllObjAnchor(currentMenu.gridArrayObj2D);
                    }

                    else if (currentMenu.dimension == StateUIManager.Dimension.PLANE)
                    {
                        RandomRoomGen();
                        SetUpWeights(currentMenu.gridArray2D);
                        currentMenu.plane.GetComponent<Renderer>().material.mainTexture = AlgosUtils.SetUpTextBiColAnchor(currentMenu.gridArray2D, true);
                    }
                    currentMenu.working = false;

                }

            }






            if (GUI.Button(new Rect(10, 300, 120, 30), "Go Back"))
                currentMenu.ChangeState(0);
        }

    }

    public override void onStart(StateUIManager currentMenu)
    {
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }

    private void SetUpWeights(BasicTile[][] gridArr) 
    {
        foreach (var room in roomList)
        {
            for (int i = 0; i < room.tileCoords.Count; i++)
            {
                gridArr[room.tileCoords[i].y][room.tileCoords[i].x].tileWeight = 1; 
            }
        }
    }






    #region Random Room Gen

    private void RandomRoomGen() 
    {
        int tries = numOfRoom * 4;


        while (tries > 0)
        {
            tries--;


            if (roomList.Count >= numOfRoom) { break; }


            Vector2Int ranStartPoint = new Vector2Int(Random.Range(0, xAxis), Random.Range(0, yAxis));

            RoomsClass currRoom = new RoomsClass();

            int ranWidth = Random.Range(minWidth, maxWidth);
            if (Random.value > 0.5f)
                ranWidth *= -1;


            int ranHeight = Random.Range(minHeight, minWidth);
            if (Random.value > 0.5f)
                ranHeight *= -1;



            if (ranWidth < 0)   // so the added withd is negative so the ranPoint x is the most positve
            {
                if (ranStartPoint.x + ranWidth < 0)
                    continue;


                currRoom.maxX = ranStartPoint.x;
                currRoom.minX = ranStartPoint.x + ranWidth;


                Debug.Log($"{currRoom.minX} {currRoom.maxX}");
            }
            else
            {

                if (ranStartPoint.x + ranWidth > xAxis -1)
                    continue;

                currRoom.minX = ranStartPoint.x;
                currRoom.maxX = ranStartPoint.x + ranWidth;


                Debug.Log($"{currRoom.minX} {currRoom.maxX}");
            }



            if (ranHeight < 0)   // so the added withd is negative so the ranPoint x is the most positve
            {
                if (ranStartPoint.y + ranHeight < 0)
                    continue;

                currRoom.maxY = ranStartPoint.y;
                currRoom.minY = ranStartPoint.y + ranHeight;


                Debug.Log($"{currRoom.minY} {currRoom.maxY}");
            }
            else
            {

                if (ranStartPoint.y + ranHeight > yAxis - 1)
                    continue;

                currRoom.minY = ranStartPoint.y;
                currRoom.maxY = ranStartPoint.y + ranHeight;

                Debug.Log($"{currRoom.minY} {currRoom.maxY}");
            }

            currRoom.WorkOutCoords();

            bool toAdd = true;

            for (int i = 0; i < roomList.Count; i++)
            {
                if (AABBCol(currRoom, roomList[i])) 
                {
                    //Debug.Log($"DId this call {roomsList.Count}");
                    toAdd = false;
                    break;
                }
            }


            if (toAdd) 
            {
                WorkoutRegion(currRoom);

                roomList.Add(currRoom);
            }
        }
    }


    /// <summary>
    /// returns true when something is hit ---- return false when nothing is hit
    /// </summary>
    /// <param name="newRoom"></param>
    /// <param name="oldRoom"></param>
    /// <returns></returns>
    private bool AABBCol(RoomsClass newRoom, RoomsClass oldRoom ) 
    {

        for (int i = 0; i < newRoom.verticesList.Count; i++)
        {
            if ((oldRoom.minX <= newRoom.verticesList[i].x && newRoom.verticesList[i].x <= oldRoom.maxX) && (oldRoom.minY <= newRoom.verticesList[i].y && newRoom.verticesList[i].y <= oldRoom.maxY))
            {
                return true;
            }
        }


        return false;
    }


    #endregion




    private void WorkoutRegion(RoomsClass room) 
    {
        room.tileCoords = new List<Vector2Int>();

        //Debug.Log($"{currRoom.minY} {currRoom.maxY} {currRoom.minX} {currRoom.maxX}");

        for (int y = room.minY; y < room.maxY + 1; y++)
        {
            for (int x = room.minX; x < room.maxX + 1; x++)
            {
                room.tileCoords.Add(new Vector2Int(x, y));
            }
        }
    }


    #region BPS




    private void BPSRoomGen()
    {
        BoundsInt map = new BoundsInt();

        map.min = new Vector3Int(0, 0, 0);
        map.max = new Vector3Int(xAxis, 0, yAxis);

        roomsListBPSAlgo = BPSAlgo2d(map);

        while (roomsListBPSAlgo.Count > numOfRoom + 1)
        {
            int ranIDX = Random.Range(0, roomsListBPSAlgo.Count - 1);

            roomsListBPSAlgo.RemoveAt(ranIDX);
        }


        BoundsToWeights();

    }

    private void BoundsToWeights() 
    {
        roomList.Clear();

        foreach (var room in roomsListBPSAlgo)
        {
            roomList.Add(new RoomsClass() { maxX = room.xMax, maxY = room.zMax, minX = room.xMin, minY = room.zMin });

            WorkoutRegion(roomList[roomList.Count - 1]);
        }
    }




    public List<BoundsInt> BPSAlgo2d(BoundsInt toSplit)
    {

        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

        roomsQueue.Enqueue(toSplit);  

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();   

            if (room.size.z >= minHeight && room.size.x >= minWidth)   
            {

                if (Random.value < 0.5f)
                {
                    if (room.size.z >= minHeight * 2 + 1)
                    {
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else if (room.size.x >= minWidth * 2 + 1)
                    {
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else
                    {
                        roomsListBPSAlgo.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2 + 1)
                    {
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else if (room.size.z >= minHeight * 2 + 1)
                    {
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else
                    {
                        roomsListBPSAlgo.Add(room);
                    }
                }
            }
        }

        return roomsListBPSAlgo;


    }

    private void SplitVert(int minWidth, BoundsInt room, Queue<BoundsInt> roomQue)
    {

        int minX = room.min.x;
        int maxX = room.max.x;

        int adjustedMinX = minX + minWidth;
        int adjustedMaxX = maxX - minWidth;

        var ranPosition = Random.Range(adjustedMinX, adjustedMaxX);

        BoundsInt roomLeft = new BoundsInt();

        roomLeft.min = new Vector3Int(room.min.x, 0, room.min.z);
        roomLeft.max = new Vector3Int(ranPosition, 0, room.max.z);


        BoundsInt roomRight = new BoundsInt();

        roomRight.min = new Vector3Int(ranPosition, 0, room.min.z);
        roomRight.max = new Vector3Int(room.max.x, 0, room.max.z);

        roomQue.Enqueue(roomRight);
        roomQue.Enqueue(roomLeft);

    }

    private void SplitHori(int minHeight, BoundsInt room, Queue<BoundsInt> roomQue)
    {
        int minY = room.min.z;
        int maxY = room.max.z;

        int adjustedMinY = minY + minHeight;
        int adjustedMaxY = maxY - minHeight;

        var ranPosition = Random.Range(adjustedMinY, adjustedMaxY);

        BoundsInt roomTop = new BoundsInt();

        roomTop.min = new Vector3Int(room.min.x, 0, ranPosition);
        roomTop.max = new Vector3Int(room.max.x, 0, room.max.z);

        BoundsInt roomBot = new BoundsInt();

        roomBot.min = new Vector3Int(room.min.x, 0, room.min.z);
        roomBot.max = new Vector3Int(room.max.x, 0, ranPosition);

        roomQue.Enqueue(roomBot);
        roomQue.Enqueue(roomTop);

    }

    #endregion

}




public class RoomsClass
{
    public int minX;
    public int maxX;
    public int minY;
    public int maxY;

    public Vector2Int topLeftCoord;  
    public Vector2Int topRightCoord;    
    public Vector2Int botLeftCoord;  
    public Vector2Int botRightCoord;
    public List<Vector2Int> verticesList = new List<Vector2Int>();

    public int width;
    public int height;

    public List<Vector2Int> tileCoords = new List<Vector2Int>() ;
    public GameObject tile;


    public void WorkOutCoords() 
    {
        verticesList = new List<Vector2Int>();

        topLeftCoord = new Vector2Int(minX, maxY);
        topRightCoord = new Vector2Int(maxX, maxY);
        botLeftCoord = new Vector2Int(minX, minY);
        botRightCoord = new Vector2Int(maxX, minY);

        verticesList.Add(topLeftCoord);
        verticesList.Add(topRightCoord);
        verticesList.Add(botLeftCoord);
        verticesList.Add(botRightCoord);
    
    }


}

