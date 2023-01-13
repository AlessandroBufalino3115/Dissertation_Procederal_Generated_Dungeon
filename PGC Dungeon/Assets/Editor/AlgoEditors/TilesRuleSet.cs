using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class TilesRuleSet : ScriptableObject
{
    public List<GameObject> FloorTiles = new List<GameObject>();
    public List<GameObject> CeilingTiles = new List<GameObject>();
    public List<GameObject> WallsTiles = new List<GameObject>();
}
