using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILSystemState : UiBaseState
{


    public List<KeyValuePair> ListToSee = new List<KeyValuePair>();



    public static L_systemAlgoBasic instance;
    public string axium;


    public string proposedRuleName;
    public string proposedAction;

    public Dictionary<char, string> rulesDict = new Dictionary<char, string>();

    [Range(2, 10)]
    public int iterations;



    public string solution;







    public override void onExit(StateUIManager currentMenu)
    {
    }

    public override void onGUI(StateUIManager currentMenu)
    {

        // axium   input field 
        
        // add key    input field
        // add rule    input field








    }

    public override void onStart(StateUIManager currentMenu)
    {
    }

    public override void onUpdate(StateUIManager currentMenu)
    {
    }





    public void AddRule()
    {
        if (proposedRuleName.Length != 1)
        {
            Debug.Log($"<color=red>This RuleName is not 1 character</color>");

            proposedRuleName = "";

            return;
        }

        proposedRuleName.ToUpper();
        char ruleName = proposedRuleName[0];

        if (rulesDict.ContainsKey(ruleName))
        {
            Debug.Log($"<color=red>This RuleName is already there</color>");
        }

        if (proposedAction.Length > 0)
        {
            proposedAction.ToUpper();
        }
        else
        {
            Debug.Log($"<color=red>Action not valid</color>");

            proposedAction = "";
            return;
        }

        string actionRule = proposedAction;


        rulesDict.Add(ruleName, actionRule);

    }

    public void PrintDict()
    {
        ListToSee.Clear();

        foreach (KeyValuePair<char, string> entry in rulesDict)
        {
            ListToSee.Add(new KeyValuePair(entry.Key, entry.Value));
        }
    }



    public void RunLSystemAlgo()
    {
        solution = axium;


        for (int i = 0; i < iterations; i++)
        {
            string presolution = "";
            for (int z = 0; z < solution.Length; z++)
            {
                presolution += rulesDict[(char)solution[z]];
            }

            solution = presolution;


        }


        Debug.Log($"<color=green>{solution}</color>");

    }


}
