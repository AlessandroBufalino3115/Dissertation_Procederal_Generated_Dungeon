using System.Collections.Generic;
using UnityEngine;

public class RandomWalkMA : MonoBehaviour
{

    [HideInInspector]
    public PCGManager pcgManager;
   

    //specific to main algo
    [HideInInspector]
    public int iterations;

    [HideInInspector]
    public bool startFromMiddle = false;

    [HideInInspector]
    public bool alreadyPassed;




    //general

    [HideInInspector]
    public bool pathType = false;



    [HideInInspector]
    public int neighboursNeeded = 3;

    [HideInInspector]
    public int typeOfTri;


    [HideInInspector]
    public int minSize =40;

    [HideInInspector]
    public bool started;

    [HideInInspector]
    public List<List<Tile>> rooms = new List<List<Tile>>();

    [HideInInspector]
    public List<Edge> edges = new List<Edge>();

    [HideInInspector]
    public  bool allowedBack;
    [HideInInspector]
    public bool allowedForward;
    [HideInInspector]
    public int currStateIndex = 0;



    public enum PathFindingType 
    {
       A_STAR,
       DJISTRA,
       BFS,
       DFS
    }
    [HideInInspector]
    public PathFindingType pathFindingType;


    public enum UISTATE
    {
        MAIN_ALGO,
        CA,
        ROOM_GEN,
        EXTRA_ROOM_GEN,
        PATHING,
        GENERATION
    }
    [HideInInspector]
    public UISTATE currUiState = UISTATE.MAIN_ALGO;



    public void InspectorAwake() 
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
    }

}
