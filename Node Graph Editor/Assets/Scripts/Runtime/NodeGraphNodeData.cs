using System;
using UnityEngine;

namespace NodeSys
{
    [Serializable]
    public class NodeGraphNodeData
    {
        private string nodeGUID;
        private string textValue;
        private Vector2 position;

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
        public Vector2 Position { get { return position; } set { position = value; } }
    }
}