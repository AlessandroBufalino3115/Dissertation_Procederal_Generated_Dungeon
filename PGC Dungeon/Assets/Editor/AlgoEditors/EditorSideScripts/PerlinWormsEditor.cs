using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PerlinWormsMA))]
public class PerlinWormsEditor : Editor
{

    bool showCA = false;

    bool showPath = false;
    bool showRooms = false;

    bool showRules = false;

    bool useWeights = false;


    int selGridPathType = 0;
    GUIContent[] selStringsPathType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualr path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have manu corridors" }, new GUIContent() { text = "Prim's algo + random", tooltip = "Create a singualr path that traverses the whole dungeon, with some random diviation" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridors" } };


    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algortihm Marhcing cubes create a mesh object whihc can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tielset provided" } };

    bool started = false;

    int thicknessWorm = 2;



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PerlinWormsMA mainScript = (PerlinWormsMA)target;


        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have chosen perlin worms");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion

        GeneralUtil.SpacesUILayout(4);

        #region Main algo region

        mainScript.OffsetX = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset X", tooltip = "" }, mainScript.OffsetX, 0, 10000);
        mainScript.OffsetY = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset Y", tooltip = "" }, mainScript.OffsetY, 0, 10000);


        mainScript.Scale = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Scale", tooltip = "" }, mainScript.Scale, 3f, 35f);
        mainScript.Octaves = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Octaves", tooltip = "" }, mainScript.Octaves, 1, 8);

        mainScript.Persistance = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Persitance", tooltip = "" }, mainScript.Persistance, 0.1f, 0.9f);
        mainScript.Lacunarity = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Lacunarity", tooltip = "" }, mainScript.Lacunarity, 0.5f, 10f);

        mainScript.MinThreshold = EditorGUILayout.Slider(new GUIContent() { text = "Min Threshold", tooltip = "" }, mainScript.MinThreshold, 0.1f, 0.9f);
        mainScript.MaxThreshold = EditorGUILayout.Slider(new GUIContent() { text = "Max Threshold", tooltip = "" }, mainScript.MaxThreshold, 0.1f, 0.9f);

        thicknessWorm = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness Worm", tooltip = "" }, thicknessWorm, 2, 4);


        if (GUILayout.Button("Run worm Gen"))
        {
            mainScript.PcgManager.gridArray2D = AlgosUtils.PerlinWorms(mainScript.PcgManager.gridArray2D, mainScript.Scale, mainScript.Octaves, mainScript.Persistance, mainScript.Lacunarity, mainScript.OffsetX, mainScript.OffsetY, mainScript.MaxThreshold, mainScript.MinThreshold);


            AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);

            for (int i = 0; i < thicknessWorm - 1; i++)
            {
                for (int y = 0; y < mainScript.PcgManager.gridArray2D.Length; y++)
                {
                    for (int x = 0; x < mainScript.PcgManager.gridArray2D[0].Length; x++)
                    {
                        if (mainScript.PcgManager.gridArray2D[y][x].tileType == Tile.TileType.WALLCORRIDOR)
                        {
                            mainScript.PcgManager.gridArray2D[y][x].tileType = Tile.TileType.FLOORCORRIDOR;
                        }
                        if (mainScript.PcgManager.gridArray2D[y][x].tileType == Tile.TileType.FLOORCORRIDOR)
                        {
                        }
                    }
                }

                AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);
            }


            AlgosUtils.SetUpTileTypesFloorWall(mainScript.PcgManager.gridArray2D);

            mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);

            started = true;
        }

        #endregion

    }
}
