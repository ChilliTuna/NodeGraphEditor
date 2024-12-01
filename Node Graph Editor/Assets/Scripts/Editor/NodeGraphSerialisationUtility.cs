using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSys
{
    public class NodeGraphSerialisationUtility
    {
        private NodeGraphView targetNodeGraphView;
        private List<Edge> edges => targetNodeGraphView.edges.ToList();
        private List<NodeGraphNode> nodes => targetNodeGraphView.nodes.ToList().Cast<NodeGraphNode>().ToList();

        private NodeGraphContainer cachedContainer;

        public static NodeGraphSerialisationUtility GetInstance(NodeGraphView targetGraphView)
        {
            return new NodeGraphSerialisationUtility
            {
                targetNodeGraphView = targetGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            if (!edges.Any()) { return; }
            
            NodeGraphContainer graphContainer = ScriptableObject.CreateInstance<NodeGraphContainer>();
            Edge[] connectedPorts = edges.Where((item) => { return item.input.node != null; }).ToArray();
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                NodeGraphNode outputNode = connectedPorts[i].output.node as NodeGraphNode;
                NodeGraphNode inputNode = connectedPorts[i].input.node as NodeGraphNode;

                graphContainer.nodeLinks.Add(new NodeGraphNodeLinkData()
                {
                    RootNodeGUID = outputNode.NodeGUID,
                    RootNodePortID = connectedPorts[i].output.portName,
                    BranchNodeGUID = inputNode.NodeGUID
                });
            }

            foreach (NodeGraphNode node in nodes.Where((node) => { return !node.EntryPoint; }))
            {
                graphContainer.nodeData.Add(new NodeGraphNodeData()
                {
                    NodeGUID = node.NodeGUID,
                    TextValue = node.TextValue,
                    Position = node.GetPosition().position
                });
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.CreateFolder("Assets/Resources", "NodeGraph");
            }
            else if (!AssetDatabase.IsValidFolder("Assets/Resources/NodeGraph"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "NodeGraph");
            }

            AssetDatabase.CreateAsset(graphContainer, $"Assets/Resources/NodeGraph/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            cachedContainer = Resources.Load<NodeGraphContainer>(fileName);
            if (cachedContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", $"Node Graph \"{fileName}\" file does not exist.", "OK");
                return;
            }

            ClearExistingGraph();
            CreateNodesFromFile();
        }

        private void ClearExistingGraph()
        {
            nodes.Find((item) => { return item.EntryPoint; }).NodeGUID = cachedContainer.nodeLinks[0].RootNodeGUID;

            foreach (NodeGraphNode node in nodes)
            {
                if (node.EntryPoint) return;
                edges.Where((item) => { return item.input.node == node; }).ToList().ForEach((edge) => { targetNodeGraphView.RemoveElement(edge); });

                targetNodeGraphView.RemoveElement(node);
            }
        }

        private void CreateNodesFromFile()
        {
            foreach (NodeGraphNodeData nodeData in cachedContainer.nodeData)
            {
                NodeGraphNode tempNode = targetNodeGraphView.GenerateNode(nodeData.TextValue);
                tempNode.NodeGUID = nodeData.NodeGUID;
                targetNodeGraphView.AddElement(tempNode);

                List<NodeGraphNodeLinkData> links = cachedContainer.nodeLinks.Where((item) => { return item.RootNodeGUID == nodeData.NodeGUID; }).ToList();
                links.ForEach((item) => { targetNodeGraphView.AddOutPort(tempNode, item.RootNodePortID); });
            }
        }
    }
}
