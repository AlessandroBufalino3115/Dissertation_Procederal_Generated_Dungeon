using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LSystemRuleSet : ScriptableObject
{

    [Header("bla bla\n bla bla\nbla bla\nbla bla\nbla bla\nbla bla\nbla bla\nbla bla\n")]

    public List<string> A_RuleSet = new List<string>();
    public List<string> B_RuleSet = new List<string>();
    public List<string> C_RuleSet = new List<string>();
    public List<string> S_RuleSet = new List<string>();
    public List<string> L_RuleSet = new List<string>();
    public List<string> P_RuleSet = new List<string>();
    public List<string> N_RuleSet = new List<string>();
}

