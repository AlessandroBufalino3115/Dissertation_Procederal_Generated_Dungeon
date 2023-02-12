using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

public class WFCRuleDecipher : MonoBehaviour
{
    public GameObject[] tileSet = new GameObject[0];

    [SerializeField]
    public List<WFCTileRule> ruleSet = new List<WFCTileRule>();

    public string ruleSetFileName = "";
    public string tileSetFileName = "";
}


[Serializable]
public class WFCTileRule
{
    public GameObject mainAsset;

    public int assetIdx;

    public List<int> allowedObjLeft = new List<int>();
    public List<int> allowedObjRight = new List<int>();
    public List<int> allowedObjAbove = new List<int>();
    public List<int> allowedObjBelow = new List<int>();
}





