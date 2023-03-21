

namespace DungeonForge.Editor
{
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;
    using static DungeonForge.AlgoScript.PCGManager;

    [CustomEditor(typeof(LoadMapMA))]
    public class LoadMapEditor : Editor
    {

        public bool showRules = false;


        int selGridGenType = 0;
        GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

        bool succesfullLoading = false;
        bool blockGeneration = false;

        float keepPercentage = 0.2f;
        float radiusPoissant = 1f;

        GameObject heightTester;

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


            DFEditorUtil.SpacesUILayout(4);

            mainScript.fileName = EditorGUILayout.TextField("Save file name: ", mainScript.fileName);

            if (GUILayout.Button(new GUIContent() { text = "load Data" }))
            {
                succesfullLoading = false;
                var map = mainScript.LoadDataCall(mainScript.fileName);


                if (map == null) { }
                else
                {
                    succesfullLoading = true;
                    mainScript.PcgManager.gridArr = map;
                    mainScript.PcgManager.height = mainScript.PcgManager.gridArr.GetLength(1);
                    mainScript.PcgManager.width = mainScript.PcgManager.gridArr.GetLength(0);

                    mainScript.PcgManager.CreatePlane();

                    mainScript.PcgManager.gridArr = map;

                    //mainScript.rooms.Clear();

                    //mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.PcgManager.gridArr, true);
                    //Debug.Log(mainScript.rooms.Count);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArr, 0, 1, true);
                }
            }


            DFEditorUtil.SpacesUILayout(4);


            EditorGUI.BeginDisabledGroup(succesfullLoading == false);


            GUILayout.BeginVertical("Box");
            selGridGenType = GUILayout.SelectionGrid(selGridGenType, selStringsGenType, 1);
            GUILayout.EndVertical();

            DFEditorUtil.SpacesUILayout(2);

            if (GUILayout.Button(new GUIContent() { text = "Generate YOUR Dungeon!" }))
            {
                mainScript.generatedMap = true;

                switch (selGridGenType)
                {
                    case 0:

                        for (int y = 0; y < mainScript.PcgManager.gridArr.GetLength(1); y++)
                        {
                            for (int x = 0; x < mainScript.PcgManager.gridArr.GetLength(0); x++)
                            {
                                if (mainScript.PcgManager.gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                                {
                                    mainScript.PcgManager.gridArr[x, y].tileType = DFTile.TileType.FLOORCORRIDOR;
                                }
                            }
                        }

                        DFAlgoBank.SetUpTileTypesCorridor(mainScript.PcgManager.gridArr);

                        mainScript.PcgManager.FormObject(DFAlgoBank.MarchingCubesAlgo(DFAlgoBank.ExtrapolateMarchingCubes(mainScript.PcgManager.gridArr, mainScript.PcgManager.RoomHeight), false));
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
                DFEditorUtil.SpacesUILayout(1);
                mainScript.PcgManager.ChunkHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "This is for the chunk size", tooltip = "" }, mainScript.PcgManager.ChunkHeight, 10, 30);
                mainScript.PcgManager.ChunkWidth = mainScript.PcgManager.ChunkHeight;
            }



            DFEditorUtil.SpacesUILayout(4);

            EditorGUI.BeginDisabledGroup(mainScript.generatedMap == false);


            if (GUILayout.Button(new GUIContent() { text = "Generate Poissant preview height object" }))
            {
                if (heightTester != null)
                    DestroyImmediate(heightTester);

                Vector3 middleOfMap = new Vector3(0f, 0f, 0f);

                heightTester = new GameObject("HeightTesterForPoissant");

                heightTester.transform.position = middleOfMap;
            }

            keepPercentage = EditorGUILayout.Slider(new GUIContent() { text = "keep percentage", tooltip = "" }, keepPercentage, 0f, 1f);
            radiusPoissant = EditorGUILayout.Slider(new GUIContent() { text = "radius Poissant", tooltip = "" }, radiusPoissant, 0.5f, 10f);

            if (GUILayout.Button(new GUIContent() { text = "Generate Poissant Objects" }))
            {

                var poissant = DFAlgoBank.GeneratePossiantPoints(mainScript.PcgManager.gridArr.GetLength(0), mainScript.PcgManager.gridArr.GetLength(1), radiusPoissant);

                var acceptedPointed = DFAlgoBank.RunPoissantCheckOnCurrentTileMap(poissant, mainScript.PcgManager.gridArr, keepPercentage);

                for (int i = 0; i < acceptedPointed.Count; i++)
                {
                    int collidedIndex = -1;
                    var objRef = Instantiate(mainScript.mapRandomObjects.Count == 1 ? mainScript.mapRandomObjects[0].objectPrefab : mainScript.mapRandomObjects[mainScript.PcgManager.RatioBasedChoice(mainScript.mapRandomObjects)].objectPrefab, new Vector3(acceptedPointed[i].x, heightTester == null ? 0 : heightTester.transform.position.y, acceptedPointed[i].y), Quaternion.identity);

                    for (int j = 0; j < mainScript.PcgManager.chunks.Count; j++)
                    {
                        if (mainScript.PcgManager.AABBCol(objRef.transform.position, mainScript.PcgManager.chunks[j]))
                        {
                            collidedIndex = j;
                            break;
                        }
                    }

                    if (collidedIndex == -1)
                    {
                        Debug.Log($"no index found");
                        DestroyImmediate(objRef);
                    }
                    else
                    {
                        objRef.transform.parent = mainScript.PcgManager.chunkObjs[collidedIndex];
                    }
                }
            }

            EditorGUI.EndDisabledGroup();


            EditorGUI.EndDisabledGroup();
        }
    }
}