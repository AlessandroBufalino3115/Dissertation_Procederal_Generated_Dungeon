using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewWFCAlog : MonoBehaviour
{

    [SerializeField]
    public WFCTile[][] arrayOfWFCTiles = new WFCTile[0][];
    private int xSize = 20;
    private int ySize = 20;

    public bool outskirtsCheck = false;
    public int indexOutskirts = 0;

    private WFCRuleDecipher rulesInst;

    private PCGManager pcgManager;
    public PCGManager PcgManager
    {
        get { return pcgManager; }
    }


    public void InspectorAwake()
    {
        pcgManager = this.transform.GetComponent<PCGManager>();
        rulesInst = this.transform.GetComponent<WFCRuleDecipher>();
    }


    public void RunWFCAlgo()
    {
        DestroyKids();

        rulesInst = this.transform.GetComponent<WFCRuleDecipher>();

        ySize = pcgManager.height;
        xSize = pcgManager.width;

        arrayOfWFCTiles = new WFCTile[ySize][];

        for (int y = 0; y < arrayOfWFCTiles.Length; y++)
        {
            arrayOfWFCTiles[y] = new WFCTile[xSize];

            for (int x = 0; x < arrayOfWFCTiles[y].Length; x++)
            {
                arrayOfWFCTiles[y][x] = new WFCTile(rulesInst.tileSet.Count());
                arrayOfWFCTiles[y][x].coord = new Vector2Int(x, y);

            }
        }


        if (outskirtsCheck)
            SetOutSkirts();
        

        var ranStart = GeneralUtil.RanVector2Int(xSize, ySize);
        arrayOfWFCTiles[ranStart.y][ranStart.x].solved = true;// choosen idx should be the indx of the item choosen

        int ranNum = Random.Range(0, arrayOfWFCTiles[ranStart.y][ranStart.x].AllowedObjectsIDXs.Count);

        arrayOfWFCTiles[ranStart.y][ranStart.x].choosenIDX = arrayOfWFCTiles[ranStart.y][ranStart.x].AllowedObjectsIDXs[ranNum];

        var spawnedAsset = Instantiate(rulesInst.tileSet[arrayOfWFCTiles[ranStart.y][ranStart.x].choosenIDX], this.transform);
        spawnedAsset.transform.position = new Vector3(ranStart.x, 0, ranStart.y);
        spawnedAsset.transform.name = $"{ranStart.x} {ranStart.y} {arrayOfWFCTiles[ranStart.y][ranStart.x].choosenIDX}";
        arrayOfWFCTiles[ranStart.y][ranStart.x].AllowedObjectsIDXs.Clear();
        ResetNeighbours(arrayOfWFCTiles, ranStart, arrayOfWFCTiles[ranStart.y][ranStart.x].choosenIDX);

        bool finishedAlgo = false;

        while (!finishedAlgo)
        {
            finishedAlgo = true;
            int lowestSuperposition = 999;
            Vector2Int coordOfLowestSuperposition = new Vector2Int(0, 0);

            for (int y = 0; y < arrayOfWFCTiles.Length; y++)
            {
                for (int x = 0; x < arrayOfWFCTiles[0].Length; x++)
                {
                    if (!arrayOfWFCTiles[y][x].solved)
                    {
                        finishedAlgo = false;
                        if (arrayOfWFCTiles[y][x].AllowedObjectsIDXs.Count < lowestSuperposition)
                        {
                            lowestSuperposition = arrayOfWFCTiles[y][x].AllowedObjectsIDXs.Count;
                            coordOfLowestSuperposition = new Vector2Int(x, y);
                        }
                    }
                }
            }

            if (!finishedAlgo)
            {
                // get the choosen idx
                if (arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].AllowedObjectsIDXs.Count == 1)
                {
                    arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].choosenIDX = arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].AllowedObjectsIDXs[0];
                }
                else
                {
                    arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].choosenIDX = arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].AllowedObjectsIDXs[Random.Range(0, arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].AllowedObjectsIDXs.Count)];
                }

                spawnedAsset = Instantiate(rulesInst.tileSet[arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].choosenIDX], this.transform);
                spawnedAsset.transform.position = new Vector3(coordOfLowestSuperposition.x, 0, coordOfLowestSuperposition.y);
                spawnedAsset.transform.name = $"{coordOfLowestSuperposition.x} {coordOfLowestSuperposition.y} {arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].choosenIDX}";
                arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].AllowedObjectsIDXs.Clear();
                ResetNeighbours(arrayOfWFCTiles, coordOfLowestSuperposition, arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].choosenIDX);

                arrayOfWFCTiles[coordOfLowestSuperposition.y][coordOfLowestSuperposition.x].solved = true;
            }
        }
    }





    private void SetOutSkirts()
    {
        WFCTileRule ruleRef = null;

        foreach (var rule in rulesInst.ruleSet)
        {
            if (rule.assetIdx == indexOutskirts)
            {
                ruleRef = rule;
                break;
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            arrayOfWFCTiles[0][x].NeighbourAllowed(ruleRef.allowedObjAbove);
        }

        for (int x = 0; x < xSize ; x++)
        {
            arrayOfWFCTiles[ySize-1][x].NeighbourAllowed(ruleRef.allowedObjBelow);
        }

        for (int y = 0; y < ySize; y++)
        {
            arrayOfWFCTiles[y][0].NeighbourAllowed(ruleRef.allowedObjRight);
        }

        for (int y = 0; y < ySize; y++)
        {
            arrayOfWFCTiles[y][xSize-1].NeighbourAllowed(ruleRef.allowedObjLeft);
        }
    }



    public void ResetNeighbours(WFCTile[][] arrayOfWFCTiles, Vector2Int mainTileCoord, int mainTileIDX)
    {

        WFCTileRule ruleRef = null;

        foreach (var rule in rulesInst.ruleSet)
        {
            if (rule.assetIdx == mainTileIDX)
            {
                ruleRef = rule;
                break;
            }
        }


        if (mainTileCoord.x + 1 < xSize)   // the one on the right  exists
        {
            if (!arrayOfWFCTiles[mainTileCoord.y][mainTileCoord.x + 1].solved)
                arrayOfWFCTiles[mainTileCoord.y][mainTileCoord.x + 1].NeighbourAllowed(ruleRef.allowedObjRight);
        }

        if (mainTileCoord.x - 1 >= 0)   // the one on the left  exists
        {
            if (!arrayOfWFCTiles[mainTileCoord.y][mainTileCoord.x - 1].solved)
                arrayOfWFCTiles[mainTileCoord.y][mainTileCoord.x - 1].NeighbourAllowed(ruleRef.allowedObjLeft);
        }

        if (mainTileCoord.y + 1 < ySize)   // the one on the right  exists
        {
            if (!arrayOfWFCTiles[mainTileCoord.y + 1][mainTileCoord.x].solved)
                arrayOfWFCTiles[mainTileCoord.y + 1][mainTileCoord.x].NeighbourAllowed(ruleRef.allowedObjAbove);
        }

        if (mainTileCoord.y - 1 >= 0)   // the one on the right  exists
        {
            if (!arrayOfWFCTiles[mainTileCoord.y - 1][mainTileCoord.x].solved)
                arrayOfWFCTiles[mainTileCoord.y - 1][mainTileCoord.x].NeighbourAllowed(ruleRef.allowedObjBelow);
        }

    }



    public void DestroyKids()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public class WFCTile
    {
        public Vector2Int coord;
        public bool solved = false;
        public int choosenIDX = 9999;
        public List<int> AllowedObjectsIDXs = new List<int>();


        public WFCTile(int num)
        {
            for (int i = 0; i < num; i++)
            {
                AllowedObjectsIDXs.Add(i);
            }
        }

        public void NeighbourAllowed(List<int> neighbourAllowedIDXs)
        {
            for (int i = AllowedObjectsIDXs.Count; i-- > 0;)
            {
                bool isThere = false;

                foreach (var allowedIDX in neighbourAllowedIDXs)
                {
                    if (AllowedObjectsIDXs[i] == allowedIDX)
                    {
                        isThere = true;
                        break;
                    }
                }


                if (!isThere)
                {
                    AllowedObjectsIDXs.RemoveAt(i);
                }

            }


            if (AllowedObjectsIDXs.Count == 0)
            {
                AllowedObjectsIDXs.Add(0);
            }
        }
    }
}






