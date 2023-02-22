using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiMA : MonoBehaviour, IUndoInteraction
{
    [HideInInspector]
    public PCGManager pcgManager;

    [HideInInspector]
    public GeneralUtil.UISTATE currUiState = GeneralUtil.UISTATE.MAIN_ALGO;

    [HideInInspector]
    public bool allowedBack = false;
    [HideInInspector]
    public bool allowedForward = false;

    [HideInInspector]
    public int currStateIndex = 0;

    [HideInInspector]
    public List<List<Tile>> rooms = new List<List<Tile>>();


    [HideInInspector]
    public GeneralUtil.PathFindingType pathFindingType;
    [HideInInspector]
    public bool pathType = false;


    [HideInInspector]
    public List<Edge> edges = new List<Edge>();

    public void DeleteLastSavedRoom()
    {
        if (currUiState == GeneralUtil.UISTATE.EXTRA_ROOM_GEN)
            rooms.RemoveAt(rooms.Count - 1);
    }

    public void InspectorAwake()
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
    }

}
