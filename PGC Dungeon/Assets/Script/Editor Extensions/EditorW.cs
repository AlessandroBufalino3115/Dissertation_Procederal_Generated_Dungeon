using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class EditorW : EditorWindow
{
    [MenuItem("Window/Test Graph")]
    public static void Open()
    {
        GetWindow<EditorW>("Test Graph");
    }


    private void CreateGUI()
    {
        AddGraphView();

        AddStyles();
    }

    private void AddStyles()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("GraphViewVariables.uss");

        rootVisualElement.styleSheets.Add(styleSheet);
    }

    private void AddGraphView() 
    {
        EditorGV graphView = new EditorGV();

        graphView.StretchToParentSize();

        rootVisualElement.Add(graphView);


    }
}
