using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fusion;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utils.UIElements;

//[ExecuteInEditMode]
public class Node : MonoBehaviour
{
    [SerializeField] Node nextNode;
    [field: SerializeField] public NodeData NodeData { get; set; }
}

[CustomEditor(typeof(Node))]
public class NodeEditor: Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        container.Add(Layout.GetDefaultScriptPropertyField(serializedObject));

        return container;
    }
}
