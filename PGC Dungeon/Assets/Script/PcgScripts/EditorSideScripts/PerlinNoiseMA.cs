using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseMA : MonoBehaviour
{
    private PCGManager pcgManager;
    public PCGManager PcgManager
    {
        get { return pcgManager; }
    }


    // main algo specific
    private int offsetX;
    public int OffsetX
    {
        get { return offsetX; }
        set { offsetX = value; }
    }

    private int offsetY;
    public int OffsetY
    {
        get { return offsetY; }
        set { offsetY = value; }
    }


    private float scale;
    public float Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    private int octaves;
    public int Octaves
    {
        get { return octaves; }
        set { octaves = value; }
    }

    private float persistance;
    public float Persistance
    {
        get { return persistance; }
        set { persistance = value; }
    }

    private float lacunarity;
    public float Lacunarity
    {
        get { return lacunarity; }
        set { lacunarity = value; }
    }

    private float threshold;
    public float Threshold
    {
        get { return threshold; }
        set { threshold = value; }
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

    private int minSize = 40;
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


    public List<List<Tile>> rooms = new List<List<Tile>>();
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
