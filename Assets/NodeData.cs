using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CreateAssetMenu(menuName = "ScriptableObjects/Board/Node")]
public class NodeData : ScriptableObject
{
    [field: ReadOnly,SerializeField] public string Guid { get; private set; }
    public int amount = 1;

    public void Init()
    {
        Guid = System.Guid.NewGuid().ToString();
    }

    public static NodeData CreateInstance(GraphData graph)
    {
        var data = CreateInstance<NodeData>();
        data.Init();
        data.name = $"Node : {data.Guid}";

        AssetDatabase.AddObjectToAsset(data, graph);
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssetIfDirty(data);

        return data;
    }
    /*
    public static NodeData CreateAsset(GraphData graph)
    {
        var data = CreateInstance();
        AssetDatabase.CreateAsset(data);
        AssetDatabase.SaveAssets();
        return data;
    }*/
}


[CustomPropertyDrawer(typeof(NodeData))]
public class NodeDataPropertyDrawer : PropertyDrawer
{
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Load Uxml
        /*
        var assetTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UiDoc/GraphData.uxml");
        var root = assetTree.Instantiate();
        */
        // Create property container element.
        var container = new VisualElement();

        var label = new Label("a");

        container.Add(label);
        
        return container;
    }
}