using DS.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements 
{

    using DS.Utilities;
    public class DSMultiChoiceNode : DSNode
    {


        public override void Initialize(Vector2 pos)
        {

            base.Initialize(pos);


            dialogueType = DSDialogueType.MultiChoice;

           // Choices.Add("New choice");
        }


        public override void Draw()
        {
            base.Draw();

            Label dialogueTextField = new Label("\n Main Rule");

            titleContainer.Insert(0, dialogueTextField);


            // multi field its going to be the middle fo the ruel, needs to have 

            // textfield with the index reperesenitng the tile

            //4 ports for each direction


            var textFieldIndexRule = DSElementUtility.CreateTextField(indexVal);
            textFieldIndexRule.MarkDirtyRepaint();
            textFieldIndexRule.RegisterValueChangedCallback(evt => indexVal = evt.newValue);

            mainContainer.Insert(1,textFieldIndexRule);


            Port LeftPort = this.CreatePort("Left Side", Orientation.Horizontal, Direction.Input,Port.Capacity.Multi);
            inputContainer.Add(LeftPort);
            
            Port UpPort = this.CreatePort("Up Side", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(UpPort);

            Port RightPort = this.CreatePort("Right Side", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(RightPort);

            Port DownPort = this.CreatePort("Down Side", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(DownPort);

            RefreshExpandedState();
        }
    }

}
