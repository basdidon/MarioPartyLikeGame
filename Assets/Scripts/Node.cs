using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fusion;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using BasDidon.PathFinder.NodeBase;
using BasDidon.Editor.UiElements;
using System.Linq;
using System;

public enum NodeTypes
{
    StartNode,
    LandNode,
}

[ExecuteInEditMode]
public class Node : MonoBehaviour, INode<Node>
{
    [SerializeField] Transform nodeMarkerTransform;
    [SerializeField,ReadOnly] bool selectable;
    public bool Selectable {
        get => selectable;
        set
        {
            selectable = value;

            OnSelectableChanged?.Invoke(Selectable);
        }
    }

    [SerializeField] NodeTypes nodeType;
    public NodeTypes NodeType {
        get => nodeType;
        set {
            nodeType = value;
            LoadNodeData();
        }
    }

    [SerializeField] NodeData nodeData;
    public NodeData NodeData { 
        get => nodeData;
        set {
            nodeData = value;
            OnNodeDataChanged?.Invoke(NodeData);
        } 
    }
    [SerializeField] List<NodeBridge> nodeBridges;

    public List<Node> NextNodes => nodeBridges.Select(b => b.IsOutputOf(this, out Node other) ? other : null).Where(n=>n != null).ToList();

    // Events
    public event Action<bool> OnSelectableChanged;
    public event Action<NodeData> OnNodeDataChanged;

    private void OnEnable()
    {
        nodeBridges = new(NodeRegistry.Instance.GetNodeBridgesByNode(this));

        NodeRegistry.Instance.OnAddedNodeBridge += OnAddedNodeBridgeHandle;
        NodeRegistry.Instance.OnRemovedNodeBridge += OnRemovedNodeBridgeHandle;

        transform.hasChanged = false;

        Selectable = false;

        LoadNodeData();
    }

    private void OnDisable()
    {
        NodeRegistry.Instance.OnAddedNodeBridge -= OnAddedNodeBridgeHandle;
        NodeRegistry.Instance.OnRemovedNodeBridge -= OnRemovedNodeBridgeHandle;
    }

    private void OnDestroy()
    {
        NodeRegistry.Instance.RemoveNode(this);
    }

    void LoadNodeData()
    {
        NodeData = AssetDatabase.LoadAssetAtPath<NodeData>($"Assets/Resources/NodeDataSet/{NodeType}Data.asset");
    }

    public void AdjustAllNodeBridges()
    {
        foreach (var bridge in nodeBridges)
        {
            bridge.ToCenter();
        }
    }

    public void OnAddedNodeBridgeHandle(NodeBridge nodeBridge)
    {
        if(nodeBridge.From == this || nodeBridge.To == this)
        {
            nodeBridges.Add(nodeBridge);
        }
    }

    public void OnRemovedNodeBridgeHandle(NodeBridge nodeBridge)
    {
        if (nodeBridge.From == this || nodeBridge.To == this)
        {
            nodeBridges.Remove(nodeBridge);
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(Node))]
public class NodeEditor: Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        container.Add(BD_PropertyField.GetDefaultScriptRef(serializedObject));

        var nodeTypePF = new PropertyField(serializedObject.FindProperty("nodeType"));
        nodeTypePF.RegisterValueChangeCallback(cb => {
            Debug.Log($"{cb.changedProperty.enumNames[cb.changedProperty.enumValueIndex]}");
            (target as Node).NodeType = (NodeTypes) cb.changedProperty.enumValueIndex;
        });
        var selectablePF = new PropertyField(serializedObject.FindProperty("selectable"));

        // Add next node [create nodePrefap and nodeBridgePrefab to sceneView, connent new node by setting on nodeBridge]
        var createOutputNodeBtn = new Button() { text = "Create Output Node" };
        createOutputNodeBtn.clicked += () =>
        {
            if (NodeRegistry.Instance == null)
            {
                Debug.LogWarning("NodeRegistry.Instance is null");
                return;
            }

            NodeRegistry.Instance.CreateOutputNode(target as Node);
        };

        // List of bridges
        var nodeBridgesPF = new PropertyField(serializedObject.FindProperty("nodeBridges"));
        //
        container.Add(nodeTypePF);
        container.Add(selectablePF);
        container.Add(createOutputNodeBtn);
        container.Add(nodeBridgesPF);
        return container;
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.red;

        if (target is Node node)
        {   
            var moveSnap = EditorSnapSettings.move;

            var rightPos = node.transform.position + Vector3.right * moveSnap.x;
            var leftPos = node.transform.position - Vector3.right * moveSnap.x;
            var forwardPos = node.transform.position + Vector3.forward * moveSnap.z;
            var backPos = node.transform.position - Vector3.forward * moveSnap.z;

            DrawCreateNodeBtn(rightPos, node);
            DrawCreateNodeBtn(leftPos, node);
            DrawCreateNodeBtn(forwardPos, node);
            DrawCreateNodeBtn(backPos, node);

            if (Selection.activeGameObject == node.gameObject && node.transform.hasChanged)
            {
                node.AdjustAllNodeBridges();

                node.transform.hasChanged = false;
            }
        }
    }

    void DrawCreateNodeBtn(Vector3 position,Node fromNode)
    {
        if (Handles.Button(position, Quaternion.identity, .2f, .2f, Handles.SphereHandleCap))
        {
            NodeRegistry.Instance.CreateOutputNode(position, fromNode);
        }
    }
}
#endif