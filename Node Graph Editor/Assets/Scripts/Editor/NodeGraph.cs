using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSys
{
    public class NodeGraph : EditorWindow
    {
        private NodeGraphView graphView;

        private void OnEnable()
        {
            ConstructGraph();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        [MenuItem("Graph/Node Graph")]
        public static void OpenNodeGraphWindow()
        {
            EditorWindow window = GetWindow<NodeGraph>();
            window.titleContent = new GUIContent(text: "Node Graph");
        }

        private void ConstructGraph()
        {
            graphView = new NodeGraphView
            {
                name = "Node Graph"
            };

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            GenerateToolbar();
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            Button createNodeButton = new Button(() => { graphView.CreateNode("Node"); });

            createNodeButton.text = "Create Node";
            toolbar.Add(createNodeButton);

            rootVisualElement.Add(toolbar);
        }
    }
}