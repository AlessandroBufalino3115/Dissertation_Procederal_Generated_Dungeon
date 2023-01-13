using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class PCGManager : MonoBehaviour
{
    public Dictionary<BasicTile.TileType, float> tileTypeToCostDict = new Dictionary<BasicTile.TileType, float>();

    public bool working;
    public bool gizmo;


    [Tooltip("Test material given to draw the mesh generated variant of the outcome")]
    public Material mat;

    public BasicTile[][] gridArray2D = new BasicTile[1][];


    private GameObject plane;
    public GameObject Plane
    {
        get { return plane; }
    }

    [Tooltip("How wide the drawing canvas where the algorithms will take place will be")]
    [Range(40f, 650f)]
    public int width = 50;
    [Tooltip("How tall the drawing canvas where the algorithms will take place will be")]
    [Range(40f, 650f)]
    public int height = 50;

    [Tooltip("How tall the dungeon will be.")]
    [Range(4f, 8f)]
    public int RoomHeight = 6;


    [Tooltip("How many floors will the dungeons have")]
    [Range(1f, 4f)]
    public int DungeonFloors=1;


    public enum MainAlgo
    {
        VORONI = 0,
        RANDOM_WALK = 1,
        ROOM_GEN = 2,
        CELLULAR_AUTOMATA =3,
        L_SYSTEM = 4,
        DELUNARY = 5,
        WFC = 6,
        PERLIN_NOISE = 7,
        DIAMOND_SQUARE =8
    }

    [Tooltip("The main algorithm to start with, this depends on the type of dungeons prefered")]
    public MainAlgo mainAlgo;
    public string TileSetRuleFileName = "";

    public List<GameObject> FloorTiles = new List<GameObject>();
    public List<GameObject> CeilingTiles = new List<GameObject>();
    public List<GameObject> WallsTiles = new List<GameObject>();


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

                foreach (Transform child in transform)
                    DestroyImmediate(child.gameObject);
                
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

    public void DrawTileMap()
    {
        int iter = 0;


        if (WallsTiles.Count == 0|| CeilingTiles.Count == 0|| FloorTiles.Count == 0) 
        {

            EditorUtility.DisplayDialog("Inavlid Index Given", "lase ensure all the index are within range", "OK!");
            return;
        }



        for (int z = 0; z < RoomHeight; z++)  // this is the heihgt of the room
        {
            for (int y = 0; y < gridArray2D.Length; y++)
            {
                for (int x = 0; x < gridArray2D[0].Length; x++)
                {
                    if (z == 0 || z == RoomHeight - 1) //we draw everything as this is the ceiling and the floor
                    {
                        if (gridArray2D[y][x].tileType != BasicTile.TileType.VOID)
                        {
                            var objRef = Instantiate(FloorTiles.Count > 1 ? FloorTiles[Random.Range(0, FloorTiles.Count)] : FloorTiles[0], this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            iter++;
                        }
                    }

                    if (gridArray2D[y][x].tileType == BasicTile.TileType.WALL)
                    {
                        var checkVector = new Vector2Int(x, y);

                        checkVector = new Vector2Int(x + 1, y);// riht check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 90, 0);

                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 90, 0);

                                iter++;
                            }
                        }



                        checkVector = new Vector2Int(x - 1, y);// left check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 270, 0);

                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 270, 0);

                                iter++;

                            }
                        }




                        checkVector = new Vector2Int(x, y + 1);// above check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.name = "0";

                            objRef.transform.Rotate(0, 0, 0);
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.name = "0";

                                objRef.transform.Rotate(0, 0, 0);

                                iter++;
                            }
                        }





                        checkVector = new Vector2Int(x, y - 1);// down check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 180, 0);
                            objRef.transform.name = "180";
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[Random.Range(0, WallsTiles.Count)] : WallsTiles[0], this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 180, 0);
                                objRef.transform.name = "180";

                                iter++;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"{iter}");

    }
}
