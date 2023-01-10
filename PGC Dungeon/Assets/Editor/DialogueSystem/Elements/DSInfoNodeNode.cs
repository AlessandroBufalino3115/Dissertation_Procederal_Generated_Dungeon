using DS.Enumerations;
using DS.Windows;
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

