using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fusion;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[ExecuteInEditMode]
public class Node : MonoBehaviour
{
    [field: SerializeField] public NodeData NodeData { get; set; }
    public NodeEdge[] nodeEdges;

#if UNITY_EDITOR
    private void OnDestroy()
    {
        Debug.Log("boom!!");
    }
#endif

}

[System.Serializable]
public class NodeEdge
{
    [field: SerializeField] public Node From { get; set; }
    [field: SerializeField] public Node To { get; set; }
    [field: SerializeField] public bool IsOneWay { get; set; }
    [field: SerializeField] public bool IsActive { get; set; }
}
/*
[CustomPropertyDrawer(typeof(NodeEdge))]
public class NodeEdgePD : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        var label = new Label("a");

        container.Add(label);

        return container;
    }
}
*/