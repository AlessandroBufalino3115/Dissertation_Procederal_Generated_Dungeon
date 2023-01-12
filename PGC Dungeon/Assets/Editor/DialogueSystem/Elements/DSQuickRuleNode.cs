using DS.Enumerations;
using DS.Utilities;
using DS.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{

    public class DSQuickRuleNode : DSNode
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

            var textFieldIndexRule = DSElementUtility.CreateTextField(indexVal);
            textFieldIndexRule.MarkDirtyRepaint();
            //textFieldIndexRule.RegisterValueChangedCallback(
            //evt => {
            //    indexVal = CheckExists(evt.newValue);
            //    titleString = allowed == true ? $"{titleString}" : $"<color=red>{titleString}</color>";
            //});   //indexVal = evt.newValue




            mainContainer.Insert(1, textFieldIndexRule);



            Toggle isOpenRight = new Toggle() { text = "Right" };
            mainContainer.Add(isOpenRight);
            Toggle isOpenLeft = new Toggle() { text = "Left" };
            mainContainer.Add(isOpenLeft);
            Toggle isOpenAbove = new Toggle() { text = "Above" };
            mainContainer.Add(isOpenAbove);
            Toggle isOpenBelow = new Toggle() { text = "Below" };
            mainContainer.Add(isOpenBelow);






            RefreshExpandedState();
        }
    }



}
