using System;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

namespace NodeSys
{
    public enum NodeType
    {
        Standard,
        Choice
    }

    [Serializable]
    public class NodeGraphNodeData
    {
        [SerializeField]
        private string nodeGUID;
        [SerializeField]
        private string textValue;
        [SerializeField]
        private string titleText;
        [SerializeField]
        private Vector2 position;
        
        private NodeType nodeType;

        public string NodeGUID
        {
            get { return nodeGUID; }
            set
            {
                if (nodeGUID == null)
                {
                    nodeGUID = value;
                }
                else
                {
                    throw new Exception("Tried to reassign Node GUID (from: " + nodeGUID + "; to: " + value + ")");
                }
            }
        }
        public string TextValue { get { return textValue; } set { textValue = value; } }
        public string TitleText { get { return titleText; } set { titleText = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public NodeType NodeType { get { return nodeType; } set { nodeType = value; } }
    }
}
