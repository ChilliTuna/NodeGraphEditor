using System;
using UnityEditor.Experimental.GraphView;

namespace NodeSys
{
    public class NodeGraphNode : Node
    {
        private string nodeGUID;
        private string textValue;
        private bool entryPoint = false;

        public string NodeGUID { get { return nodeGUID; } internal set { textValue = value; } }
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
    }
}