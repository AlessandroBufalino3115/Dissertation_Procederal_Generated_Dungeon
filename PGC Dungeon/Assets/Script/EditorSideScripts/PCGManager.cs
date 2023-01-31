using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Metadata;
using Random = UnityEngine.Random;

public class PCGManager : MonoBehaviour
{
    public Dictionary<BasicTile.TileType, float> tileTypeToCostDict = new Dictionary<BasicTile.TileType, float>();

    [Tooltip("Test material given to draw the mesh generated variant of the outcome")]
    public Material mat;

    public BasicTile[][] gridArray2D = new BasicTile[1][];

    public BasicTile[][] prevGridArray2D = new BasicTile[1][];

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
    [Range(3f, 8f)]
    public int RoomHeight = 6;


    public enum MainAlgo
    {
        VORONI = 0,
        RANDOM_WALK = 1,
        ROOM_GEN = 2,
        CELLULAR_AUTOMATA = 3,
        L_SYSTEM = 4,
        DELUNARY = 5,
        WFC = 6,
        PERLIN_NOISE = 7,
        PERLIN_WORM = 8,
        DIAMOND_SQUARE = 9
    }

    [Space(30)]
    [Tooltip("The main algorithm to start with, this depends on the type of dungeons prefered")]
    public MainAlgo mainAlgo;

    [Space(30)]
    [Tooltip("Name of file of the Rule that contains the tiles")]
    public string TileSetRuleFileName = "";


    public List<TileRuleSetPCG> FloorTiles = new List<TileRuleSetPCG>();
    public List<TileRuleSetPCG> CeilingTiles = new List<TileRuleSetPCG>();
    public List<TileRuleSetPCG> WallsTiles = new List<TileRuleSetPCG>();
 

    [Space(30)]
    [Tooltip("Name of file of the Rule that contains the tiles")]
    public string WeightRuleFileName = "";
    public float[] tileCosts = new float[0];


    [Tooltip("How many floors will the dungeons have///THIS IS DISABLED")]
    [Header("THIS IS WHERE THE PLAYER GOES IN CASE OF TILESET GEN")]
    public List<GameObject> player = new List<GameObject>();

    [HideInInspector]
    public List<Chunk> chunks;


    private int chunkWidth = 10;
    public int ChunkWidth
    {
        get { return chunkWidth; }
        set { chunkWidth = value; }
    }


    private int chunkHeight = 10;
    public int ChunkHeight
    {
        get { return chunkHeight; }
        set { chunkHeight = value; }
    }



    private int currMainAlgoIDX = 10;
    public int CurrMainAlgoIDX
    {
        get { return currMainAlgoIDX; }
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    private void Update()
    {
        if (chunks.Count > 0) 
        {
            Debug.Log($"Getting calledd");


            CheckChunkRender();
        }
    }

    //to change
    private void CheckChunkRender() 
    {

        int length = gridArray2D[0].Length / chunkWidth;
        int width = gridArray2D.Length / chunkHeight;

        var indexesToDraw = new HashSet<int>();

       



        foreach (var player in player)
        {
            int collidedIndex = -1;
            for (int i = 0; i < chunks.Count; i++)
            {
                if (AABBCol(player.transform.position, chunks[i]))
                {
                    collidedIndex = i;
                    break;
                }
            }


            if (collidedIndex == -1)
            {
                Debug.Log($"The plyaer is out of bounds");
            }
            else
            {
                indexesToDraw.Add(collidedIndex);

                // this is where we add the other 8 things

            }
        }
    }



    /// <summary>
    /// returns true if it collides
    /// </summary>
    /// <param name="player"></param>
    /// <param name="chunk"></param>
    /// <returns></returns>
    private bool AABBCol(Vector3 player, Chunk chunk)
    {

        if (player.x >= chunk.bottomLeft.x && player.x < chunk.topRight.x    ) 
        {
            if (player.z >= chunk.bottomLeft.y && player.z < chunk.topRight.y) 
            {
                return true;
            }
        }



        return false;
    }


    public void CreateBackUpGrid() 
    {
        prevGridArray2D = new BasicTile[gridArray2D.Length][];
        for (int y = 0; y < gridArray2D.Length; y++)
        {
            prevGridArray2D[y] = new BasicTile[gridArray2D[0].Length];
            for (int x = 0; x < gridArray2D[0].Length; x++)
            {
                prevGridArray2D[y][x] = new BasicTile(gridArray2D[y][x]);
            }
        }
    }

    public void LoadBackUpGrid() 
    {

        gridArray2D = new BasicTile[prevGridArray2D.Length][];
        for (int y = 0; y < prevGridArray2D.Length; y++)
        {
            gridArray2D[y] = new BasicTile[prevGridArray2D[0].Length];
            for (int x = 0; x < prevGridArray2D[0].Length; x++)
            {
                gridArray2D[y][x] = new BasicTile(prevGridArray2D[y][x]);
            }
        }



        Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(gridArray2D, 0, 1, true);
    }


    public void CreatePlane()
    {
        RefreshPlane();

        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

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
            var comp = this.transform.AddComponent<PerlinNoiseMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 8)
        {
            var comp = this.transform.AddComponent<PerlinWormsMA>();
            comp.InspectorAwake();
        }
        else if ((int)mainAlgo == 9)
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
                DestroyImmediate(this.transform.GetComponent<PerlinWormsMA>());
                break;
            case 9:
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
        CreateBackUpGrid();
        Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(gridArray2D);
    }

    public void DrawTileMap()
    {
        int iter = 0;
        ChunkCreate(chunkWidth, chunkHeight);

        if (WallsTiles.Count == 0 || CeilingTiles.Count == 0 || FloorTiles.Count == 0)
        {
            EditorUtility.DisplayDialog("Invalid tile Rules given", "Please make sure you have loaded all of the tile object correctly and all the 3 lists have at least one object in them to use this Generation method", "OK!");
            return;
        }

        for (int z = 0; z < RoomHeight; z++)  // this is the heihgt of the room
        {
            for (int y = 0; y < gridArray2D.Length; y++)
            {
                for (int x = 0; x < gridArray2D[0].Length; x++)
                {  
                    if (z == 0 || z == RoomHeight - 1) //we draw everything as this is the ceiling and the floor       THIS IS WHERE THE CEILING SHOULD BE
                    {
                        if (gridArray2D[y][x].tileType != BasicTile.TileType.VOID)
                        {
                            var objRef = Instantiate(FloorTiles.Count > 1 ? FloorTiles[RatioBasedChoice(FloorTiles)].Tile : FloorTiles[0].Tile, this.transform);
                            this.transform.GetChild(0);

                            objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                            objRef.isStatic = true;
                            objRef.transform.position = new Vector3(x, z, y);
                            iter++;
                        }
                    }

                    if (gridArray2D[y][x].tileType == BasicTile.TileType.WALL)
                    {
                        var checkVector = new Vector2Int(x, y);

                        checkVector = new Vector2Int(x + 1, y);// right check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].Tile : WallsTiles[0].Tile, this.transform);



                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 90, 0);
                            objRef.isStatic = true;
                            objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].Tile : WallsTiles[0].Tile, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 90, 0);
                                objRef.isStatic = true;
                                objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                                iter++;
                            }
                        }



                        checkVector = new Vector2Int(x - 1, y);// left check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].Tile : WallsTiles[0].Tile, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 270, 0);
                            objRef.isStatic = true;
                            objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].Tile : WallsTiles[0].Tile, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 270, 0);
                                objRef.isStatic = true;
                                objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                                iter++;

                            }
                        }




                        checkVector = new Vector2Int(x, y + 1);// above check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].Tile : WallsTiles[0].Tile, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.isStatic = true;
                            objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                            objRef.transform.Rotate(0, 0, 0);
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ?      WallsTiles[RatioBasedChoice(WallsTiles)].Tile        : WallsTiles[0].Tile, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 0, 0);
                                objRef.isStatic = true;
                                objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                                iter++;
                            }
                        }





                        checkVector = new Vector2Int(x, y - 1);// down check

                        if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArray2D[0].Length || checkVector.y >= gridArray2D.Length)
                        {
                            var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].Tile : WallsTiles[0].Tile, this.transform);

                            objRef.transform.position = new Vector3(x, z, y);
                            objRef.transform.Rotate(0, 180, 0);
                            objRef.isStatic = true;
                            objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);
                            iter++;
                        }
                        else
                        {
                            if (gridArray2D[checkVector.y][checkVector.x].tileType == BasicTile.TileType.VOID)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].Tile : WallsTiles[0].Tile, this.transform);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 180, 0);
                                objRef.isStatic = true;
                                objRef.transform.parent = this.transform.GetChild(gridArray2D[y][x].idx + 1);

                                iter++;
                            }
                        }
                    }
                }
            }
        }
    }

    private int RatioBasedChoice(List<TileRuleSetPCG> objects) 
    {
        int totRatio = 0;

        foreach (var obj in objects) 
        {
            totRatio += obj.occurance;
        }

        int ranNum = Random.Range(1, totRatio);

        int countRatio = 0;
        int savedIdx = 0;
        for (int i = 1; i < objects.Count; i++)
        {
            if (ranNum > countRatio && ranNum <= countRatio + objects[i].occurance) 
            {
                savedIdx = i;
                break;
            }

            countRatio += objects[i].occurance;
        }

        return savedIdx;
    }

    public void ChunkCreate(int height, int width)
    {
        int maxWidth = gridArray2D[0].Length;
        int maxHeight = gridArray2D.Length;


        Vector2Int BLhead = Vector2Int.zero;
        Vector2Int TRhead = Vector2Int.zero;

        int correctHeight = (maxHeight - 1) - TRhead.y >= height ? height : (maxHeight - 1) - TRhead.y;

        TRhead = new Vector2Int(0, TRhead.y + correctHeight);

        chunks = new List<Chunk>();
        while (true)
        {

            if (TRhead.x + 1 >= maxWidth)  // needs to go in the new line
            {
                if (TRhead.y + 1 >= maxHeight)  // this checks if we are dont with the algo
                {
                    break;
                }

                BLhead = new Vector2Int(0, TRhead.y);

                correctHeight = (maxHeight - 1) - TRhead.y >= height ? height : (maxHeight - 1) - TRhead.y;

                TRhead = new Vector2Int(0, TRhead.y + correctHeight + 1);

            }
            else
            {
                int correctWidth = (maxWidth - 1) - TRhead.x >= width ? width : (maxWidth - 1) - TRhead.x;

                TRhead = new Vector2Int(TRhead.x + correctWidth + 1, TRhead.y);

                chunks.Add(new Chunk() { width = correctWidth, height = correctHeight });

                var currChunk = chunks[chunks.Count - 1];
                currChunk.topRight = TRhead;
                currChunk.bottomLeft = BLhead;
                currChunk.index = chunks.Count - 1;

                BLhead = new Vector2Int(TRhead.x, BLhead.y);
            }
        }

        for (int i = 0; i < chunks.Count; i++)
        {
            var objRef = new GameObject();
            objRef.transform.parent = this.transform;
            objRef.transform.name = i.ToString();
            objRef.isStatic = true;
            chunks[i].mainParent = objRef;

            int widthChunk = chunks[i].topRight.x - chunks[i].bottomLeft.x;
            int heightChunk = chunks[i].topRight.y - chunks[i].bottomLeft.y;

            for (int y = 0; y < heightChunk; y++)
            {
                for (int x = 0; x < widthChunk; x++)
                {
                    gridArray2D[y + chunks[i].bottomLeft.y][x + chunks[i].bottomLeft.x].idx = chunks[i].index;
                }
            }

        }
    }
}


[Serializable]
public class Chunk
{
    public Vector2Int topRight = Vector2Int.zero;
    public Vector2Int bottomLeft = Vector2Int.zero;

    public int width;
    public int height;

    public int index = 0;
    public GameObject mainParent = null;
    public List<GameObject> listOfObjInChunk = new List<GameObject>();

}

[Serializable]
public class TilesRuleSetPCG
{
    public List<TileRuleSetPCG> FloorTiles = new List<TileRuleSetPCG>();
    public List<TileRuleSetPCG> CeilingTiles = new List<TileRuleSetPCG>();
    public List<TileRuleSetPCG> WallsTiles = new List<TileRuleSetPCG>();
}


[Serializable]
public class TileRuleSetPCG
{
    public GameObject Tile;
    public int occurance = 1;
}
