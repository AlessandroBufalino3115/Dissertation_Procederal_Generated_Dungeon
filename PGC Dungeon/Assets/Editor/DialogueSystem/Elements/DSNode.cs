using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;




namespace DS.Elements 
{

    using DS.Enumerations;
    using DS.Utilities;
    using UnityEditor;

    public class DSNode : Node
    {
        //public string DialogueName { get; set; }
       // public List<string> Choices { get; set; }
       // public string Text { get; set; }
        
        public DSDialogueType dialogueType { get; set; }
        public string indexVal = "";
        public string nodeGuid = "";

        public virtual void Initialize(Vector2 pos) 
        {
            nodeGuid = GUID.Generate().ToString();

            SetPosition(new Rect(pos, Vector2.zero));
            RefreshPorts();
            RefreshExpandedState();
        }


        public virtual void Draw() 
        {

        }


        


    }
}

