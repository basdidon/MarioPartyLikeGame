using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utils.UIElements;

[CreateAssetMenu(menuName = "ScriptableObjects/Board/Node")]
public class NodeData : ScriptableObject
{
    [field: SerializeField] public string Guid { get; private set; }
    [SerializeField] public int amount = 1;
    public int myInt = 42;

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
        var container = new VisualElement();
        
        

        var guidLabel = new TextField("Guid")
        {
            bindingPath = "Guid"
        };

        guidLabel.SetEnabled(false);

        if(property.objectReferenceValue is NodeData nodeData)
        {
            guidLabel.value = nodeData.Guid;
            container.Add(Layout.DrawDefaultScriptObjectField("NodeData",nodeData));
        }

        /*
        SerializedProperty serializedPropertyMyInt = property.FindPropertyRelative("myInt");

        Debug.Log("myInt " + serializedPropertyMyInt.intValue);

        Debug.Log(property.FindPropertyRelative("amount").intValue);
        */

        container.Add(guidLabel);

        return container;
    }
}