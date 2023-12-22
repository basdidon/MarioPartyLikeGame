using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fusion;
using System.Linq;
using System;
using BasDidon.PathFinder.NodeBase;

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
        NodeData = Resources.Load<NodeData>($"Assets/Resources/NodeDataSet/{NodeType}Data.asset");
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