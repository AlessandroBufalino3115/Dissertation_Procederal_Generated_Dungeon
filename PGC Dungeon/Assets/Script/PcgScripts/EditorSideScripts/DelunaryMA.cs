using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelunaryMA : MonoBehaviour
{
    [HideInInspector]
    public PCGManager pcgManager;
    
    [HideInInspector]
    public bool readyToGen = false;

    [HideInInspector]
    public List<List<Tile>> rooms = new List<List<Tile>>();


    public void InspectorAwake()
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
    }

}
