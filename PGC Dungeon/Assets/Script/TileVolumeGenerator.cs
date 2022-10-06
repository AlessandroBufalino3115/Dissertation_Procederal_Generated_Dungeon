using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TileVolumeGenerator : MonoBehaviour
{

    public int volumeWidth;
    public int volumeFloors;
    public int volumeLength;

    [SerializeField] GameObject emptyBlock;
    [SerializeField] GameObject CubeBlock;
    public bool clearBlock = false;

    public int acceptedRoomFailures;


    List<GameObject> Tiles = new List<GameObject>();

    public void GenTiles()
    {
        int timerStart = Environment.TickCount & Int32.MaxValue;

        float _x = 0;
        float _y = 0;
        float _z = 0;
        int blockNum = 0;


        for (int y = 0; y < volumeFloors * 3; y++)
        {

            for (int z = 0; z < volumeLength; z++)
            {

                for (int x = 0; x < volumeWidth; x++)
                {

                    GameObject newRef = null;

                    if (clearBlock) 
                    {
                        newRef = Instantiate(emptyBlock, this.gameObject.transform);
                    }
                    else
                    { 
                        newRef = Instantiate(CubeBlock, this.gameObject.transform);
                    }


                    newRef.transform.localPosition = new Vector3(_x + 0.5f, _y + 0.5f, _z + 0.5f);

                    Tiles.Add(newRef);


                    _x++;
                    blockNum++;
                }

                _z++;
                _x = 0;

            }

            _y++;
            _x = 0;
            _z = 0;
        }



        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;

        Debug.Log($"The total time this has taken was {totalTicks}, to generate {blockNum} positions");
        // 1 tick seems to be 1 millisecond

    }



    public void DestroyAllTiles() 
    {
        int timerStart = Environment.TickCount & Int32.MaxValue;


        foreach (Transform child in transform)
            Destroy(child.gameObject);



        Tiles.Clear();
        int timerEnd = Environment.TickCount & Int32.MaxValue;

        int totalTicks = timerEnd - timerStart;

        Debug.Log($"The total time that destorying all the children has taken was {totalTicks}");
    }




    public void RoomSpawner() 
    {

        int stratoSize = volumeLength * volumeWidth;

        int offsetIndex = 0;

        int currentFailures;


        for (int x = 0; x < volumeFloors; x++)
        {
            for (int i = 0; i < stratoSize; i++)
            {
                if (clearBlock == false)
                    Tiles[i + offsetIndex].transform.GetComponent<MeshRenderer>().material.color = Color.black;

                if (i == 140)
                    ActuallySpawnRoom(i + offsetIndex);


                 
            }
            offsetIndex = stratoSize * 3;
        }
        
        
    }



    // needs to pick a random start index on then call this 
    //half way it all prinintg that should be the middle so use that as the main connection node



    // i need the origin point to be 1 away from any other room
    public void ActuallySpawnRoom(int originIndex) 
    {

        int width = Random.Range(3, 9);
        int length = Random.Range(3, 9);

        int targetindex = originIndex;

        for (int i = 0; i < width; i++)
        {
            for (int x = 0; x < length; x++)
            {
                if (clearBlock == true)
                {
                    GameObject newRef = Instantiate(CubeBlock, this.gameObject.transform);
                    newRef.transform.position = new Vector3(Tiles[targetindex].gameObject.transform.position.x, Tiles[targetindex].gameObject.transform.position.y, Tiles[targetindex].gameObject.transform.position.z);
                }
                else
                    Tiles[targetindex].transform.GetComponent<MeshRenderer>().material.color = Color.red;
                targetindex--;
            }
            targetindex -= volumeWidth;
            targetindex += length;
        }
        
    }






}





public class Room 
{ 
    public List<GameObject> roomTiles = new List<GameObject>();
    public Vector2 pos2D;
    public Vector3 pos3D;
    

    public Room() { }
}
