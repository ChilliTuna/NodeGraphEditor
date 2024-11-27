using System;
using System.Collections;
using System.Collections.Generic;
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
                UID = Guid.NewGuid().ToString(),
                textValue = "default text",
                entryPoint = true
            };
        }

        private NodeGraphNode GenerateEntryPointNode()
        {
            NodeGraphNode node = GenerateBlankNode("Start Node");

            Port defaultPort = GeneratePort(node, Direction.Output, Port.Capacity.Single);
            defaultPort.portName = "Next";
            node.outputContainer.Add(defaultPort);

            node.SetPosition(new Rect(100, 200, 100, 100));
            RefreshNode(node);

            return node;
        }

        private NodeGraphNode GenerateNode(string nodeName)
        {
            NodeGraphNode node = GenerateBlankNode(nodeName);

            Port outputPort = GeneratePort(node, Direction.Output);
            outputPort.portName = "Out";

            Port inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "In";

            node.outputContainer.Add(inputPort);
            node.outputContainer.Add(outputPort);

            Button newOutputButton = new Button(() => { AddOutPort(node); });
            newOutputButton.text = "Add new Output";

            node.titleButtonContainer.Add(newOutputButton);

            node.SetPosition(new Rect(new Vector2(100, 100), defaultNodeSize));
            RefreshNode(node);


            return node;
        }

        private void RefreshNode(NodeGraphNode node)
        {
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        private Port GeneratePort<T>(NodeGraphNode node, Direction ioDirection, Port.Capacity capacity=Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, ioDirection, capacity, typeof(T));
        }
        
        private Port GeneratePort(NodeGraphNode node, Direction ioDirection, Port.Capacity capacity=Port.Capacity.Single)
        {
            return GeneratePort<string>(node, ioDirection, capacity);
        }

        private void AddOutPort(NodeGraphNode node)
        {
            Port newPort = GeneratePort(node, Direction.Output);

            int outPortCount = node.outputContainer.Query("connector").ToList().Count;
            newPort.portName = "Out " + outPortCount.ToString();

            node.outputContainer.Add(newPort);
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
