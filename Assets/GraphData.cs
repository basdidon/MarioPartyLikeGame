using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using BasDidon.Editor.UiElements;

[CreateAssetMenu(menuName ="ScriptableObjects/Board/Graph",fileName = "GraphData")]
public class GraphData : ScriptableObject
{
    [SerializeField] GameObject NodePrefab;
    [SerializeField] List<NodeData> nodes = new();
    public IReadOnlyList<NodeData> Nodes => nodes;

    public void AddNode(NodeData nodeData)
    {
        nodes.Add(nodeData);
    }

    public void ClearNode()
    {
        nodes.Clear();
    }
}

[CustomEditor(typeof(GraphData))]
public class GraphDataCustomEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        if (target is GraphData graphData)
        {
            var scriptField = BD_PropertyField.GetDefaultScriptRef(serializedObject);

            var btnGroup = new VisualElement();
            btnGroup.style.flexDirection = FlexDirection.Row;

            var createNodeBtn = new Button() { text = "Create Node" };
            createNodeBtn.style.flexGrow = 1;
            var clearNodesBtn = new Button() { text = "Clear Node" };
            clearNodesBtn.style.flexGrow = 1;


            var nodePrefabOF = new ObjectField("NodePrefab")
            {
                bindingPath = "NodePrefab"
            };
            nodePrefabOF.allowSceneObjects = false;

            nodePrefabOF.RegisterValueChangedCallback(evt=> {
                //serializedObject.FindProperty("NodePrefab").objectReferenceValue = evt.newValue;
            });
            //var of = (ObjectField)nodePrefabOF.ElementAt(0);
            //of.allowSceneObjects = false;
            var nodesListField = new PropertyField(serializedObject.FindProperty("nodes"));
            nodesListField.SetEnabled(false);

            root.Add(scriptField);
            btnGroup.Add(createNodeBtn);
            btnGroup.Add(clearNodesBtn);
            root.Add(btnGroup);
            root.Add(nodePrefabOF);

            root.Add(nodesListField);
            
            createNodeBtn.clicked += OnCreateNode;
            clearNodesBtn.clicked += OnClearNodes;

        }

        return root;
    }
    
    public void OnCreateNode()
    {
        if (target is GraphData graphData)
        {
            if (serializedObject.FindProperty("NodePrefab").objectReferenceValue is not GameObject go)
            {
                Debug.LogWarning("NodePrefab can't be null.");
                return;
            }
            // Setup
            CreateFolderIfNotExits("Assets", "Resources");
            CreateFolderIfNotExits("Assets/Resources", "GraphDataSet");

            // create asset instance
            var nodeData = NodeData.CreateInstance(graphData);
            // add that instance to Nodes
            graphData.AddNode(nodeData);

            GameObject _go = PrefabUtility.InstantiatePrefab(new GameObject("as")) as GameObject;

            InstantiateNodeToScene(go,nodeData);
        }
    }

    public void OnClearNodes()
    {
        if(target is GraphData graphData)
        {
            graphData.ClearNode();

            string path = "Assets/Resources/GraphData.asset";

            var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
            Debug.Log(subAssets.Length);

            
            foreach(var subAsset in subAssets)
            {
                if(subAsset == mainAsset)
                    continue;
                
                AssetDatabase.RemoveObjectFromAsset(subAsset);
            }

            AssetDatabase.SaveAssets();
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

    void InstantiateNodeToScene(GameObject prefab, NodeData nodeData)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        go.transform.position = Vector3.zero;

        if(!go.TryGetComponent(out Node node))
        {
            node = go.AddComponent<Node>();
        }

        node.NodeData = nodeData;
    }
}
