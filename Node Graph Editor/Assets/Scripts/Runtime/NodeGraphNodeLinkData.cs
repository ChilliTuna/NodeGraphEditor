using System;

namespace NodeSys
{
    [Serializable]
    public class NodeGraphNodeLinkData
    {
        private string rootNodeGUID;
        private string rootNodePortID;

        private string branchNodeGUID;
        private string branchNodePortID;

        public string RootNodeGUID { get { return rootNodeGUID; } set { rootNodeGUID = value; } }
        public string RootNodePortID { get { return rootNodePortID; } set { rootNodePortID = value; } }
        public string BranchNodeGUID { get { return branchNodeGUID; } set { branchNodeGUID = value; } }
        public string BranchNodePortID { get { return branchNodePortID; } set { branchNodePortID = value; } }

    }
}