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
                    BranchNodeGUID = inputNode.NodeGUID,
                    BranchNodePortID = "In"
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
            cachedContainer = Resources.Load<NodeGraphContainer>($"NodeGraph/{fileName}");
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
            if (cachedContainer.nodeLinks.Count > 0)
            {
                nodes.Find((item) => { return item.EntryPoint; }).NodeGUID = cachedContainer.nodeLinks[0].RootNodeGUID;

                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    if (nodes[i].EntryPoint) return;
                    edges.Where((item) => { return item.input.node == nodes[i]; }).ToList().ForEach((edge) => { targetNodeGraphView.RemoveElement(edge); });

                    targetNodeGraphView.RemoveElement(nodes[i]);
                }
            }
        }

        private void CreateNodesFromFile()
        {
            List<NodeGraphNode> createdNodes = new List<NodeGraphNode>();
            createdNodes.Add(nodes.Find((item) => { return item.EntryPoint; }));
            foreach (NodeGraphNodeData nodeData in cachedContainer.nodeData)
            {
                NodeGraphNode tempNode = targetNodeGraphView.GenerateChoiceNode(nodeData);
                targetNodeGraphView.AddElement(tempNode);

                List<NodeGraphNodeLinkData> links = cachedContainer.nodeLinks.Where((item) => { return item.RootNodeGUID == nodeData.NodeGUID; }).ToList();
                links.ForEach((item) => { targetNodeGraphView.AddChoiceOutPort(tempNode, item.RootNodePortID); });
                if (links.Count == 0)
                {
                    targetNodeGraphView.AddChoiceOutPort(tempNode, "Out 1");
                }
                createdNodes.Add(tempNode);
            }
            foreach (NodeGraphNodeLinkData nodeLink in cachedContainer.nodeLinks)
            {
                Edge edge = new Edge();
                foreach (NodeGraphNode node in createdNodes)
                {
                    if (node.NodeGUID == nodeLink.RootNodeGUID)
                    {
                        Port oPort = targetNodeGraphView.ports.Where((item) => { return item.portName == nodeLink.RootNodePortID && item.node == node; }).First();
                        edge.output = oPort;
                        oPort.Connect(edge);
                    }
                    else if(node.NodeGUID == nodeLink.BranchNodeGUID)
                    {
                        Port iPort = targetNodeGraphView.ports.Where((item) => { return item.portName == "In" && item.node == node; }).First();
                        edge.input = iPort;
                        iPort.Connect(edge);
                    }
                }
                targetNodeGraphView.AddElement(edge);
            }
        }
    }
}