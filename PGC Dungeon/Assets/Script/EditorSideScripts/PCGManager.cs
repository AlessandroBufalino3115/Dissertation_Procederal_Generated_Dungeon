using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PCGManager : MonoBehaviour
{
    public Dictionary<BasicTile.TileType, float> tileTypeToCostDict = new Dictionary<BasicTile.TileType, float>();

    public bool working;
    public bool gizmo;

    public Material mat;

    public BasicTile[][] gridArray2D = new BasicTile[1][];

    private GameObject plane;
    public GameObject Plane
    {
        get { return plane; }
    }

    [Range(40f, 650f)]
    public int width = 50;
    [Range(40f, 650f)]
    public int height = 50;

    [Range(4f, 8f)]
    public int RoomHeight = 6;


    [Range(1f, 4f)]
    public int DungeonFloors=1;

    public enum MainAlgo
    {
        VORONI = 0,
        RANDOM_WALK = 1,
        ROOM_GEN = 2,
        CA =3,
        L_SYSTEM = 4,
        DELU = 5,
        WFC = 6,
        PERLIN_NOISE = 7,
        DIAMOND_SQUARE =8
    }
    public MainAlgo mainAlgo;


    private int currMainAlgoIDX = 10;
    public int CurrMainAlgoIDX
    {
        get { return currMainAlgoIDX; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreatePlane()
    {
        RefreshPlane();

        plane =  GameObject.CreatePrimitive(PrimitiveType.Plane);

        //plane = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Plane), this.transform);

        plane.transform.position = Vector3.zero;
        plane.transform.parent = this.transform;

        plane.transform.localScale = new Vector3(width / 4, 1, height / 4);

        gridArray2D = new BasicTile[height][];

        for (int y = 0; y < height; y++)
        {
            gridArray2D[y] = new BasicTile[width];

            for (int x = 0; x < width; x++)
            {
                gridArray2D[y][x] = new BasicTile();
                gridArray2D[y][x].position = new Vector2Int(x, y);
                gridArray2D[y][x].tileType = BasicTile.TileType.VOID;
            }
        }
    }


    public void RefreshPlane() 
    {
        if (plane != null)
            DestroyImmediate(plane);
    }



    public void LoadMainAlgo()
    {
        DelPrevAlgo();

        currMainAlgoIDX = (int)mainAlgo;





        if ((int)mainAlgo == 0) 
        {
             var comp = this.transform.AddComponent<VoronoiMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 1) 
        {
            var comp = this.transform.AddComponent<RandomWalkMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 2)
        {
            var comp = this.transform.AddComponent<RanRoomGenMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 3)
        {
            var comp = this.transform.AddComponent<CellularAutomataMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 4)
        {
            var comp = this.transform.AddComponent<NewLSystem>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 5)
        {
            var comp = this.transform.AddComponent<DelunaryMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 6)
        {
            this.transform.AddComponent<WFCRuleDecipher>();


            var comp = this.transform.AddComponent<NewWFCAlog>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 7)
        {
           var  comp = this.transform.AddComponent<PerlinNoiseMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 8)
        {
            var comp = this.transform.AddComponent<DiamondSquareMA>();
            comp.InspectorAwake();
        }
        else 
        {
            Debug.Log($"There was a n issue with this setting");
        }


    }



    public void DelPrevAlgo()
    {
        switch (currMainAlgoIDX)
        {
            case 0:
                DestroyImmediate(this.transform.GetComponent<VoronoiMA>());
                break;
            case 1:

                DestroyImmediate(this.transform.GetComponent<RandomWalkMA>());

                break;
            case 2:
                DestroyImmediate(this.transform.GetComponent<RanRoomGenMA>());
                break;
            case 3:
                DestroyImmediate(this.transform.GetComponent<CellularAutomataMA>());
                break;
            case 4:
                DestroyImmediate(this.transform.GetComponent<NewLSystem>());
                break;
            case 5:
                DestroyImmediate(this.transform.GetComponent<DelunaryMA>());
                break;
            case 6:
                DestroyImmediate(this.transform.GetComponent<NewWFCAlog>());
                DestroyImmediate(this.transform.GetComponent<WFCRuleDecipher>());
                break;
            case 7:
                DestroyImmediate(this.transform.GetComponent<PerlinNoiseMA>());
                break;
            case 8:
                DestroyImmediate(this.transform.GetComponent<DiamondSquareMA>());
                break;


            default:
                break;
        }


        currMainAlgoIDX = 10;

    }


    public void FormObject(Mesh mesh)
    {
        GameObject newPart = new GameObject();
        newPart.transform.position = this.transform.position;
        newPart.transform.rotation = this.transform.rotation;
        newPart.transform.localScale = this.transform.localScale;

        var renderer = newPart.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = mat;

        var filter = newPart.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        var collider = newPart.AddComponent<MeshCollider>();
        collider.convex = false;
    }

    public void Restart() 
    {
        AlgosUtils.RestartArr(gridArray2D);
        Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = AlgosUtils.SetUpTextBiColAnchor(gridArray2D);
    }


}
