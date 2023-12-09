using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utils.UIElements;

public class NodeBridge : MonoBehaviour
{
    [SerializeField] Node from;
    [SerializeField] Node to;
    [SerializeField] bool isOneWay;
    [SerializeField] bool isActive;
    
    public Node From {
        get => from;
        set
        {
            from = value;
        }
    }
    public Node To { 
        get => to; 
        set => to = value; 
    }
    public bool IsOneWay {
        get => isOneWay;
        set => isOneWay = value;
    }
    public bool IsActive {
        get => isActive;
        set => isActive = value;
    }
    
    public void ToCenter()
    {
        if (From == null || To == null)
            return;

        var newPos = Vector3.Lerp(From.transform.position, To.transform.position, 0.5f);
        transform.position = newPos;
        transform.LookAt(to.transform.position);
    }
}

[CustomEditor(typeof(NodeBridge))]
public class NodeBridgeEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        var fromNodeField = new PropertyField(serializedObject.FindProperty("from"));
        var toNodeField = new PropertyField(serializedObject.FindProperty("to"));
        var isOneWaySP = serializedObject.FindProperty("isOneWay");
        var isOneWayField = new PropertyField(isOneWaySP);
        var isActiveField = new PropertyField(serializedObject.FindProperty("isActive"));

        var toCenterBtn = new Button() { text = "ToCenter" };

        container.Add(Layout.GetDefaultScriptPropertyField(serializedObject));
        // btn[s]
        container.Add(toCenterBtn);
        // property field
        container.Add(fromNodeField);
        container.Add(toNodeField);
        container.Add(isOneWayField);
        container.Add(isActiveField);

        // Events
        toCenterBtn.clicked += () => {
            (target as NodeBridge).ToCenter();
        };

        isOneWayField.RegisterValueChangeCallback(evt => {
            var newValue = evt.changedProperty.boolValue;
            Debug.Log(newValue);

            var childRenderer = (target as NodeBridge).transform.GetChild(0).GetComponent<Renderer>();
            childRenderer.sharedMaterial.SetInt("_IsOneWay", newValue ? 0 : 1);
        });
        return container;
    }
}