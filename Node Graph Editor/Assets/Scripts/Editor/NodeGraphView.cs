using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void CreateNode(string nodeName)
        {
            AddElement(GenerateNode(nodeName));
        }

        private NodeGraphNode GenerateBlankNode(string nodeName)
        {
            return new NodeGraphNode
            {
                title = nodeName,
                TextValue = "default text",
                EntryPoint = false
            };
        }

        private NodeGraphNode GenerateEntryPointNode()
        {
            NodeGraphNode node = GenerateBlankNode("Start Node");
            node.EntryPoint = true;

            Port defaultPort = GeneratePort(node, Direction.Output, Port.Capacity.Single);
            defaultPort.portName = "Start";
            node.outputContainer.Add(defaultPort);

            node.SetPosition(new Rect(100, 200, 100, 100));
            RefreshNode(node);

            return node;
        }

        public NodeGraphNode GenerateNode(string nodeName)
        {
            NodeGraphNode node = GenerateBlankNode(nodeName);

            Port inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "In";

            node.inputContainer.Add(inputPort);

            Button newOutputButton = new Button(() => { AddOutPort(node); });
            newOutputButton.text = "Add new Output";

            node.titleButtonContainer.Add(newOutputButton);

            node.SetPosition(new Rect(new Vector2(100, 100), defaultNodeSize));
            node.style.minWidth = defaultNodeSize.x;

            AddOutPort(node, "Out 1");

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

        public void AddOutPort(NodeGraphNode node, string portName = null)
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
    }
}
