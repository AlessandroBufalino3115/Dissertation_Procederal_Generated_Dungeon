using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPSalgo : MonoBehaviour
{

    public static BPSalgo Instance;

    public int initialWidth = 100;
    public int initialHeight = 100;

    List<BoundsInt> roomList = new List<BoundsInt>();

    public int minWidth = 10;
    public int minHeight = 10;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        BoundsInt initialRoom = new BoundsInt();

        initialRoom.min = new Vector3Int(0,0, 0);
        initialRoom.max = new Vector3Int(initialWidth,0, initialHeight);

        BSPAlgo(initialRoom);
        Debug.Log(roomList.Count);
    }



    private void OnDrawGizmos()
    {
        foreach (var room in roomList)
        {
            Gizmos.DrawLine(room.min, room.max);
        }
    }

    public List<BoundsInt> BSPAlgo(BoundsInt toSplit) 
    {

        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

        roomsQueue.Enqueue(toSplit);   // enque add to que
        while (roomsQueue.Count > 0) 
        {
            var room = roomsQueue.Dequeue();   // take out and split this

            // this room can either contain a room or split  room
            if (room.size.z >= minHeight && room.size.x >= minWidth)   // all rooms should at least be big enough
            {

                if (Random.value < 0.5f) 
                {

                    Debug.Log("hori first");

                    if (room.size.z >= minHeight * 2 +1) 
                    {
                        Debug.Log("in hehehehehehe");
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else if (room.size.x >= minWidth * 2 +1)
                    {
                        Debug.Log("oooooooo");
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else// if (room.size.x <= minWidth * 2 && room.size.y <= minHeight *2)
                    {
                        Debug.Log("add to list");
                        roomList.Add(room);
                    }


                    Debug.Log("actuallly skipped");
                }
                else 
                {


                    Debug.Log("veri first");
                    if (room.size.x >= minWidth * 2 +1)
                    {
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else if (room.size.z >= minHeight * 2 +1)
                    {
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else 
                    {

                        Debug.Log("add to list");
                        roomList.Add(room);
                    }
                }

            }
           
        }



        return roomList;
    
    
    
    
    }






    private void SplitVert(int minWidth, BoundsInt room, Queue<BoundsInt> roomQue) 
    {

        Debug.Log("000000000000000000000000000000000000000000000");

        int minX = room.min.x;
        int maxX = room.max.x;

        int adjustedMinX = minX + minWidth;
        int adjustedMaxX = maxX - minWidth;

        Debug.Log(adjustedMaxX + "    " + adjustedMinX);

        var ranPosition = Random.Range(adjustedMinX, adjustedMaxX);

        BoundsInt roomLeft = new BoundsInt();

        roomLeft.min = new Vector3Int(room.min.x, 0, room.min.z);
        roomLeft.max = new Vector3Int(ranPosition, 0, room.max.z);


        BoundsInt roomRight = new BoundsInt();

        roomRight.min = new Vector3Int(ranPosition, 0, room.min.z);
        roomRight.max = new Vector3Int(room.max.x,0, room.max.z);

        Debug.Log($"The size of room1 is x{roomLeft.size.x} and for the y{roomLeft.size.y}");
        Debug.Log($"The size of room2 is x{roomRight.size.x} and for the y{roomRight.size.y}");

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

        roomBot.min = new Vector3Int(room.min.x,0, room.min.z);
        roomBot.max = new Vector3Int(room.max.x, 0, ranPosition);

        Debug.Log($"The size of room1 is x{roomBot.size.x} and for the y{roomBot.size.y}");
        Debug.Log($"The size of room2 is x{roomTop.size.x} and for the y{roomTop.size.y}");


        roomQue.Enqueue(roomBot);
        roomQue.Enqueue(roomTop);

    }



}
