using System.Collections.Generic;
using UnityEngine;

namespace NodeSys
{
    public class NodeGraphContainer : ScriptableObject
    {
        [SerializeField]
        public List<NodeGraphNodeLinkData> nodeLinks = new List<NodeGraphNodeLinkData>();
        [SerializeField]
        public List<NodeGraphNodeData> nodeData = new List<NodeGraphNodeData>();
    }
}
