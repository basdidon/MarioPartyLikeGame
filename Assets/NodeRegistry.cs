using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Utils.UIElements;

[ExecuteInEditMode]
public class NodeRegistry : MonoBehaviour
{
    public static NodeRegistry Instance { get; private set; }

    [SerializeField] List<Node> nodes;
    [SerializeField] List<NodeBridge> nodeBridges;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Setup();
    }

    public void Setup()
    {
        nodes = new List<Node>(FindObjectsByType<Node>(FindObjectsSortMode.None));
        nodeBridges = new List<NodeBridge>(FindObjectsByType<NodeBridge>(FindObjectsSortMode.None));

        foreach (var nodeBridge in nodeBridges)
        {
            nodeBridge.ToCenter();
        }
    }
}

[CustomEditor(typeof(NodeRegistry))]
public class NodeRigistryEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        var setupBtn = new Button() { text = "Setup"};

        var nodesPF = new PropertyField(serializedObject.FindProperty("nodes"));
        var nodeBridgesPF = new PropertyField(serializedObject.FindProperty("nodeBridges"));



        container.Add(Layout.GetDefaultScriptPropertyField(serializedObject));
        container.Add(setupBtn);
        container.Add(nodesPF);
        container.Add(nodeBridgesPF);

        setupBtn.clicked += (target as NodeRegistry).Setup;

        return container;
    }
}
