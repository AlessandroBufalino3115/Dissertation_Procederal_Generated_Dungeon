using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


public class EditorGV : GraphView
{

    public EditorGV() 
    {
        AddManipulators();

        AddGridBackGround();


        AddStyles();
    }


    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(CreateNodeContextualMenu());
        
        this.AddManipulator(new ContentDragger());

        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }


    private IManipulator CreateNodeContextualMenu() 
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Node", ActionEvent => AddElement(CreateNode(ActionEvent.eventInfo.localMousePosition)))
            );

        return contextualMenuManipulator;

    }

    private GraphViewNode CreateNode(Vector2 pos)
    {
        GraphViewNode node = new GraphViewNode();

        node.Initialize(pos);
        node.Draw();

        return node;
    }

    private void AddGridBackGround()
    {
        GridBackground gridBackground = new GridBackground();

        gridBackground.StretchToParentSize();

        Insert(0, gridBackground);
    }

    private void AddStyles() 
    {
        StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("UIGraphViewStyle.uss");

        styleSheets.Add(styleSheet);
    
        
    }
}
