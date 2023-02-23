

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(WFCRuleDecipher))]
public class WFCRuleDecipherEditor : Editor
{

    bool showRules;

    private const int MAX_TEXTURE_SIZE = 64;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        GeneralUtil.SpacesUILayout(4);


        WFCRuleDecipher ruleDec = (WFCRuleDecipher)target;

        GeneralUtil.SpacesUILayout(3);

        #region explanation

        showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

        if (showRules)
        {
            GUILayout.TextArea("You have choosen The Wave Function algorithm as your algorithm\n\nExplenation: \n\nStep 1: Create a folder, in the Resource standard folder of unity, and insert all the tile objects required in there. Input the folder name in the Tile Set file name variable and then load the tileSet" +
                "\n\nStep 2: Create the RuleSet using the GraphView provided and input the name of the ruleSet in the variable space. Load the ruleSet" +
                "\n\nStep 3: Decide if to have tiles in the outskirts of the map to, if so tick the box and give the index of the tile that is in the outskirt but looking at the tile set array" +
                "\n\nStep 4: Run the WFC algo");

        }

        if (!Selection.activeTransform)
        {
            showRules = false;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        #endregion

        GeneralUtil.SpacesUILayout(3);

        ruleDec.tileSetFileName = EditorGUILayout.TextField(new GUIContent() { text = "TileSet Objects File Name: " }, ruleDec.tileSetFileName); 
        if (GUILayout.Button("Load TileSet"))
        {
            var namesList = new List<string>();


            var fileName = "Assets/Resources/" + ruleDec.tileSetFileName;

            var info = new DirectoryInfo(fileName);
            var fileInfo = info.GetFiles();


            var currIdx = 0;

            foreach (var file in fileInfo)
            {
                if (file.Name.Contains("meta"))
                {
                    continue;
                }

                int index = file.Name.IndexOf(".");
                var manipString = file.Name.Substring(0, index);

                namesList.Add(ruleDec.tileSetFileName + "/" + manipString);

                currIdx++;
            }

            ruleDec.tileSet = new GameObject[namesList.Count];
            for (int i = 0; i < ruleDec.tileSet.Length; i++)
            {
                ruleDec.tileSet[i] = Resources.Load(namesList[i]) as GameObject;
            }

        }
        GeneralUtil.SpacesUILayout(1);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("tileSet"));
        GeneralUtil.SpacesUILayout(3);


        ruleDec.useText = EditorGUILayout.Toggle(new GUIContent() { text = "use texture" }, ruleDec.useText);
        GeneralUtil.SpacesUILayout(2);
        if (ruleDec.useText)
        {
            ruleDec.texture = (Texture2D)EditorGUILayout.ObjectField("Texture2D", ruleDec.texture, typeof(Texture2D), false);

            if (GUILayout.Button("translate Texture"))
            {
                if (ruleDec.texture == null)
                {
                    EditorGUILayout.HelpBox("There is no texture", MessageType.Error);
                }

                if (ruleDec.texture != null && (ruleDec.texture.width > MAX_TEXTURE_SIZE || ruleDec.texture.height > MAX_TEXTURE_SIZE))
                {
                    EditorGUILayout.HelpBox("The selected texture is too large. Please select a texture that is " + MAX_TEXTURE_SIZE + "x" + MAX_TEXTURE_SIZE + " or smaller.", MessageType.Error);
                }

                ruleDec.colToIntList.Clear();

                for (int x = 0; x < ruleDec.texture.width; x++)
                {
                    for (int y = 0; y < ruleDec.texture.height; y++)
                    {
                        Color color = ruleDec.texture.GetPixel(x, y);

                        if (color.a != 1) 
                        {
                            continue;
                        }

                        bool present = false;
                        Debug.Log(color);
                        for (int i = 0; i < ruleDec.colToIntList.Count; i++)
                        {
                            if (ruleDec.colToIntList[i].pixelColor == color) 
                            {
                                present = true;
                                break;
                            }
                        }

                        if (present == false) 
                        {
                            ruleDec.colToIntList.Add(new TextColToObj() { pixelColor = color });
                        }
                    }
                }
            }
            

            //if (ruleDec.colToIntList.Count > 0) 
            //{
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("colToIntList"));
            //}
        }
        else 
        {
            //ruleDec.ruleSetFileName = EditorGUILayout.TextField(new GUIContent() { text = "RulesSet Scriptable Object File Name: " }, ruleDec.ruleSetFileName);

            if (GUILayout.Button("Load Rule Set"))
            {
                IDictionary<int, string> dictNameIdx = new Dictionary<int, string>();

                var fileName = "Assets/Resources/" + ruleDec.tileSetFileName;

                var info = new DirectoryInfo(fileName);
                var fileInfo = info.GetFiles();


                var currIdx = 0;

                foreach (var file in fileInfo)
                {
                    if (file.Name.Contains("meta"))
                    {
                        continue;
                    }

                    int index = file.Name.IndexOf(".");
                    var manipString = file.Name.Substring(0, index);

                    dictNameIdx.Add(currIdx, ruleDec.tileSetFileName + "/" + manipString);

                    currIdx++;
                }

                ruleDec.ruleSet.Clear();

                var graphViewCont = Resources.Load<GraphViewDataCont>("Resources_Algorithms/WFC_Rule_Sets/" + ruleDec.ruleSetFileName);

                foreach (var node in graphViewCont.nodeData)   // this creates all the rules 
                {
                    if (node.dialogueType == DS.Enumerations.DSDialogueType.MultiChoice)
                    {
                        int idx = int.Parse(node.IndexTile);

                        bool present = false;

                        foreach (var rule in ruleDec.ruleSet)
                        {
                            if (rule.assetIdx == idx)
                            {
                                present = true;
                                break;
                            }
                        }

                        if (!present)
                        {
                            ruleDec.ruleSet.Add(new WFCTileRule() { assetIdx = idx, mainAsset = ruleDec.tileSet[idx] });
                        }
                    }
                }


                foreach (var edge in graphViewCont.nodeLinkData)
                {
                    NodeData inputNode = null;
                    NodeData outputNode = null;

                    foreach (var node in graphViewCont.nodeData)
                    {
                        if (node.nodeGuid == edge.TargetNodeGuid)
                        {
                            inputNode = node;
                        }
                        else if (node.nodeGuid == edge.BaseNodeGuid)
                        {

                            outputNode = node;
                        }
                    }

                    foreach (var rule in ruleDec.ruleSet)
                    {

                        bool added = false;
                        if (rule.assetIdx == int.Parse(inputNode.IndexTile))
                        {
                            int idxToAdd = int.Parse(outputNode.IndexTile);

                            switch (edge.PortName)
                            {

                                case "Left Side":
                                    if (!rule.allowedObjLeft.Contains(idxToAdd))
                                    {
                                        rule.allowedObjLeft.Add(idxToAdd);
                                    }

                                    break;

                                case "Up Side":
                                    if (!rule.allowedObjAbove.Contains(idxToAdd))
                                    {
                                        rule.allowedObjAbove.Add(idxToAdd);
                                    }
                                    break;

                                case "Right Side":
                                    if (!rule.allowedObjRight.Contains(idxToAdd))
                                    {
                                        rule.allowedObjRight.Add(idxToAdd);
                                    }

                                    break;

                                case "Down Side":
                                    if (!rule.allowedObjBelow.Contains(idxToAdd))
                                    {
                                        rule.allowedObjBelow.Add(idxToAdd);
                                    }

                                    break;

                                default:
                                    break;
                            }

                            added = true;
                        }

                        if (added) { break; }
                    }
                }


                foreach (var quickNode in graphViewCont.quickNodeData)
                {
                    //first of all check if the index exixts, then fore xapmple if i start with the above i need to get the index of evertyhing is open on the bottom and add it 
                    int idx = int.Parse(quickNode.IndexTile);

                    bool present = false;

                    foreach (var rule in ruleDec.ruleSet)
                    {
                        if (rule.assetIdx == idx)
                        {
                            present = true;
                            break;
                        }
                    }

                    if (!present)
                    {
                        ruleDec.ruleSet.Add(new WFCTileRule() { assetIdx = idx, mainAsset = ruleDec.tileSet[idx] });
                    }


                    WFCTileRule savedRuleRef = new WFCTileRule();

                    foreach (var rule in ruleDec.ruleSet)
                    {
                        if (rule.assetIdx == idx)
                        {
                            savedRuleRef = rule;
                            break;
                        }
                    }


                    foreach (var otherQuickNodes in graphViewCont.quickNodeData)
                    {
                        if (quickNode.IsOpenBelow)
                        {
                            if (otherQuickNodes.IsOpenAbove)
                            {
                                if (!savedRuleRef.allowedObjBelow.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjBelow.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }
                        else
                        {
                            if (!otherQuickNodes.IsOpenAbove)
                            {
                                if (!savedRuleRef.allowedObjBelow.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjBelow.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }


                        if (quickNode.IsOpenAbove)
                        {
                            if (otherQuickNodes.IsOpenBelow)
                            {
                                if (!savedRuleRef.allowedObjAbove.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjAbove.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }
                        else
                        {
                            if (!otherQuickNodes.IsOpenBelow)
                            {
                                if (!savedRuleRef.allowedObjAbove.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjAbove.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }


                        if (quickNode.IsOpenLeft)
                        {
                            if (otherQuickNodes.IsOpenRight)
                            {
                                if (!savedRuleRef.allowedObjLeft.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjLeft.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }
                        else
                        {
                            if (!otherQuickNodes.IsOpenRight)
                            {
                                if (!savedRuleRef.allowedObjLeft.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjLeft.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }


                        if (quickNode.IsOpenRight)
                        {
                            if (otherQuickNodes.IsOpenLeft)
                            {
                                if (!savedRuleRef.allowedObjRight.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjRight.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }
                        else
                        {
                            if (!otherQuickNodes.IsOpenLeft)
                            {
                                if (!savedRuleRef.allowedObjRight.Contains(int.Parse(otherQuickNodes.IndexTile)))
                                {
                                    savedRuleRef.allowedObjRight.Add(int.Parse(otherQuickNodes.IndexTile));
                                }
                            }
                        }
                    }
                }
            }
            GeneralUtil.SpacesUILayout(1);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ruleSet"));
        }
    }
}




