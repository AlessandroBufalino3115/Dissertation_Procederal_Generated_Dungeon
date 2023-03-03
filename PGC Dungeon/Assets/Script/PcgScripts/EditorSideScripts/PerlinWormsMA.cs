using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinWormsMA : MonoBehaviour
{
    [HideInInspector]
    public PCGManager pcgManager;
   
    [HideInInspector]
    public HashSet<Tile> wormsTiles = new HashSet<Tile>();

    // main algo specific
    [HideInInspector]
    public int offsetX;

    [HideInInspector]
    public int offsetY;

    [HideInInspector]
    public float scale;

    [HideInInspector]
    public int octaves;

    [HideInInspector]
    public float persistance;

    [HideInInspector]
    public float lacunarity;

  

    public void InspectorAwake()
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
    }

}
