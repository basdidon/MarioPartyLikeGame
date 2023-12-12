using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utils.UIElements;
using System;

[ExecuteInEditMode]
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
        set
        {
            isOneWay = value;
            OnIsOneWayChanged?.Invoke(isOneWay);
        }
    }
    public bool IsActive {
        get => isActive;
        set => isActive = value;
    }

    public void Initialize(Node from, Node to, bool isOneWay = true, bool isActive = true)
    {
        From = from;
        To = to;
        IsOneWay = isOneWay;
        IsActive = isActive;

        ToCenter();
    }

    // Events
    public event Action<bool> OnIsOneWayChanged;

    public void ToCenter()
    {
        if (From == null || To == null)
            return;

        var newPos = Vector3.Lerp(From.transform.position, To.transform.position, 0.5f);
        transform.position = newPos;
        transform.LookAt(to.transform.position);
    }

    private void OnDestroy()
    {
        NodeRegistry.Instance.RemoveNodeBridge(this);
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
            if (PrefabUtility.IsPartOfPrefabAsset(target))  // don't do anything if object is partPrefabAsset
                return;

            /// isOneWay is already changed
            /// ,but it change value directly to property
            /// ,so we need to set value again by setter
            /// ,to let it raise event

            if(target is NodeBridge nodeBridge)
                nodeBridge.IsOneWay = evt.changedProperty.boolValue;
        });

        return container;
    }
}