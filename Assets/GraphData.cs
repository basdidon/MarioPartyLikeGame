using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CreateAssetMenu(menuName ="ScriptableObjects/Board/Graph")]
public class GraphData : ScriptableObject
{
    [field: ReadOnly] public int Id;
    [field: ReadOnly,SerializeField] public int Idx { get; set; }
    [field: ReadOnly, SerializeField] public List<NodeData> Nodes { get; set; }
    [field: SerializeField] public NodeData Node { get; set; }
}

[CustomEditor(typeof(GraphData))]
public class GraphDataCustomEditor : Editor
{
 
    public override VisualElement CreateInspectorGUI()
    {

        //var assetTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UiDoc/GraphData.uxml");
        var root = new VisualElement();

        if (target is GraphData graphData)
        {
            var createNodeBtn = new Button() { text = "Create Node"};
            var listField = ListUiElement.DrawListUiElement(nameof(graphData.Nodes),graphData.Nodes);

            //listField.SetEnabled(false);
            root.Add(createNodeBtn);
            root.Add(listField);

            createNodeBtn.clicked += OnCreateNode;
        }

        return root;
        /*
        var container = new VisualElement();
        container.Add(base.CreateInspectorGUI());
        /*
        if (target is GraphData graphData)
        {
            // Create property container element.

            var row_1 = new VisualElement();
            row_1.style.flexDirection = FlexDirection.Row;
            var label = new Label("id :");
            var idField = new Label($"{graphData.Idx}");

            row_1.Add(label);
            row_1.Add(idField);

            container.Add(row_1);
        }

        return container;*/
    }
    /*
    public override void OnInspectorGUI()
    {
        if(target is GraphData graphData)
        {
            if (GUILayout.Button("Create Node"))
            {
                CreateFolderIfNotExits("Assets", "Resources");
                CreateFolderIfNotExits("Assets/Resources","GraphDataSet");

                var nodeData = NodeData.CreateInstance(graphData);
                
                if (graphData.Nodes == null)
                {
                    graphData.Nodes = new();
                }

                graphData.Nodes.Add(nodeData);
            }

            if (GUILayout.Button("Delete"))
            {
                if (graphData.Nodes == null || graphData.Nodes.Count == 0)
                    return;

                AssetDatabase.RemoveObjectFromAsset(graphData.Nodes[0]);
                AssetDatabase.SaveAssets();
                graphData.Nodes.RemoveAt(0);
            }
        
        }
        base.OnInspectorGUI();
    }
    */

    public void OnCreateNode()
    {
        if (target is GraphData graphData)
        {
            // Setup
            CreateFolderIfNotExits("Assets", "Resources");
            CreateFolderIfNotExits("Assets/Resources", "GraphDataSet");

            if (graphData.Nodes == null)
            {
                graphData.Nodes = new();
            }

            // create asset instance
            var nodeData = NodeData.CreateInstance(graphData);
            // add that instance to Nodes
            graphData.Nodes.Add(nodeData);

            // ****** reset ui
            
        }
    }

    // *** i wiil put this class to util class later
    public void CreateFolderIfNotExits(string parentFolder, string folderToCreate)
    {
        if (!AssetDatabase.IsValidFolder($"{parentFolder}/{folderToCreate}"))
        {
            AssetDatabase.CreateFolder(parentFolder, folderToCreate);
        }
    }
}
