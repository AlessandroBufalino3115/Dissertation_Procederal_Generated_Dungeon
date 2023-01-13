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


        if (GUILayout.Button(mainScript.Plane == null? "Generate Plane":"Refresh Plane"))
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








        if (GUILayout.Button("New tileSet rule"))
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


        if (GUILayout.Button("Load tileSet rule"))
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
