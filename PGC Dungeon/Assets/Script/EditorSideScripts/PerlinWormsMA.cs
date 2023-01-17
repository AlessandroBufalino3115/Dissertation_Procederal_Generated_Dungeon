using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinWormsMA : MonoBehaviour
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

    private float minThreshold;
    public float MinThreshold
    {
        get { return minThreshold; }
        set { minThreshold = value; }
    }


    private float maxThreshold;
    public float MaxThreshold
    {
        get { return maxThreshold; }
        set { maxThreshold = value; }
    }



    public void InspectorAwake()
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
    }




}
