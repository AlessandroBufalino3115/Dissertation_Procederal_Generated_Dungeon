

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DS.Windows;
using System.IO;

[CustomEditor(typeof(WFCRuleDecipher))]
public class WFCRuleDecipherEditor : Editor
{

    //the saving and loading al of that works fine

    // the issues are the resources folde risnt getting loaded
    // need a quick way to insta some of this shits 
    //need a space on top of the bar
    


    // maybes:
    //maybe find a way to show the tile
    //maybe 




    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WFCRuleDecipher ruleDec = (WFCRuleDecipher)target;

     
        if (GUILayout.Button("Load rule Set"))
        {

            IDictionary<int, string> dictNameIdx = new Dictionary<int, string>();
            

            var fileName = "Assets/Resources/" + ruleDec.tileSetFolderName;

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

                dictNameIdx.Add(currIdx, ruleDec.tileSetFolderName + "/" + manipString);

                currIdx++;
            }



            ruleDec.ruleSet.Clear();

            var graphViewCont = Resources.Load<GraphViewDataCont>(ruleDec.ruleSetFolderName);

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
                        Debug.Log(dictNameIdx[idx]);
                        ruleDec.ruleSet.Add(new WFCTileRule() { assetIdx = idx , mainAsset = Resources.Load(dictNameIdx[idx]) as GameObject });
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
                        Debug.Log(outputNode.IndexTile);
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


                                if (!rule.allowedObjForward.Contains(idxToAdd))
                                {
                                    rule.allowedObjForward.Add(idxToAdd);
                                }
                                break ;

                            case "Right Side":

                                if (!rule.allowedObjRight.Contains(idxToAdd))
                                {
                                    rule.allowedObjRight.Add(idxToAdd);
                                }

                                break ;

                            case "Down Side":

                                if (!rule.allowedObjBackwards.Contains(idxToAdd))
                                {
                                    rule.allowedObjBackwards.Add(idxToAdd);
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
        }
    }




}
