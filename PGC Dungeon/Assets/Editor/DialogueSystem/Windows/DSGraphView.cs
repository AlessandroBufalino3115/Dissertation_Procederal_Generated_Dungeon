using DS.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Windows
{

    using DS.Enumerations;
    using DS.Utilities;
    using System;
    using System.IO;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEditor.Graphs;
    using UnityEngine.Windows;

    public class DSGraphView : GraphView
    {

        //need to add checks for whne the val is incorrect
        // add a text under the sub rule node to say which one is sleected



        private DSSearchWindow searchWindow;

        private DSEditorWindow editorWindow;

        private GraphViewDataCont graphViewCont;

        private DSInfoNodeNode ruleNode;
        public IDictionary<int,string> ruleDict = new Dictionary<int,string>();


        public DSGraphView( DSEditorWindow dSEditorWindow) 
        {
      

            editorWindow = dSEditorWindow;

            AddManipulators();

            AddSearchWindow();
            AddGridBackground();


            AddMiniMap();

            AddStyles();
        }

        private void AddMiniMap() 
        {
            MiniMap miniMap = new MiniMap()
            {
                anchored = true,
            };

            miniMap.SetPosition(new Rect(15, 50, 200, 100));

            this.Add(miniMap);

        }

        private void AddSearchWindow() 
        {
            if (searchWindow == null) 
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();

                searchWindow.Initilize(this);
            
            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
            
        }
        private void AddManipulators() 
        {

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(CreateNodeContextualMenu("Add node (Rule Node)", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add node (Tile Node)", DSDialogueType.MultiChoice));
            this.AddManipulator(new ContentDragger());


            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateGroupContextualMenu());
        }


        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }
        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/UIGraphViewStyle.uss");

            styleSheets.Add(styleSheet);

        }

        public void RefreshRules(string fileName)
        {

            ruleDict.Clear();

            var nodes = this.nodes.ToList();
            bool respawn = true;


            foreach (var GVnode in nodes)
            {
                DSNode iterNode = GVnode as DSNode;

                if (iterNode.dialogueType == DSDialogueType.InfoNode)
                {
                    respawn = false;
                    break;
                }
            }


            if (respawn) 
            {
                ruleNode = (DSInfoNodeNode)CreateNode(DSDialogueType.InfoNode, Vector2.zero);
                this.AddElement(ruleNode);
            }





            if (fileName == null) { return; }


            List<string> fileNames = new List<string>();


            fileName = "Assets/Resources/" + fileName;

            var info = new DirectoryInfo(fileName);
            var fileInfo = info.GetFiles();

            foreach (var file in fileInfo)
            {
                if (file.Name.Contains("meta")) 
                {
                    continue;
                }


                int index = file.Name.IndexOf(".");
                var manipString = file.Name.Substring(0, index);

                manipString = manipString.Replace("Variant", "");

                fileNames.Add(manipString);

                ruleDict.Add(fileNames.Count - 1, manipString);

            }

            AddRuleNode(fileNames);
        }



        private void AddRuleNode(List<string> TileSetNames)
        {


            ruleNode.inputContainer.Clear();
            ruleNode.outputContainer.Clear();

            Label tileNameDesc = new Label() { text = "Name of the tile:" };
            Label tileIndexDesc = new Label() { text = "Index of the tile:" };

            ruleNode.inputContainer.Add(tileNameDesc);
            ruleNode.outputContainer.Add(tileIndexDesc);

            ruleNode.RefreshExpandedState();


            for (int i = 0; i < TileSetNames.Count; i++)
            {

                Label tileName = new Label() { text = TileSetNames[i] };
                Label tileIndex = new Label() { text = "    " +i.ToString() };

                ruleNode.inputContainer.Add(tileName);
                ruleNode.outputContainer.Add(tileIndex);

                ruleNode.RefreshExpandedState();
            }
        }






        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add group", actionEvent => AddElement(CreateGroup("Dialouge group", GetLocalMousePos( actionEvent.eventInfo.localMousePosition))))
                );



            return contextualMenuManipulator;
        }

        public GraphElement CreateGroup(string title, Vector2 pos) 
        {
            

            Group group = new Group() 
            {
                title = title
            };

            group.SetPosition(new Rect(pos, Vector2.zero));

            return group;

        }




        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType type) 
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(type, GetLocalMousePos(actionEvent.eventInfo.localMousePosition))))
                );



            return contextualMenuManipulator;
        }
        public DSNode CreateNode(DSDialogueType type,Vector2 pos)
        {

            Type nodeType = Type.GetType($"DS.Elements.DS{type}Node");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(pos,this);
            node.Draw();

            return node;
        }


        public DSNode CreateNode(DSDialogueType type, Vector2 pos, string indexVal, string Guid)
        {

            Type nodeType = Type.GetType($"DS.Elements.DS{type}Node");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(pos,this);
            node.indexVal = indexVal;
            node.nodeGuid = Guid;
            
            node.Draw();




            return node;
        }



        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port => {
            
            if (startPort == port) { return; }

            if (startPort.node == port.node) { return; }

            if (startPort.direction == port.direction) { return; }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public Vector2 GetLocalMousePos (Vector2 pos, bool isSearchWindow = false) 
        {
            Vector2 worldMousePos = pos;

            if (searchWindow) 
            {

                worldMousePos -= editorWindow.position.position;
            }


            Vector2 localMousePos = contentViewContainer.WorldToLocal(worldMousePos);

            return localMousePos;
        }



        public void LoadGraph(string filename) 
        {
            graphViewCont = Resources.Load<GraphViewDataCont>(filename);

            if (graphViewCont == null) { return; }



            ClearGraph();
            GenNodes();


            var arr = nodes.ToList().Cast<DSNode>().ToList();
            ConnectNodes(arr);

            

            RefreshRules(editorWindow._fileNameResources);

        }

        private void ConnectNodes(List<DSNode> arr)
        {


            var nodeConnections = graphViewCont.nodeLinkData;

            foreach (var nodeCon in nodeConnections)
            {

                DSNode inputNode = null;
                DSNode outputNode = null;

                foreach (var node in arr)
                {
                    if (node.nodeGuid == nodeCon.TargetNodeGuid) 
                    {
                        inputNode = node;
                    }
                    else if (node.nodeGuid == nodeCon.BaseNodeGuid)
                    {
                        outputNode = node; 
                    }
                }



                int idx = DSElementUtility.GetPortIdx(nodeCon.PortName);


                var tempEdge = new UnityEditor.Experimental.GraphView.Edge { output = (Port)outputNode.outputContainer[0], input = (Port)inputNode.inputContainer[idx] };


                tempEdge?.input.Connect(tempEdge);
                tempEdge?.output.Connect(tempEdge);

                this.Add(tempEdge);

            }


        }

        private void GenNodes() 
        {

            var list = new List<DSNode>();

            foreach (var node in graphViewCont.nodeData)
            {
                var createdNode = CreateNode(node.dialogueType, node.position, node.IndexTile, node.nodeGuid);
                list.Add(createdNode);
                this.AddElement(createdNode);
            }

        }

        private void ClearGraph() 
        {

            foreach (var edge in edges)
            {
                this.RemoveElement(edge);
            }

            foreach (var node in nodes)
            {
                
                this.RemoveElement(node);
            }
        }






        public void SaveGraph(string filename) 
        {
            


            var edges = this.edges.ToList();  
            var nodes = this.nodes.ToList();  


            if (!edges.Any()) return;



            var GVcont = ScriptableObject.CreateInstance<GraphViewDataCont>();


            var connectedPorts = edges.Where(x => x.input.node != null).ToArray();

            for (int i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as DSNode;
                var inputNode = connectedPorts[i].input.node as DSNode;

                GVcont.nodeLinkData.Add(new NodeLinkData() { BaseNodeGuid = outputNode.nodeGuid, PortName = connectedPorts[i].input.portName, TargetNodeGuid = inputNode.nodeGuid });

            }

            bool save = true;

            foreach (var GVnode in nodes)
            {
                DSNode iterNode = GVnode as DSNode;

                if (iterNode.dialogueType == DSDialogueType.InfoNode)  { continue; }
                if (iterNode.allowed == false) 
                {
                    save  =false;
                    EditorUtility.DisplayDialog("Inavlid Index Given", "lase ensure all the index are within range", "OK!");
                    break;
                }
                GVcont.nodeData.Add(new NodeData() { position = iterNode.GetPosition().position,     nodeGuid = iterNode.nodeGuid  ,  dialogueType = iterNode.dialogueType,    IndexTile = iterNode.indexVal          });
            }


            if (save == false) 
            {
                return;
            }


            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.Refresh();
               
            }

            AssetDatabase.CreateAsset(GVcont, $"Assets/Resources/{filename}.asset");
            AssetDatabase.SaveAssets();
           
        
        }



    }




    [Serializable]
    public class GraphViewDataCont : ScriptableObject 
    {
       public List<NodeData> nodeData = new List<NodeData>();
       public List<NodeLinkData> nodeLinkData = new List<NodeLinkData>();
    }



    [Serializable]
    public class NodeData
    {
        public Vector2 position;
        public string nodeGuid;
        public string IndexTile;
        public DSDialogueType dialogueType;
    }




    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string PortName;
        public string TargetNodeGuid;
    }




}

