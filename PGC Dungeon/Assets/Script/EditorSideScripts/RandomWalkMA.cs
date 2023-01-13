using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class RandomWalkMA : MonoBehaviour
{
    private PCGManager pcgManager;
    public PCGManager PcgManager
    {
        get { return pcgManager; }
    }

    //specific to main algo

    private int iterations;
    public int Iterations
    {
        get { return iterations; }
        set { iterations = value; }
    }

    private bool startFromMiddle = false;
    public bool StartFromMiddle
    {
        get { return startFromMiddle; }
        set { startFromMiddle = value; }
    }

    private bool alreadyPassed;
    public bool AlreadyPassed
    {
        get { return alreadyPassed; }
        set { alreadyPassed = value; }
    }



    //general

    private bool pathType = false;
    public bool PathType
    {
        get { return pathType; }
        set { pathType = value; }
    }


    private int neighboursNeeded = 3;
    public int NeighboursNeeded
    {
        get { return neighboursNeeded; }
        set { neighboursNeeded = value; }
    }

    private bool typeOfTri;
    public bool TypeOfTri
    {
        get { return typeOfTri; }
        set { typeOfTri = value; }
    }

    private int minSize =40;
    public int MinSize
    {
        get { return minSize; }
        set { minSize = value; }
    }


    private bool started;
    public bool Started
    {
        get { return started; }
        set { started = value; }
    }


    public List<List<BasicTile>> rooms = new List<List<BasicTile>>();
    public List<Edge> edges = new List<Edge>();



    public enum PathFindingType 
    {
       A_STAR,
       DJISTRA,
       BFS,
       DFS
    }
    private PathFindingType pathFindingType;
    public PathFindingType PathfindingType
    {
        get { return pathFindingType; }
        set { pathFindingType = value; }
    }





    public void InspectorAwake() 
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
    }



}
