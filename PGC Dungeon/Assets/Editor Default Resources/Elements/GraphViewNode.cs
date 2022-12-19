using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphViewNode : Node
{
   public string DialogueName { get; set; }
    public List<string> Choices { get; set; }

    public string Text { get; set; }

    public GVRuleType RuleType { get; set; }


    public void Initialize(Vector2 position) 
    {
        DialogueName = "DialogueName";
        Choices = new List<string>();
        Text = "Dialogue text.";

        SetPosition(new Rect(position, Vector2.zero));
    }

    public void Draw() 
    {
        TextField dialogueNameTextField = new TextField()
        {
            value = DialogueName
        };

        titleContainer.Insert(0, dialogueNameTextField);

        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));

        inputPort.portName = "Dialogue Connection";

        inputContainer.Add(inputPort);

        VisualElement customConatiner = new VisualElement();

        Foldout textFoldout = new Foldout() {
        
            text = "dialogue text"
        };


        TextField textTextField = new TextField() {
        
            value = Text
        };


        textFoldout.Add(textTextField);
        customConatiner.Add(textFoldout);

        extensionContainer.Add(customConatiner);

        RefreshExpandedState();
    }

}
