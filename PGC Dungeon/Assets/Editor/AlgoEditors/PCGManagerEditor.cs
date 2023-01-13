using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PCGManager))]
public class PCGManagerEditor : Editor
{


    [MenuItem("PCG Algorithms/Main Algo")]
    static void SpawnObject() 
    {
        var objToSpawn = new GameObject("Cool GameObject made from Code");
        objToSpawn.transform.name = "PCG element";
        objToSpawn.AddComponent<PCGManager>();
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PCGManager mainScript = (PCGManager)target;


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        GUILayout.TextArea("Welcome to the PCG tool, Use the sliders to set the canvas from which the dungeon will rise from\n\n" +
            "Before starting, you can load the tiles that will be used for the generation by creating a new rule and loading that rule\n\n" +
            "Then choose the starting main algorithm which will shape your dungeon");



        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        if (GUILayout.Button(new GUIContent() { text = mainScript.Plane == null ? "Generate Plane" : "Refresh Plane", tooltip = mainScript.Plane == null ? "Generate The canvas where the PCG will be reinprinted" : "Restart the Canvas" }))
        {

            if (mainScript.mainAlgo == PCGManager.MainAlgo.WFC)
            {
            }
            else
            {
                mainScript.CreatePlane();
                mainScript.Restart();
            }
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
            if (GUILayout.Button(new GUIContent() { text = mainScript.CurrMainAlgoIDX == (int)mainScript.mainAlgo ? "Refresh Main Algorithm Component" : "Load New Algorithm Component", tooltip = mainScript.CurrMainAlgoIDX == (int)mainScript.mainAlgo ? "Refresh the algorithm component" : "Load the choosen algorithm component to start to use it" }))
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



        if (GUILayout.Button(new GUIContent() { text = "New tileSet rule", tooltip = "create a new scriptable object for the rules of the tiles that you want to use"}))
        {

            var GVcont = ScriptableObject.CreateInstance<TilesRuleSet>();

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Tile_Sets_Ruleset"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Tile_Sets_Ruleset");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(GVcont, $"Assets/Resources/Tile_Sets_Ruleset/{mainScript.TileSetRuleFileName}.asset");
            AssetDatabase.SaveAssets();

        }




        if (GUILayout.Button(new GUIContent() { text = "Load tileSet rule", tooltip = "Remember to give the filename" }))
        {

            var tileRules = Resources.Load<TilesRuleSet>("Tile_Sets_Ruleset/" + mainScript.TileSetRuleFileName);

            mainScript.WallsTiles.Clear();
            mainScript.FloorTiles.Clear();
            mainScript.CeilingTiles.Clear();


            foreach (var item in tileRules.WallsTiles)
            {
                mainScript.WallsTiles.Add(item);
            }



            foreach (var item in tileRules.FloorTiles)
            {
                mainScript.FloorTiles.Add(item);
            }


            foreach (var item in tileRules.CeilingTiles)
            {
                mainScript.CeilingTiles.Add(item);
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
