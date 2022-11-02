using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CellularAutomataBasicAlgo;

public class LineMakingRoomThing : MonoBehaviour
{
    public static LineMakingRoomThing instance;

    [Range(4, 25)]
    public int linesX;

    [Range(4, 25)]
    public int linesY;

    [Range(4, 24)]
    public int roomsNum;


    int[,] childPosArry = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
    // it is possible that the above thigns completly break everything 


    public int test_x = 0;
    public int test_y = 0;

    Tile[][] gridArray2D = new Tile[1][];

    List<LineMakerRoom2D> roomsList = new List<LineMakerRoom2D>();

    private void Awake()
    {
        instance = this;
    }

    public void GenRoomLines2D(Tile[][] _gridArray)
    {
        int lengthY = _gridArray.Length;
        int lengthX = _gridArray[0].Length;



        for (int i = 0; i < lengthX; i++)
        {
            _gridArray[0][i].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
            _gridArray[lengthY - 1][i].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        for (int i = 0; i < lengthY; i++)
        {
            _gridArray[i][0].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
            _gridArray[i][lengthX - 1].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
        }


        List<int> coloumsX = new List<int>();
        List<int> coloumsY = new List<int>();


        int tries = 0;


        while (coloumsX.Count <= linesX)
        {
            tries++;


            if (tries == 400)
            {

                break;
            }


            int ran = Random.Range(4, lengthX - 5);

            if (coloumsX.Contains(ran)) { continue; }

            bool tooClose = false;


            for (int i = 0; i < coloumsX.Count; i++)
            {
                if (Mathf.Abs(ran - coloumsX[i]) <= 4)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose == false)
            {
                coloumsX.Add(ran);

            }

        }


        tries = 0;


        while (coloumsY.Count <= linesY)
        {
            tries++;


            if (tries == 400)
            {

                break;
            }



            int ran = Random.Range(4, lengthY - 5);

            if (coloumsY.Contains(ran)) { continue; }

            bool tooClose = false;


            for (int i = 0; i < coloumsY.Count; i++)
            {
                if (Mathf.Abs(ran - coloumsY[i]) <= 4)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose == false)
            {
                coloumsY.Add(ran);
            }

        }


        foreach (var item in coloumsX)
        {
            for (int i = 1; i < lengthY; i++)
            {
                if (_gridArray[i][item].arrayTileObj.GetComponent<MeshRenderer>().material.color == Color.black)
                {
                    break;
                }
                else
                {
                    _gridArray[i][item].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
                }
            }

        }

        foreach (var item in coloumsY)
        {

            float start = Random.Range(0f, 1f);

            if (start > 0.5)
            {
                for (int i = 1; i < lengthX; i++)
                {
                    if (_gridArray[item][i].arrayTileObj.GetComponent<MeshRenderer>().material.color == Color.black)
                    {
                        float ran = Random.Range(0f, 1f);

                        if (ran >= 0.10f)
                        {
                            _gridArray[item][i].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
                        }
                        else
                        {
                            break;
                        }

                    }
                    else
                    {
                        _gridArray[item][i].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
                    }
                }



            }
            else
            {


                for (int i = 1; i < lengthX; i++)
                {
                    if (_gridArray[item][lengthX - i - 1].arrayTileObj.GetComponent<MeshRenderer>().material.color == Color.black)
                    {
                        float ran = Random.Range(0f, 1f);

                        if (ran >= 0.10f)
                        {
                            _gridArray[item][lengthX - i - 1].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        _gridArray[item][lengthX - i - 1].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.black;
                    }
                }
            }
        }

        DecideRooms2D(_gridArray, lengthY, lengthX);
    }

    public void DecideRooms2D(Tile[][] _gridArray, int lengthY, int lengthX)
    {

        gridArray2D = _gridArray;


        List<string> takenPositions = new List<string>();


        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                if (_gridArray[y][x].arrayTileObj.GetComponent<MeshRenderer>().material.color == Color.black)
                {
                    takenPositions.Add(_gridArray[y][x].arrayTileObj.transform.name);
                }
            }
        }

        for (int i = 0; i < roomsNum; i++)
        {

            int x = 0;
            int y = 0;

            int tries = 400;

            while (true)
            {
                tries = tries + 1;

                if (tries == 400)
                {
                    break;
                }

                int ranX = Random.Range(1, lengthX - 1);
                int ranY = Random.Range(1, lengthY - 1);

                string pos = ranX + " " + ranY;

                if (!takenPositions.Contains(pos))
                {

                    y = ranY;
                    x = ranX;

                    takenPositions.Add(pos);
                    break;
                }
            }


            Color col = new Color(Random.Range(0.01F, 0.99F), Random.Range(0.01F, 0.99F), Random.Range(0.01F, 0.99F));
            int count = roomsList.Count;
            roomsList.Add(new LineMakerRoom2D(col, count));
            //StartCoroutine(Flood2D(x, y, col,i));
            Flood2D(x, y, col, i);

        }


        ConnectTheRooms();

    }



    public void ConnectTheRooms()
    {
        List<int> roomID = new List<int>();




        for (int i = 0; i < roomsNum; i++)
        {
            roomID.Add(i);
        }

        int tries = 0;
        while (roomID.Count > 0)
        {
            tries++;


            if (tries > 200) { break; }


            int roomOne = Random.Range(0, roomID.Count);
            roomOne = roomID[roomOne];
            roomID.Remove(roomOne);


            int roomTwo = Random.Range(0, roomID.Count);
            roomTwo = roomID[roomTwo];
            roomID.Remove(roomTwo);

            int roomThree = Random.Range(0, roomsNum);


            int startX = roomsList[roomOne].position[Random.Range(0, roomsList[roomOne].position.Count - 1)][0];
            int startY = roomsList[roomOne].position[Random.Range(0, roomsList[roomOne].position.Count - 1)][1];

            int endX = roomsList[roomTwo].position[Random.Range(0, roomsList[roomTwo].position.Count - 1)][0];
            int endY = roomsList[roomTwo].position[Random.Range(0, roomsList[roomTwo].position.Count - 1)][1];


            var corridor = SolveA_StarPathfinding2D(gridArray2D, startX, startY, endX, endY, roomsList[roomOne].colorRoom, roomsList[roomTwo].colorRoom);

            foreach (var node in corridor)
            {

                Color col = node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color;

                if (col == Color.black || col == Color.white)
                    node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
            }

            endX = roomsList[roomThree].position[Random.Range(0, roomsList[roomThree].position.Count - 1)][0];
            endY = roomsList[roomThree].position[Random.Range(0, roomsList[roomThree].position.Count - 1)][1];

            corridor = SolveA_StarPathfinding2D(gridArray2D, startX, startY, endX, endY, roomsList[roomOne].colorRoom, roomsList[roomThree].colorRoom);


            foreach (var node in corridor)
            {

                Color col = node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color;

                if (col == Color.black || col == Color.white)
                    node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
            }



        }






    }




    private void Flood2D(int x, int y, Color col, int i)
    {
        if (y >= 0 && x >= 0 && y < gridArray2D.Length && x < gridArray2D[y].Length)
        {


            if (gridArray2D[y][x].arrayTileObj.GetComponent<MeshRenderer>().material.color != Color.black && gridArray2D[y][x].arrayTileObj.GetComponent<MeshRenderer>().material.color != col)
            {

                roomsList[i].position.Add(new int[] { x, y });

                gridArray2D[y][x].arrayTileObj.GetComponent<MeshRenderer>().material.color = col;

                Flood2D(x + 1, y, col, i);
                Flood2D(x - 1, y, col, i);
                Flood2D(x, y + 1, col, i);
                Flood2D(x, y - 1, col, i);
            }
        }

    }



    private IEnumerator Flood2DCor(int x, int y)
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        if (y >= 0 && x >= 0 && y < gridArray2D.Length && x < gridArray2D[y].Length)
        {

            yield return wait;

            Color tileCol = gridArray2D[y][x].arrayTileObj.GetComponent<MeshRenderer>().material.color;


            if (tileCol != Color.black && tileCol != Color.white && tileCol != Color.green)
            {

                gridArray2D[y][x].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.green;

                StartCoroutine(Flood2DCor(x + 1, y));
                StartCoroutine(Flood2DCor(x - 1, y));
                StartCoroutine(Flood2DCor(x, y + 1));
                StartCoroutine(Flood2DCor(x, y - 1));
            }
        }

    }

    //draw line around the whole map   this can be done with bounds






    public List<AStar_Node> SolveA_StarPathfinding2D(Tile[][] tileArray2D, int startX, int startY, int endX, int endY, Color roomCol,Color targetCol)
    {
        // here we need a way to turn the whatever given tileset into nodes prob inheritance is best here
        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();


        AStar_Node start_node = new AStar_Node(tileArray2D[startY][startX]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[endY][endX]);


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



            Color tarColor = tileArray2D[currNode.refToGameObj.y_cord][currNode.refToGameObj.x_cord].arrayTileObj.GetComponent<MeshRenderer>().material.color;


            if (   (currNode.refToGameObj.x_cord == end_node.refToGameObj.x_cord && currNode.refToGameObj.y_cord == end_node.refToGameObj.y_cord)      ||   tarColor == targetCol  )
            {

                List<AStar_Node> path = new List<AStar_Node>();

                AStar_Node current = currNode;

                while (current.parent != null)
                {
                    path.Add(current);
                    current = current.parent;
                }

                return path;

            }
            else
            {
                List<AStar_Node> children = new List<AStar_Node>();


                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { currNode.refToGameObj.x_cord + x_buff, currNode.refToGameObj.y_cord + y_buff };


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
                        if (child.refToGameObj.x_cord == closedListItem.refToGameObj.x_cord && child.refToGameObj.y_cord == closedListItem.refToGameObj.y_cord)
                        {
                            continue;
                        }
                    }



                    //if white or black  costs 0.15
                    // same color 0
                    // other colort costs 0.18

                    float addedCost = 0;
                    Color childColor = tileArray2D[child.refToGameObj.y_cord][child.refToGameObj.x_cord].arrayTileObj.GetComponent<MeshRenderer>().material.color;


                    if (childColor == roomCol)
                    {
                        addedCost = 0.02f;
                    }
                    else if (childColor == Color.black || childColor == Color.white)
                    {
                        addedCost = 0.1f;
                    }
                    else if (childColor == Color.gray) 
                    {
                        addedCost = -0.15f;
                    }
                    else { addedCost = -0.05f; }



                    child.g = currNode.g + 0.3f;
                    child.h = UcledianDistance2D(end_node, child);
                    child.f = child.g + child.h + addedCost;
                    child.parent = currNode;


                    foreach (var openListItem in openList)
                    {
                        if (child.refToGameObj.x_cord == openListItem.refToGameObj.x_cord && child.refToGameObj.y_cord == openListItem.refToGameObj.y_cord && child.g > openListItem.g)
                        {
                            continue;
                        }
                    }

                    openList.Add(child);

                }
            }
        }

        return null;

    }



    public void CallFullDraw() 
    {
        StartCoroutine(Flood2DCor(test_x, test_y));
    }



    private float UcledianDistance2D(AStar_Node end_point, AStar_Node curr_node)
    {
        float distance = Mathf.Pow((end_point.refToGameObj.x_cord - curr_node.refToGameObj.x_cord), 2) + Mathf.Pow((end_point.refToGameObj.y_cord - curr_node.refToGameObj.y_cord), 2);
        distance = Mathf.Sqrt(distance);
        return distance;
    }









}




public class LineMakerRoom2D
{
    public List<int[]> position = new List<int[]>();
    public Color colorRoom;
    public int indexRoom;



    public LineMakerRoom2D(Color colorRoom, int indexRoom)
    {
        this.colorRoom = colorRoom;
        this.indexRoom = indexRoom;
    }
}
