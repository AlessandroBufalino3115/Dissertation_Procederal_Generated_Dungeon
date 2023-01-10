using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PCGManager))]
public class PCGManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PCGManager mainScript = (PCGManager)target;


        if (GUILayout.Button(mainScript.Plane == null? "Generate Plane":"Refresh Plane"))
        {
             mainScript.CreatePlane();

            mainScript.Restart();

        }

        if(mainScript.Plane != null) 
        {
            if (GUILayout.Button("Delete Plane"))
            {
                mainScript.RefreshPlane();
                mainScript.DelPrevAlgo();
            }
        }

        if (mainScript.Plane != null || mainScript.mainAlgo == PCGManager.MainAlgo.WFC)
        {
            if (GUILayout.Button(mainScript.CurrMainAlgoIDX == (int)mainScript.mainAlgo ? "Refresh Main Algo Component" : "Load New Algo Component"))
            {
                if (mainScript.mainAlgo == PCGManager.MainAlgo.WFC)
                {
                    if (mainScript.Plane != null)
                    {
                        DestroyImmediate(mainScript.Plane);
                    }
                    mainScript.LoadMainAlgo();
                }
                else 
                {

                    mainScript.Restart();
                    mainScript.LoadMainAlgo();
                }
            }
        }
    }

}


/*
 
 https://docs.unity3d.com/ScriptReference/EditorGUILayout.Space.html
https://docs.unity3d.com/ScriptReference/EditorGUI.ProgressBar.html
https://docs.unity3d.com/ScriptReference/TooltipAttribute.html

 https://docs.unity3d.com/ScriptReference/HeaderAttribute.html


https://docs.unity3d.com/Manual/editor-CustomEditors.html
https://answers.unity.com/questions/1567638/how-can-i-change-the-variables-order-in-inspector.html
 */
