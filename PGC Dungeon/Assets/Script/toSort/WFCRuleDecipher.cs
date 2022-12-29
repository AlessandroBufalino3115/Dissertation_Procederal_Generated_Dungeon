using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

public class WFCRuleDecipher : MonoBehaviour
{


    [SerializeField]
    public List<WFCTileRule> ruleSet = new List<WFCTileRule>();
    public string ruleSetFolderName = "";
    public string tileSetFolderName = "";



    public static WFCRuleDecipher instance;

    private void Awake()
    {
        instance = this; 
    }



}


[Serializable]
public class WFCTileRule
{
    public GameObject mainAsset;

    public int assetIdx;

    public List<int> allowedObjLeft = new List<int>();
    public List<int> allowedObjRight = new List<int>();
    public List<int> allowedObjForward = new List<int>();
    public List<int> allowedObjBackwards = new List<int>();
}





