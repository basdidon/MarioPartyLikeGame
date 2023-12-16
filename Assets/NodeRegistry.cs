using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System.Linq;
using System;
using BasDidon.Editor.UiElements;
using BasDidon.ObjectPool;

[ExecuteInEditMode]
public class NodeRegistry : MonoBehaviour
{
    public static NodeRegistry Instance { get; private set; }

    [SerializeField] Transform nodesTransform;
    [SerializeField] Transform nodeBridgesTransform;

    [SerializeField] List<Node> nodes;
    public IReadOnlyList<Node> Nodes => nodes;
    [SerializeField] List<NodeBridge> nodeBridges;

    GameObject nodePrefab;
    GameObject nodeBridgePrefab;

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

        nodePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/NodeObjects/Node.prefab");
        nodeBridgePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/NodeObjects/NodeBridge.prefab");

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
        if(nodesTransform == null || nodeBridgesTransform == null)
            return;        
        
        nodes = new List<Node>(nodesTransform.GetComponentsInChildren<Node>());
        nodeBridges = new List<NodeBridge>(nodeBridgesTransform.GetComponentsInChildren<NodeBridge>());

        foreach (var nodeBridge in nodeBridges)
        {
            nodeBridge.ToCenter();
        }
    }

    public void ResetCanMoveTo()
    {
        foreach(var node in nodes)
        {
            node.CanMoveTo = false;
        }
    }

    public Node CreateNode()
    {
        var nodeGo = PrefabUtility.InstantiatePrefab(nodePrefab, nodesTransform) as GameObject;
        if (!nodeGo.TryGetComponent(out Node node))
        {
            node = nodeGo.AddComponent<Node>();
        }

        nodes.Add(node);

        return node;
    }

    public void RemoveNode(Node nodeToRemove)
    {
        var bridgesToRemove = nodeBridges.Where(bridge => bridge.From == nodeToRemove || bridge.To == nodeToRemove).ToList();
        for(int i=0;i<bridgesToRemove.Count;i++)
        {
            DestroyImmediate(bridgesToRemove[i].gameObject);
        }

        nodes.Remove(nodeToRemove);
    }

    public NodeBridge CreateNodeBridge(Node fromNode,Node toNode,bool isOneWay = true,bool isActive = true)
    {
        var nodeBridgeGO = PrefabUtility.InstantiatePrefab(nodeBridgePrefab, nodeBridgesTransform) as GameObject;

        if (!nodeBridgeGO.TryGetComponent(out NodeBridge nodeBridge))
        {
            nodeBridge = nodeBridgeGO.AddComponent<NodeBridge>();
        }

        nodeBridge.Initialize(fromNode, toNode,isOneWay,isActive);

        nodeBridges.Add(nodeBridge);
        OnAddedNodeBridge?.Invoke(nodeBridge);

        return nodeBridge;
    }

    public void RemoveNodeBridge(NodeBridge nodeBridge)
    {
        nodeBridges.Remove(nodeBridge);
        OnRemovedNodeBridge?.Invoke(nodeBridge);
    }

    public void CreateOutputNode(Node fromNode)
    {
        CreateOutputNode(fromNode.transform.position,fromNode);
    }

    public void CreateOutputNode(Vector3 targetPos, Node fromNode)
    {
        var toNode = CreateNode();
        toNode.transform.position = targetPos;

        CreateNodeBridge(fromNode, toNode);

        Selection.activeObject = toNode;
    }

    public IEnumerable<NodeBridge> GetNodeBridgesByNode(Node node)
    {
        return nodeBridges.Where(bridge => bridge.From == node || bridge.To == node);
    }

    // Events
    public event Action<NodeBridge> OnAddedNodeBridge;
    public event Action<NodeBridge> OnRemovedNodeBridge;
}

[CustomEditor(typeof(NodeRegistry))]
public class NodeRigistryEditor : Editor
{
    enum EdgeDirections { Normal, Inverese, Both }
    EdgeDirections edgeDirection = EdgeDirections.Normal;

    Node l_Node;
    Node r_Node;

    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        var startNodePF = new PropertyField(serializedObject.FindProperty("startNode"));

        var nodesTranformPF = new PropertyField(serializedObject.FindProperty("nodesTransform"));
        var nodeBridgesTranformPF = new PropertyField(serializedObject.FindProperty("nodeBridgesTransform"));

        var setupBtn = new Button() { text = "Setup"};

        var nodesPF = new PropertyField(serializedObject.FindProperty("nodes"));
        var nodeBridgesPF = new PropertyField(serializedObject.FindProperty("nodeBridges"));

        var poolManagerPF = new PropertyField(serializedObject.FindProperty("poolManager"));

        // Connect to exits node
        var connectNodesContainer = new VisualElement();
        var L_NodeOF = new ObjectField() { objectType = typeof(Node), value = l_Node };
        var R_NodeOF = new ObjectField() { objectType = typeof(Node), value = r_Node };
        var swapBtn = new Button() { text = GetBtnIcon() };
        var createBridgeBtn = new Button() { text = "Create Bridge" };

        connectNodesContainer.style.flexDirection = FlexDirection.Row;
        connectNodesContainer.Add(L_NodeOF);
        connectNodesContainer.Add(swapBtn);
        connectNodesContainer.Add(R_NodeOF);
        connectNodesContainer.Add(createBridgeBtn);

        L_NodeOF.RegisterValueChangedCallback(evt =>
        {
            l_Node = evt.newValue as Node;
        });

        R_NodeOF.RegisterValueChangedCallback(evt =>
        {
            r_Node = evt.newValue as Node;
        });

        swapBtn.clicked += () =>
        {
            var enumIdx = (int)edgeDirection;
            enumIdx = (enumIdx + 1) % Enum.GetValues(typeof(EdgeDirections)).Length;
            edgeDirection = (EdgeDirections) enumIdx;

            swapBtn.text = GetBtnIcon();
        };

        createBridgeBtn.clicked += ()=> OnCreateEdgeBtnClicked();

        container.Add(BD_PropertyField.GetDefaultScriptRef(serializedObject));
        container.Add(startNodePF);
        container.Add(nodesTranformPF);
        container.Add(nodeBridgesTranformPF);
        container.Add(poolManagerPF);
        container.Add(setupBtn);
        container.Add(connectNodesContainer);
        container.Add(nodesPF);
        container.Add(nodeBridgesPF);

        setupBtn.clicked += (target as NodeRegistry).Setup;

        return container;
    }

    private void OnCreateEdgeBtnClicked()
    {
        if (target is NodeRegistry nodeRegistry && l_Node != null && r_Node != null)
        {
            if (edgeDirection == EdgeDirections.Normal)
            {
                nodeRegistry.CreateNodeBridge(l_Node,r_Node);
            }else if(edgeDirection == EdgeDirections.Inverese)
            {
                nodeRegistry.CreateNodeBridge(r_Node, l_Node);
            }
            else
            {
                nodeRegistry.CreateNodeBridge(l_Node, r_Node, false);
            }
        }
    }
    

    string GetBtnIcon()
    {
        if(edgeDirection == EdgeDirections.Normal)
        {
            return "-->";
        }
        else if(edgeDirection == EdgeDirections.Inverese)
        {
            return "<--";
        }
        else
        {
            return "<O>";
        }
    }
}
