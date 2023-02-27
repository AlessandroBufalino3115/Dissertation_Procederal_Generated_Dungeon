using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquareMA : MonoBehaviour,IUndoInteraction
{
    [HideInInspector]
    public PCGManager pcgManager;

    //general

    [HideInInspector]
    public bool pathType = false;

    [HideInInspector]
    public int neighboursNeeded = 3;

    [HideInInspector]
    public int typeOfTri;

    [HideInInspector]
    public int minSize = 40;

    [HideInInspector]
    public List<List<Tile>> rooms = new List<List<Tile>>();

    [HideInInspector]
    public List<Edge> edges = new List<Edge>();

    [HideInInspector]
    public bool allowedBack;
    [HideInInspector]
    public bool allowedForward;
    [HideInInspector]
    public int currStateIndex = 0;



    [HideInInspector]
    public GeneralUtil.PathFindingType pathFindingType;

    [HideInInspector]
    public GeneralUtil.UISTATE currUiState = GeneralUtil.UISTATE.MAIN_ALGO;

    public void DeleteLastSavedRoom()
    {
        if (currUiState == GeneralUtil.UISTATE.EXTRA_ROOM_GEN)
            rooms.RemoveAt(rooms.Count - 1);
    }

    public void InspectorAwake()
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
        pcgManager.UndoInteraction = this;
    }

}
