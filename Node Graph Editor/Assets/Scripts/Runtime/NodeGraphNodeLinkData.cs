using System;
using UnityEngine;

namespace NodeSys
{
    [Serializable]
    public class NodeGraphNodeLinkData
    {
        [SerializeField]
        private string rootNodeGUID;
        [SerializeField]
        private string rootNodePortID;

        [SerializeField]
        private string branchNodeGUID;
        [SerializeField]
        private string branchNodePortID;

        public string RootNodeGUID { get { return rootNodeGUID; } set { rootNodeGUID = value; } }
        public string RootNodePortID { get { return rootNodePortID; } set { rootNodePortID = value; } }
        public string BranchNodeGUID { get { return branchNodeGUID; } set { branchNodeGUID = value; } }
        public string BranchNodePortID { get { return branchNodePortID; } set { branchNodePortID = value; } }

    }
}