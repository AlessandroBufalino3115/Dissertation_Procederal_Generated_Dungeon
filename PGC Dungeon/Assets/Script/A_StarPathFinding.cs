using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class A_StarPathFinding : MonoBehaviour
{
   
public class Node 
    {

        public Tile refToGameObj;
        public Node parent;
        
        public float g = 0;
        public float f = 0;
        public float h = 0;

        public Node(Tile gameobject) 
        { 
            refToGameObj = gameobject;
        }

        public void SetParent(Node _parent) 
        {
            parent = _parent;
        }

        




    }

    List<Node> openList = new List<Node>();
    List<Node> closedList = new List<Node>();


    public Node start_node;
    public Node end_node;

    static public A_StarPathFinding instance;

    int[,] childPosArry = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    

    public List<Node> SolveA_StarPathfinding2D(Tile[][] tileArray2D) 
    {
        // here we need a way to turn the whatever given tileset into nodes prob inheritance is best here
        openList = new List<Node>();
        closedList = new List<Node>();

        int ran_x_s = Random.Range(1, TileVolumeGenerator.Instance.x_Length);
        int ran_x_e = Random.Range(1, TileVolumeGenerator.Instance.x_Length);

        int ran_y_s = Random.Range(1, TileVolumeGenerator.Instance.y_Height) ;
        int ran_y_e = Random.Range(1, TileVolumeGenerator.Instance.y_Height);

        //int ran_x_s = 1;
        //int ran_x_e = 99;

        //int ran_y_s = 1;
        //int ran_y_e = 99;


        tileArray2D[ran_y_s][ran_x_s].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.red;
        tileArray2D[ran_y_e][ran_x_e].arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.green;

        start_node = new Node(tileArray2D[ran_y_s][ran_x_s]);
        start_node.parent = null;

        end_node = new Node(tileArray2D[ran_y_e][ran_x_e]);


        openList.Add(start_node);



        int iter = 0;

        while (openList.Count > 0) 
        {

            iter++;

            Node currNode = openList[0];
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

                List<Node> path = new List<Node>();

                Node current = currNode;

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
                List<Node> children = new List<Node>();


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
                        Node new_node = new Node(tileArray2D[node_position[1]][node_position[0]]);
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

    public void RetrivePath(Node nodePath) 
    {
        List<Node> path = new List<Node>();

        Node current = nodePath;

        while (current.parent != null) 
        {
            path.Add(current);
            current = current.parent;
        }
        

        foreach (var node in path) 
        {
            node.refToGameObj.arrayTileObj.GetComponent<MeshRenderer>().material.color = Color.blue;
        }

        //while current is not None:
        //        path.append(current.position)
        //        current = current.parent
        //    return path[::- 1] # Return reversed path
    }

    private float UcledianDistance2D(Node end_point, Node curr_node) 
    {

        float distance = Mathf.Pow((end_point.refToGameObj.x_cord - curr_node.refToGameObj.x_cord), 2) + Mathf.Pow((end_point.refToGameObj.y_cord - curr_node.refToGameObj.y_cord), 2);
        distance = Mathf.Sqrt(distance);
        return distance;
    }
}
