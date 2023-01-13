using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(NewWFCAlog))]
public class WFCAlgoEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NewWFCAlog mainScript = (NewWFCAlog)target;


        if (GUILayout.Button("Run WFC Algo"))
        {
            mainScript.RunWFCAlgo();
        }

        if (GUILayout.Button("Delete previous run"))
        {
            mainScript.DestroyKids();
        }





    }
}
