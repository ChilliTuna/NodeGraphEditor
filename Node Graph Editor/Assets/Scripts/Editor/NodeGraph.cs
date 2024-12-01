using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSys
{
    public class NodeGraph : EditorWindow
    {
        private NodeGraphView graphView;
        private string fileName = "New Graph";

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

            TextField fileNameField = new TextField("File Name");
            fileNameField.SetValueWithoutNotify(fileName);
            fileNameField.MarkDirtyRepaint();
            fileNameField.RegisterValueChangedCallback((callback) => { fileName = callback.newValue; });

            toolbar.Add(new Button(() => { graphView.CreateNode("Node"); }) { text = "Create Node"});
            toolbar.Add(new Button(() => { AttemptSaveOrLoad(TransactionType.Save); }) { text = "Save" });
            toolbar.Add(new Button(() => { AttemptSaveOrLoad(TransactionType.Load); }) { text = "Load" });
            toolbar.Add(fileNameField);

            rootVisualElement.Add(toolbar);
        }

        enum TransactionType
        {
            Save,
            Load
        }

        private void AttemptSaveOrLoad(TransactionType type)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                EditorUtility.DisplayDialog("Invalid filename!", "Please enter a valid filename.", "OK");
                return;
            }

            NodeGraphSerialisationUtility serialisationUtility = NodeGraphSerialisationUtility.GetInstance(graphView);
            switch (type)
            {
                case TransactionType.Save:
                    serialisationUtility.SaveGraph(fileName);
                    break;
                case TransactionType.Load:
                    serialisationUtility.LoadGraph(fileName);
                    break;
                default:
                    break;
            }
        }
    }
}