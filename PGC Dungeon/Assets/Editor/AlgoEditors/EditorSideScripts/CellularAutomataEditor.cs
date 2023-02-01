using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;





[CustomEditor(typeof(CellularAutomataMA))]
public class CellularAutomataEditor : Editor
{
    bool showRules = false;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CellularAutomataMA mainScript = (CellularAutomataMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have chosen CA");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion


        GeneralUtil.SpacesUILayout(4);
    }
}
