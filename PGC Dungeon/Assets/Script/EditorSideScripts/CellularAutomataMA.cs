using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataMA : MonoBehaviour
{
    private PCGManager pcgManager;
    public PCGManager PcgManager
    {
        get { return pcgManager; }
    }


    public void InspectorAwake()
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
    }

}
