using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[ExecuteInEditMode]
public class NodeRegistry : MonoBehaviour
{
    public static NodeRegistry Instance { get; private set; }

    [SerializeField] public Transform NodesTransform { get; private set; }
    [SerializeField] public Transform NodeBridgesTransform { get; private set; }

    [SerializeField] List<Node> nodes;
    public IReadOnlyList<Node> Nodes => nodes;
    [SerializeField] List<NodeBridge> nodeBridges;

    public Node startNode;

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

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Setup()
    {   
        if(NodesTransform == null || NodeBridgesTransform == null)
            return;        
        
        nodes = new List<Node>(NodesTransform.GetComponentsInChildren<Node>());
        nodeBridges = new List<NodeBridge>(NodeBridgesTransform.GetComponentsInChildren<NodeBridge>());

        foreach (var nodeBridge in nodeBridges)
        {
            nodeBridge.ToCenter();
        }
    }

    public void ResetCanMoveTo()
    {
        foreach(var node in nodes)
        {
            node.Selectable = false;
        }
    }

    public IEnumerable<NodeBridge> GetNodeBridgesByNode(Node node)
    {
        return nodeBridges.Where(bridge => bridge.From == node || bridge.To == node);
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    public void RemoveNode(Node nodeToRemove)
    {
        var bridgesToRemove = nodeBridges.Where(bridge => bridge.From == nodeToRemove || bridge.To == nodeToRemove).ToList();
        for (int i = 0; i < bridgesToRemove.Count; i++)
        {
            DestroyImmediate(bridgesToRemove[i].gameObject);
        }

        nodes.Remove(nodeToRemove);
    }

    public void AddNodeBridge(NodeBridge nodeBridge)
    {
        nodeBridges.Add(nodeBridge);
        OnAddedNodeBridge?.Invoke(nodeBridge);
    }

    public void RemoveNodeBridge(NodeBridge nodeBridge)
    {
        nodeBridges.Remove(nodeBridge);
        OnRemovedNodeBridge?.Invoke(nodeBridge);
    }

    // Events
    public event Action<NodeBridge> OnAddedNodeBridge;
    public event Action<NodeBridge> OnRemovedNodeBridge;
}
