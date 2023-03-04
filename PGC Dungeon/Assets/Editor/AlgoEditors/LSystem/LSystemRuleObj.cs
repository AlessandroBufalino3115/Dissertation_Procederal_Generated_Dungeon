using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonForge
{
    [Serializable]
    public class LSystemRuleObj : ScriptableObject
    {
        [Header("L-System ruleset\nA = move 10 blocks\nB = move 15 blocks\nC = move 20 blocks\nS = ave current position in stack\nL = load last position from stack\n+ = turn clockwise\n- = turn anti-clockwise\n")]

        public int A_Length = 10;
        public int B_Length = 20;
        public int C_Length = 30;

        public List<string> A_RuleSet = new List<string>();
        public List<string> B_RuleSet = new List<string>();
        public List<string> C_RuleSet = new List<string>();
        public List<string> S_RuleSet = new List<string>();
        public List<string> L_RuleSet = new List<string>();
        public List<string> Psign_RuleSet = new List<string>();
        public List<string> Nsign_RuleSet = new List<string>();
    }
}