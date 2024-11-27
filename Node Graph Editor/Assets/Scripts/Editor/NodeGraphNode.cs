using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSys
{
    public class NodeGraphNode :  Node
    {
        public string UID;

        public string textValue;

        public bool entryPoint = false;
    }
}
