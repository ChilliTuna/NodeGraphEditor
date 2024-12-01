using System.Collections.Generic;
using UnityEngine;

namespace NodeSys
{
    public class NodeGraphContainer : ScriptableObject
    {
        public List<NodeGraphNodeLinkData> nodeLinks = new List<NodeGraphNodeLinkData>();
        public List<NodeGraphNodeData> nodeData = new List<NodeGraphNodeData>();
    }
}
