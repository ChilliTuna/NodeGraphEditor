using System;
using UnityEditor.Experimental.GraphView;

namespace NodeSys
{
    public class NodeGraphNode : Node
    {
        private string nodeGUID;
        private string textValue;
        private bool entryPoint = false;

        //need to introduce event system, so things occur when this node is transitioned to

        public string NodeGUID { get { return nodeGUID; } set { nodeGUID = value; } }
        public string TextValue { get { return textValue; } set { textValue = value; } }
        public bool EntryPoint { get { return entryPoint; } set { entryPoint = value; } }

        public NodeGraphNode()
        {
            GenerateNodeUID();
        }

        public void GenerateNodeUID()
        {
            nodeGUID = Guid.NewGuid().ToString();
        }

        public static NodeGraphNode operator +(NodeGraphNode node, NodeGraphNodeData data)
        {
            node.NodeGUID = data.NodeGUID;
            node.TextValue = data.TextValue;
            node.title = data.TitleText;
            node.SetPosition(new UnityEngine.Rect(data.Position, UnityEngine.Vector2.zero));
            return node;
        }
    }
}