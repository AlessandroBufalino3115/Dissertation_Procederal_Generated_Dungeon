using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(LoadMapMA))]
public class LoadMapEditor : Editor
{

    public bool showRules = false;

    public string fileName = "";

    int selGridGenType = 0;
    GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

    bool succesfullLoading = false;
    bool blockGeneration = false;





    // this is where i wish the more design positioning thing oriented design goes but it will be tough


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LoadMapMA mainScript = (LoadMapMA)target;
        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("");
        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion




        GeneralUtil.SpacesUILayout(4);

        fileName = EditorGUILayout.TextField("Save file name: ", fileName);

        if (GUILayout.Button(new GUIContent() { text = "load Data" }))
        {
            succesfullLoading = false;
            var map = mainScript.LoadDataCall(fileName);


            if (map == null) { }
            else
            {
                succesfullLoading = true;
                mainScript.PcgManager.gridArray2D = map;
                mainScript.PcgManager.height = mainScript.PcgManager.gridArray2D.Length;
                mainScript.PcgManager.width = mainScript.PcgManager.gridArray2D[0].Length;

                mainScript.PcgManager.CreatePlane();

                mainScript.PcgManager.gridArray2D = map;

                mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArray2D, 0, 1, true);
            }

        }


        GeneralUtil.SpacesUILayout(4);


        EditorGUI.BeginDisabledGroup(succesfullLoading == false);


        GUILayout.BeginVertical("Box");
        selGridGenType = GUILayout.SelectionGrid(selGridGenType, selStringsGenType, 1);
        GUILayout.EndVertical();

        GeneralUtil.SpacesUILayout(2);

        if (GUILayout.Button(new GUIContent() { text = "Generate YOUR Dungeon!" }))
        {
            switch (selGridGenType)
            {
                case 0:

                    for (int y = 0; y < mainScript.PcgManager.gridArray2D.Length; y++)
                    {
                        for (int x = 0; x < mainScript.PcgManager.gridArray2D[0].Length; x++)
                        {
                            if (mainScript.PcgManager.gridArray2D[y][x].tileType == Tile.TileType.WALLCORRIDOR)
                            {
                                mainScript.PcgManager.gridArray2D[y][x].tileType = Tile.TileType.FLOORCORRIDOR;
                            }
                        }
                    }

                    AlgosUtils.SetUpTileTypesCorridor(mainScript.PcgManager.gridArray2D);

                    mainScript.PcgManager.FormObject(AlgosUtils.MarchingCubesAlgo(AlgosUtils.ExtrapolateMarchingCubes(mainScript.PcgManager.gridArray2D, mainScript.PcgManager.RoomHeight), false));
                    break;

                case 1:

                    if (blockGeneration)
                        mainScript.PcgManager.DrawTileMapBlockType();
                    else
                        mainScript.PcgManager.DrawTileMapDirectionalWalls();

                    break;
            }
        }

        if (selGridGenType == 1)
        {
            blockGeneration = EditorGUILayout.Toggle(new GUIContent() { text = blockGeneration == true ? "Block gen selected" : "Wall directional gen selected", tooltip = "" }, blockGeneration);
            GeneralUtil.SpacesUILayout(1);
            mainScript.PcgManager.ChunkHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "This is for the chunk height", tooltip = "" }, mainScript.PcgManager.ChunkHeight, 10, 40);
            mainScript.PcgManager.ChunkWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "This is for the chunk width", tooltip = "" }, mainScript.PcgManager.ChunkWidth, 10, 40);
        }


        EditorGUI.EndDisabledGroup();





















    }

}
