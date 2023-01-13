

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DS.Windows;
using System.IO;

[CustomEditor(typeof(WFCRuleDecipher))]
public class WFCRuleDecipherEditor : Editor
{

    bool showRules;



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WFCRuleDecipher ruleDec = (WFCRuleDecipher)target;

        Spaces(4);

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

        Spaces(4);


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

            var graphViewCont = Resources.Load<GraphViewDataCont>("WFC RuleSets/" + ruleDec.ruleSetFileName);



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

                foreach(var rule in ruleDec.ruleSet)
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
                                break ;

                            case "Right Side":

                                if (!rule.allowedObjRight.Contains(idxToAdd))
                                {
                                    rule.allowedObjRight.Add(idxToAdd);
                                }

                                break ;

                            case "Down Side":

                                if (!rule.allowedObjBelow.Contains(idxToAdd))
                                {
                                    rule.allowedObjBelow.Add(idxToAdd);
                                }

                                break ;

                            default:
                                break ;
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

    }



    private void Spaces(int spaceNum)
    {
        for (int i = 0; i < spaceNum; i++)
        {
            EditorGUILayout.Space();
        }
    }


}
