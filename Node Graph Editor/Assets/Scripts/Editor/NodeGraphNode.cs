using NUnit.Framework.Internal;
using Packages.Rider.Editor.UnitTesting;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace NodeSys
{
    public class NodeGraphNode : Node
    {
        private string nodeGUID;
        private string textValue;
        private bool entryPoint = false;
        private NodeType nodeType = NodeType.Standard;

        //need to introduce event system, so things occur when this node is transitioned to

        public string NodeGUID { get { return nodeGUID; } set { nodeGUID = value; } }
        public string TextValue { get { return textValue; } set { textValue = value; } }
        public bool EntryPoint { get { return entryPoint; } set { entryPoint = value; } }
        public NodeType NodeType { get { return nodeType; } set { nodeType = value; } }

        public NodeGraphNode()
        {
            GenerateNodeUID();
        }

        public NodeGraphNode(NodeGraphNodeData data = null)
        {
            if (data == null)
            {
                new NodeGraphNode();
            }
            else
            {
                AssignNodeData(data);
            }
        }

        public void GenerateNodeUID()
        {
            nodeGUID = Guid.NewGuid().ToString();
        }

        public static NodeGraphNode operator +(NodeGraphNode node, NodeGraphNodeData data)
        {
            node.AssignNodeData(data);
            return node;
        }

        private void AssignNodeData(NodeGraphNodeData data)
        {
            NodeGUID = data.NodeGUID;
            TextValue = data.TextValue;
            title = data.TitleText;
            SetPosition(new UnityEngine.Rect(data.Position, UnityEngine.Vector2.zero));
        }
    }
}