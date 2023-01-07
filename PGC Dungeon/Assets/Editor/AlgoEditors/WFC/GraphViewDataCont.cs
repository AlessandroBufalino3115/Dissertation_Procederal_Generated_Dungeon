using DS.Enumerations;
using DS.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GraphViewDataCont : ScriptableObject
{
    public List<NodeData> nodeData = new List<NodeData>();
    public List<NodeLinkData> nodeLinkData = new List<NodeLinkData>();
}

