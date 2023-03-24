

namespace DungeonForge.Editor
{
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;
    using static DungeonForge.AlgoScript.PCGManager;
    using System.Collections.Generic;
    using Codice.CM.WorkspaceServer.DataStore.Configuration;

    [CustomEditor(typeof(LoadMapMA))]
    public class LoadMapEditor : Editor
    {

        public bool showRules = false;

        public bool overWriteFloor = false;

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

            switch (mainScript.state)
            {


                case LoadMapMA.UI_STATE.PICKING_FILE:
                    {

                        mainScript.allowedBack = false;

                        mainScript.singleStringSelected = EditorGUILayout.Toggle("Single File", mainScript.singleStringSelected);

                        if (mainScript.singleStringSelected)
                        {
                            EditorGUILayout.LabelField("Single String Input", EditorStyles.boldLabel);
                            mainScript.fileName = EditorGUILayout.TextField("File Name", mainScript.fileName);
                        }
                        else
                        {
                            overWriteFloor = EditorGUILayout.Toggle("over Write Walls", overWriteFloor);

                            EditorGUILayout.LabelField("List of file Names", EditorStyles.boldLabel);
                            mainScript.stringListSize = EditorGUILayout.IntField("How many files", mainScript.stringListSize);

                            if (mainScript.stringListSize < 0)
                            {
                                mainScript.stringListSize = 0;
                            }

                            while (mainScript.stringList.Count < mainScript.stringListSize)
                            {
                                mainScript.stringList.Add("");
                            }

                            while (mainScript.stringList.Count > mainScript.stringListSize)
                            {
                                mainScript.stringList.RemoveAt(mainScript.stringList.Count - 1);
                            }

                            for (int i = 0; i < mainScript.stringList.Count; i++)
                            {
                                mainScript.stringList[i] = EditorGUILayout.TextField("List of file Names " + (i + 1), mainScript.stringList[i]);
                            }
                        }


                        DFEditorUtil.SpacesUILayout(4);


                        if (GUILayout.Button(new GUIContent() { text = "load Data" }))
                        {
                            if (mainScript.singleStringSelected)
                            {
                                succesfullLoading = false;
                                var map = mainScript.LoadDataCall(mainScript.fileName);

                                if (map == null) { }
                                else
                                {
                                    succesfullLoading = true;
                                    mainScript.pcgManager.gridArr = map;
                                    mainScript.pcgManager.height = mainScript.pcgManager.gridArr.GetLength(1);
                                    mainScript.pcgManager.width = mainScript.pcgManager.gridArr.GetLength(0);

                                    mainScript.pcgManager.CreatePlane();

                                    mainScript.pcgManager.gridArr = map;

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                    mainScript.allowedForward = true;
                                }
                            }
                            else
                            {
                                succesfullLoading = true;

                                int biggestHeight = 0;
                                int biggestWidth = 0;

                                for (int i = 0; i < mainScript.stringList.Count; i++)
                                {
                                    var map = mainScript.LoadDataCall(mainScript.stringList[i]);

                                    if (map == null)
                                    {
                                        Debug.Log($"The file name {mainScript.stringList[i]} doesnt exist");
                                        succesfullLoading = false;
                                    }
                                    else
                                    {
                                        if (map.GetLength(1) > biggestHeight)
                                            biggestHeight = map.GetLength(1);

                                        if (map.GetLength(0) > biggestWidth)
                                            biggestWidth = map.GetLength(0);
                                    }
                                }

                                if (succesfullLoading)
                                {
                                    mainScript.allowedForward = true;

                                    mainScript.pcgManager.width = biggestWidth;
                                    mainScript.pcgManager.height = biggestHeight;
                                    mainScript.pcgManager.CreatePlane();

                                    for (int i = 0; i < mainScript.stringList.Count; i++)
                                    {
                                        var map = mainScript.LoadDataCall(mainScript.stringList[i]);

                                        mainScript.AddOnGridData(map, overWriteFloor);
                                    }

                                    mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                                }
                            }
                        }

                        break;
                    }

                case LoadMapMA.UI_STATE.SELF_EDITING:
                    {

                        mainScript.allowedForward = true;
                        mainScript.allowedBack = true;

                        if (GUILayout.Button(new GUIContent() { text = "Up" }))
                        {
                            if (mainScript.pointerPosition.y + 1 < mainScript.pcgManager.gridArr.GetLength(1))
                                mainScript.pointerPosition = new Vector2Int(mainScript.pointerPosition.x, mainScript.pointerPosition.y + 1);
                        }
                        if (GUILayout.Button(new GUIContent() { text = "Down" }))
                        {
                            if (mainScript.pointerPosition.y - 1 < 0)
                                mainScript.pointerPosition = new Vector2Int(mainScript.pointerPosition.x, mainScript.pointerPosition.y - 1);
                        }
                        if (GUILayout.Button(new GUIContent() { text = "Left" }))
                        {
                            if (mainScript.pointerPosition.x - 1 < 0)
                                mainScript.pointerPosition = new Vector2Int(mainScript.pointerPosition.x - 1, mainScript.pointerPosition.y);
                        }
                        if (GUILayout.Button(new GUIContent() { text = "Right" }))
                        {
                            if (mainScript.pointerPosition.x + 1 < mainScript.pcgManager.gridArr.GetLength(0))
                                mainScript.pointerPosition = new Vector2Int(mainScript.pointerPosition.x + 1, mainScript.pointerPosition.y);
                        }

                        DFEditorUtil.SpacesUILayout(2);

                        if (GUILayout.Button(new GUIContent() { text = "Void" }))
                        {
                            DFGeneralUtil.ResetTile(mainScript.pcgManager.gridArr[mainScript.pointerPosition.x, mainScript.pointerPosition.y]);

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }
                        if (GUILayout.Button(new GUIContent() { text = "Wall" }))
                        {
                            mainScript.pcgManager.gridArr[mainScript.pointerPosition.x, mainScript.pointerPosition.y].tileType = DFTile.TileType.WALL;
                            mainScript.pcgManager.gridArr[mainScript.pointerPosition.x, mainScript.pointerPosition.y].tileWeight = 1;

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }
                        if (GUILayout.Button(new GUIContent() { text = "Floor" }))
                        {
                            mainScript.pcgManager.gridArr[mainScript.pointerPosition.x, mainScript.pointerPosition.y].tileType = DFTile.TileType.FLOORROOM;
                            mainScript.pcgManager.gridArr[mainScript.pointerPosition.x, mainScript.pointerPosition.y].tileWeight = 0.75f;

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }


                        break;
                    }

                case LoadMapMA.UI_STATE.GENERATE:
                    {
                        mainScript.allowedBack = false;
                        mainScript.allowedForward = false;

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

                                    for (int y = 0; y < mainScript.pcgManager.gridArr.GetLength(1); y++)
                                    {
                                        for (int x = 0; x < mainScript.pcgManager.gridArr.GetLength(0); x++)
                                        {
                                            if (mainScript.pcgManager.gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                                            {
                                                mainScript.pcgManager.gridArr[x, y].tileType = DFTile.TileType.FLOORCORRIDOR;
                                            }
                                        }
                                    }

                                    DFAlgoBank.SetUpTileTypesCorridor(mainScript.pcgManager.gridArr);

                                    mainScript.pcgManager.FormObject(DFAlgoBank.MarchingCubesAlgo(DFAlgoBank.ExtrapolateMarchingCubes(mainScript.pcgManager.gridArr, mainScript.pcgManager.RoomHeight), false));
                                    break;

                                case 1:

                                    if (blockGeneration)
                                        mainScript.pcgManager.DrawTileMapBlockType();
                                    else
                                        mainScript.pcgManager.DrawTileMapDirectionalWalls();

                                    break;
                            }
                        }

                        if (selGridGenType == 1)
                        {
                            blockGeneration = EditorGUILayout.Toggle(new GUIContent() { text = blockGeneration == true ? "Block gen selected" : "Wall directional gen selected", tooltip = "" }, blockGeneration);
                            DFEditorUtil.SpacesUILayout(1);
                            mainScript.pcgManager.ChunkHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "This is for the chunk size", tooltip = "" }, mainScript.pcgManager.ChunkHeight, 10, 30);
                            mainScript.pcgManager.ChunkWidth = mainScript.pcgManager.ChunkHeight;
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

                            var poissant = DFAlgoBank.GeneratePossiantPoints(mainScript.pcgManager.gridArr.GetLength(0), mainScript.pcgManager.gridArr.GetLength(1), radiusPoissant);

                            var acceptedPointed = DFAlgoBank.RunPoissantCheckOnCurrentTileMap(poissant, mainScript.pcgManager.gridArr, keepPercentage);

                            for (int i = 0; i < acceptedPointed.Count; i++)
                            {
                                int collidedIndex = -1;
                                var objRef = Instantiate(mainScript.mapRandomObjects.Count == 1 ? mainScript.mapRandomObjects[0].objectPrefab : mainScript.mapRandomObjects[mainScript.pcgManager.RatioBasedChoice(mainScript.mapRandomObjects)].objectPrefab, new Vector3(acceptedPointed[i].x, heightTester == null ? 0 : heightTester.transform.position.y, acceptedPointed[i].y), Quaternion.identity);

                                for (int j = 0; j < mainScript.pcgManager.chunks.Count; j++)
                                {
                                    if (mainScript.pcgManager.AABBCol(objRef.transform.position, mainScript.pcgManager.chunks[j]))
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
                                    objRef.transform.parent = mainScript.pcgManager.chunkObjs[collidedIndex];
                                }
                            }
                        }

                        EditorGUI.EndDisabledGroup();

                        break;
                    }
                default:
                    break;
            }


            DFEditorUtil.SpacesUILayout(4);

            if (mainScript.state != LoadMapMA.UI_STATE.GENERATE)
            {

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" })) // gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedBack = false;
                    mainScript.currStateIndex--;
                    mainScript.state = (LoadMapMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();


                EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

                if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
                {
                    if (mainScript.state == LoadMapMA.UI_STATE.PICKING_FILE) 
                    {
                        mainScript.pointerPosition = new Vector2Int(mainScript.pcgManager.gridArr.GetLength(0)/2, mainScript.pcgManager.gridArr.GetLength(1)/2);    
                    }

                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.state = (LoadMapMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();
            }

        }
    }
}