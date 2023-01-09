using DS.Enumerations;
using DS.Utilities;
using DS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements 
{

    public class DSInfoNodeNode : DSNode
    {
        public override void Initialize(Vector2 pos, DSGraphView graphView)
        {

            base.Initialize(pos, graphView);

            dialogueType = DSDialogueType.InfoNode;
        }

        public override void Draw()
        {
            base.Draw();

            Label dialogueTextField = new Label("\n Rule Legend");

            titleContainer.Insert(0, dialogueTextField);

            

            RefreshExpandedState();
        }
    }



}

