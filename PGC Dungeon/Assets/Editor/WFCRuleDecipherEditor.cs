

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using DS.Elements;
using DS.Windows;

[CustomEditor(typeof(WFCRuleDecipher))]
public class WFCRuleDecipherEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WFCRuleDecipher ruleDec = (WFCRuleDecipher)target;



        if (GUILayout.Button("Load rule Set"))
        {

            ruleDec.Ruleset.Clear();

            var graphViewCont = Resources.Load<DS.Windows.GraphViewDataCont>(ruleDec.fileName);

            var dictRuleGuid = new Dictionary<string, int>();

            foreach (var node in graphViewCont.nodeData)   // this creates all the rules 
            {
                if (node.dialogueType == DS.Enumerations.DSDialogueType.MultiChoice) 
                {
                    int idx = int.Parse(node.IndexTile);


                    bool present = false;

                    foreach (var rule in ruleDec.Ruleset)
                    {
                        if (rule.assetIdx == idx) 
                        {
                            present = true;
                            break;
                        }
                    }

                    if (!present) 
                    {
                        ruleDec.Ruleset.Add(new WFCTileRule() { assetIdx = idx });
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



                foreach(var rule in ruleDec.Ruleset)
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




        if (GUILayout.Button("Load rule ddSet"))
        {

        }



    }




}
