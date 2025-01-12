using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSys
{
    public class NodeGraphView : GraphView
    {
        private readonly Vector2 defaultNodeSize = new Vector2(100, 100);

        public NodeGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NodeGraphStyle"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
            CreateMiniMap();
        }

        public void CreateDefaultNode(string nodeName)
        {
            AddElement(GenerateChoiceNode(nodeName));
        }

        private NodeGraphNode GenerateBlankNode(string nodeName)
        {
            return new NodeGraphNode
            {
                title = nodeName,
                TextValue = "",
                EntryPoint = false
            };
        }

        private NodeGraphNode GenerateStubNode(NodeGraphNodeData basis = null)
        {
            NodeGraphNode node = GenerateBlankNode("");
            if (basis != null)
            {
                node += basis;
            }

            Port inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "In";
            node.inputContainer.Add(inputPort);

            AddNodeContentFields(node);

            node.SetPosition(new Rect(new Vector2(100, 400), defaultNodeSize));
            node.style.minWidth = defaultNodeSize.x;
            //idk why but this wasn't picked up by USS, so I have to put it here
            node.style.borderTopLeftRadius = 6;
            node.style.borderTopRightRadius = 6;
            node.style.borderBottomLeftRadius = 6;
            node.style.borderBottomRightRadius = 6;

            return node;
        }

        private NodeGraphNode GenerateEntryPointNode()
        {
            NodeGraphNode node = GenerateBlankNode("Start");
            node.EntryPoint = true;

            Port defaultPort = GeneratePort(node, Direction.Output, Port.Capacity.Single);
            defaultPort.portName = "Start";
            node.outputContainer.Add(defaultPort);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;
            node.capabilities &= ~Capabilities.Copiable;

            node.SetPosition(new Rect(100, 300, 100, 100));
            node.style.borderTopLeftRadius = 6;
            node.style.borderTopRightRadius = 6;
            node.style.borderBottomLeftRadius = 6;
            node.style.borderBottomRightRadius = 6;
            RefreshNode(node);

            return node;
        }

        public NodeGraphNode GenerateChoiceNode(string nodeName)
        {
            NodeGraphNode node = GenerateStubNode();
            node.title = nodeName;

            Button newOutputButton = new Button(() => { AddChoiceOutPort(node); });
            newOutputButton.text = "Add new Output";
            node.titleButtonContainer.Add(newOutputButton);

            AddChoiceOutPort(node, "Out 1");

            RefreshNode(node);

            return node;
        }

        public NodeGraphNode GenerateChoiceNode(NodeGraphNodeData data)
        {
            NodeGraphNode node = GenerateStubNode(data);

            node += data;

            Button newOutputButton = new Button(() => { AddChoiceOutPort(node); });
            newOutputButton.text = "Add new Output";
            node.titleButtonContainer.Add(newOutputButton);

            //for debugging
            //node.title = node.NodeGUID;

            RefreshNode(node);

            return node;
        }

        public NodeGraphNode GenerateStandardNode(NodeGraphNodeData data = null)
        {
            NodeGraphNode node = GenerateStubNode(data);

            node += data;

            //for debugging
            //node.title = node.NodeGUID;

            RefreshNode(node);

            return node;
        }

        private void RefreshNode(NodeGraphNode node)
        {
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GeneratePort<T>(NodeGraphNode node, Direction ioDirection, Port.Capacity capacity=Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, ioDirection, capacity, typeof(T));
        }
        
        private Port GeneratePort(NodeGraphNode node, Direction ioDirection, Port.Capacity capacity=Port.Capacity.Single)
        {
            return GeneratePort<string>(node, ioDirection, capacity);
        }

        public void AddNodeContentFields(NodeGraphNode node)
        {
            VisualElement container = new VisualElement();
            
            TextField textField = new TextField
            {
                value = node.TextValue,
                multiline = true,
                style =
                {
                    width = 334,
                    alignSelf = Align.Center
                }
            };
            textField.RegisterValueChangedCallback((callback) => { node.TextValue = callback.newValue; });

            //Explore later for conditions
            //ListView listView = new ListView();

            TestSerializedObject test = ScriptableObject.CreateInstance<TestSerializedObject>();
            SerializedObject testObj = new SerializedObject(test);

            IMGUIContainer eventField = new IMGUIContainer(()=> {
                testObj.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(testObj.FindProperty("testEvent"));
                if (EditorGUI.EndChangeCheck())
                {
                    testObj.ApplyModifiedProperties();
                }
            });
            container.Add(eventField);

            container.Add(new Label("Text"));
            container.Add(textField);
            
            node.Add(container);
        }

        public void AddChoiceOutPort(NodeGraphNode node, string portName = null)
        {
            Port newPort = GeneratePort(node, Direction.Output);

            string portNameVal = portName != null ? portName : "Out " + (node.outputContainer.Query("connector").ToList().Count + 1).ToString();

            Label oldLabel = newPort.contentContainer.Q<Label>("type");
            newPort.contentContainer.Remove(oldLabel);

            TextField textField = new TextField
            {
                name = string.Empty,
                value = portNameVal,
                multiline = true
            };
            textField.RegisterValueChangedCallback((callback) => { newPort.portName = callback.newValue; });
            
            Button deleteButton = new Button(() => { RemovePort(node, newPort); }) { text = "X"};
            deleteButton.style.alignSelf = Align.Stretch;

            newPort.name = Guid.NewGuid().ToString();
            newPort.portName = portNameVal;
            newPort.style.height = StyleKeyword.Auto;
            newPort.contentContainer.Add(new Label(" Out"));
            newPort.contentContainer.Add(textField);
            newPort.contentContainer.Add(deleteButton);
            node.outputContainer.Add(newPort);
            RefreshNode(node);
        }

        public void AddOutPort(NodeGraphNode node)
        {
            Port newPort = GeneratePort(node, Direction.Output);

            string portNameVal = "Out";

            newPort.name = Guid.NewGuid().ToString();
            newPort.portName = portNameVal;
            newPort.style.height = StyleKeyword.Auto;
            newPort.contentContainer.Add(new Label(" Out"));
            node.outputContainer.Add(newPort);
            RefreshNode(node);
        }

        public void RemovePort(NodeGraphNode node, Port port)
        {
            var targetEdge = edges.ToList().Where((item) => { return item.output.name == port.name && item.output.node == port.node; });

            if(targetEdge.Any())
            {
                Edge edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(edge);
            }
            node.outputContainer.Remove(port);
            RefreshNode(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach((port) => 
            {
                if(startPort != port && startPort.node != port.node && port.direction != startPort.direction)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        private void CreateMiniMap()
        {
            MiniMap miniMap = new MiniMap { anchored = true };
            miniMap.SetPosition(new Rect(10, 30, 200, 200));
            Add(miniMap);
        }
    }
}
