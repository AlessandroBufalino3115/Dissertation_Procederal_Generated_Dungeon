using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class A_StarPathFinding : MonoBehaviour
{
   


    static public A_StarPathFinding instance;

    int[,] childPosArry = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    

    public List<AStar_Node> SolveA_StarPathfinding2DTest(Tile[][] tileArray2D) 
    {
        // here we need a way to turn the whatever given tileset into nodes prob inheritance is best here
        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();

        int ran_x_s = Random.Range(1, TileVolumeGenerator.Instance.x_Length-1);
        int ran_x_e = Random.Range(1, TileVolumeGenerator.Instance.x_Length-1);

        int ran_y_s = Random.Range(1, TileVolumeGenerator.Instance.y_Height-1);
        int ran_y_e = Random.Range(1, TileVolumeGenerator.Instance.y_Height-1);


        tileArray2D[ran_y_s][ran_x_s].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.red;
        tileArray2D[ran_y_e][ran_x_e].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.green;

        AStar_Node start_node = new AStar_Node(tileArray2D[ran_y_s][ran_x_s]);
         start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[ran_y_e][ran_x_e]);


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

            if (currNode.refToGameObj.x_cord == end_node.refToGameObj.x_cord && currNode.refToGameObj.y_cord == end_node.refToGameObj.y_cord) 
            {

                List<AStar_Node> path = new List<AStar_Node>();

                AStar_Node current = currNode;

                while (current.parent != null)
                {
                    path.Add(current);
                    current = current.parent;
                }
                foreach (var node in openList) 
                {

                    node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
                }

                foreach (var node in path)
                {
                    node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.blue;
                }


                tileArray2D[ran_y_s][ran_x_s].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.red;
                tileArray2D[ran_y_e][ran_x_e].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.green;
                return path;

            }
            else 
            {
                List<AStar_Node> children = new List<AStar_Node>();


                for (int i = 0; i < childPosArry.Length/2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = {currNode.refToGameObj.x_cord+x_buff, currNode.refToGameObj.y_cord+ y_buff};


                    if (node_position[0] < 0  || node_position[1] < 0 || node_position[0] >= TileVolumeGenerator.Instance.x_Length || node_position[1] >= TileVolumeGenerator.Instance.y_Height) 
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



                    child.g = currNode.g + 0.5f;
                    child.h = UcledianDistance2D(end_node, child);
                    child.f = child.g + child.h;
                    child.parent = currNode;


                    foreach (var openListItem in openList)
                    {
                        if (child.refToGameObj.x_cord == openListItem.refToGameObj.x_cord && child.refToGameObj.y_cord == openListItem.refToGameObj.y_cord && child.g > openListItem.g)// 
                        {
                            continue; 
                        }
                    }


                   

                    openList.Add(child);

                    Debug.Log($"{openList.Count}");
                    
                }
            }
        }

        foreach (var node in openList)
        {

            node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.grey;
        }


        tileArray2D[ran_y_s][ran_x_s].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.red;
        tileArray2D[ran_y_e][ran_x_e].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.green;

        return null;
        
    }

    





    private float UcledianDistance2D(AStar_Node end_point, AStar_Node curr_node) 
    {
        float distance = Mathf.Pow((end_point.refToGameObj.x_cord - curr_node.refToGameObj.x_cord), 2) + Mathf.Pow((end_point.refToGameObj.y_cord - curr_node.refToGameObj.y_cord), 2);
        distance = Mathf.Sqrt(distance);
        return distance;
    }
}

public class AStar_Node
{

    public Tile refToGameObj;
    public AStar_Node parent;

    public float g = 0;
    public float f = 0;
    public float h = 0;

    public AStar_Node(Tile gameobject)
    {
        refToGameObj = gameobject;
    }

    public void SetParent(AStar_Node _parent)
    {
        parent = _parent;
    }

}
