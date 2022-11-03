using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static TestingScript;

public class TestingScript : MonoBehaviour
{
    // In this example we show how to invoke a coroutine and
    // continue executing the function in parallel.

    //private IEnumerator coroutine;

    //void Start()
    //{
    //    // - After 0 seconds, prints "Starting 0.0"
    //    // - After 0 seconds, prints "Before WaitAndPrint Finishes 0.0"
    //    // - After 2 seconds, prints "WaitAndPrint 2.0"
    //    print("Starting " + Time.time);

    //    // Start function WaitAndPrint as a coroutine.

    //    coroutine = WaitAndPrint(2.0f);
    //    StartCoroutine(coroutine);

    //    print("Before WaitAndPrint Finishes " + Time.time);
    //}

    //// every 2 seconds perform the print()
    //private IEnumerator WaitAndPrint(float waitTime)
    //{

    //    int num = 0;


    //    while (true)
    //    {
    //        yield return new WaitForSeconds(waitTime);
    //        print("WaitAndPrint " + Time.time);


    //        num++;


    //        if (num == 5) { break; }
    //    }



    //    Debug.Log($"time num is called {num}");

    //}
    //private void Start()
    //{
    //    MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    //    CombineInstance[] combine = new CombineInstance[meshFilters.Length];

    //    int i = 0;
    //    while (i < meshFilters.Length)
    //    {
    //        combine[i].mesh = meshFilters[i].sharedMesh;
    //        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
    //        meshFilters[i].gameObject.SetActive(false);

    //        i++;
    //    }
    //    transform.GetComponent<MeshFilter>().mesh = new Mesh();
    //    transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    //    transform.gameObject.SetActive(true);
    //}

    [SerializeField]
    public List<Triangle> triangulation = new List<Triangle>();

    private void Start()
    {

        List<Vector2> pointList = new List<Vector2>();

        pointList.Add(new Vector2(10, 20));
        pointList.Add(new Vector2(20, 40));
        pointList.Add(new Vector2(50, 26));
        pointList.Add(new Vector2(60, 23));
        pointList.Add(new Vector2(70, 67));
        pointList.Add(new Vector2(60, 34));
        pointList.Add(new Vector2(50, 23));
        pointList.Add(new Vector2(90, 78));
        pointList.Add(new Vector2(5, 46));
        pointList.Add(new Vector2(14, 74));


        triangulation = new List<Triangle>();

        Vector2 superTriangleA = new Vector2(10000, 10000);
        Vector2 superTriangleB = new Vector2(10000,0);
        Vector2 superTriangleC = new Vector2(0, 10000);

        triangulation.Add(new Triangle(superTriangleA, superTriangleB, superTriangleC));

        Debug.Log($"{triangulation.Count}");
        Debug.Log($"{pointList.Count}");

        foreach (Vector2 point in pointList)
        {
            Debug.Log($"how many times does the loop call");

            List<Triangle> badTriangles = new List<Triangle>();

            foreach (Triangle triangle in triangulation)
            {
                if (IspointInCircumcircle(triangle.a, triangle.b, triangle.c, point)) 
                {
                    badTriangles.Add(triangle);
                    Debug.Log($"This should activate the firts time");
                }
                    


            }


            Debug.Log($"this si the amount og bad triangles {badTriangles.Count}");



            List<Edge> polygon = new List<Edge>();

            foreach (Triangle triangle in badTriangles)
            {
                foreach (Edge triangleEdge in triangle.edges)
                {
                    bool isShared = false;

                    foreach (Triangle otherTri in badTriangles)
                    {
                        if (otherTri == triangle) { continue; }

                        Debug.Log($"id otn eveen knwo what to look for in here");
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

            Debug.Log($"new trinagulation count {triangulation.Count}");


        }




        for (int i = triangulation.Count - 1; i >= 0; i--)
        {
            if (triangulation[i].HasVertex(superTriangleA)  || triangulation[i].HasVertex(superTriangleB) || triangulation[i].HasVertex(superTriangleC))
            {
                triangulation.Remove(triangulation[i]);
            }
            
        }




        Debug.Log($"{triangulation.Count}");





    }

    private void Update()
    {





        foreach (var triangle in triangulation)
        {


            foreach (var edge in triangle.edges)
            {
               // Debug.Log($"{edge.edge[0][0]} and {edge.edge[0][1]}");
                Debug.DrawLine(new Vector3(edge.edge[0][0], 0, edge.edge[0][1]), new Vector3(edge.edge[1][0], 0, edge.edge[1][1]), Color.green);
            }
        }







    }

    public class Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public Edge[] edges = new Edge[3];

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;


            this.edges[0] = new Edge(a, b);
            this.edges[1] = new Edge(b, c);
            this.edges[2] = new Edge(c, a);
        }


        //public bool IsPointInCircumCircle(Vector2 point) 
        //{
        
        //}



        public bool HasVertex(Vector2 point) 
        {
            if (a== point  || b ==point  || c == point) { return true; }
            else { return false; }
        }

    }

    public class Edge
    {
        public Vector2[] edge = new Vector2[2];

        public Edge(Vector2 a, Vector2 b)
        {
            edge[0] = a;
            edge[1] = b;
        }

    }



























    /// <summary>
    /// returns true if the point is in the circle
    /// </summary>
    public bool IspointInCircumcircle(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        Debug.Log($"{A}");
        Debug.Log($"{B}");
        Debug.Log($"{C}");
        Debug.Log($"{D}");


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
        ) > 0)
        {
            return false;
        }

        else { return true; }

    }



    public bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }

    public float distance(int point1X, int point1Y, int point2X, int point2Y)
    {
        return MathF.Sqrt(MathF.Pow((point1X - point2X), 2) + MathF.Pow((point1Y - point2Y), 2));
    }
}
