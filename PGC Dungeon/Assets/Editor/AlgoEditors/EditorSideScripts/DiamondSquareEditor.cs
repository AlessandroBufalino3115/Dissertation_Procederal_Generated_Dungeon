using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




[CustomEditor(typeof(DiamondSquareMA))]
public class DiamondSquareEditor : Editor
{

    bool showRules = false;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DiamondSquareMA mainScript = (DiamondSquareMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have chosen diamond square");

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
