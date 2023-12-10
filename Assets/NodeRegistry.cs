using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Utils.UIElements;
using System.Linq;

[ExecuteInEditMode]
public class NodeRegistry : MonoBehaviour
{
    public static NodeRegistry Instance { get; private set; }

    [SerializeField] Transform nodesTransform;
    [SerializeField] Transform nodeBridgesTransform;

    [SerializeField] List<Node> nodes;
    [SerializeField] List<NodeBridge> nodeBridges;

    GameObject nodePrefab;
    GameObject nodeBridgePrefab;

    private void Awake()
    {
        Debug.Log("aw");
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

    public void Test()
    {
        Debug.Log(Instance == null);
    }

    public Node CreateNode()
    {
        var nodeGo = PrefabUtility.InstantiatePrefab(nodePrefab, nodesTransform) as GameObject;
        if (!nodeGo.TryGetComponent(out Node node))
        {
            node = nodeGo.AddComponent<Node>();
        }

        return node;
    }

    public NodeBridge CreateNodeBridge(Node fromNode,Node toNode)
    {
        var nodeBridgeGO = PrefabUtility.InstantiatePrefab(nodeBridgePrefab, nodeBridgesTransform) as GameObject;

        if (!nodeBridgeGO.TryGetComponent(out NodeBridge nodeBridge))
        {
            nodeBridge = nodeBridgeGO.AddComponent<NodeBridge>();
        }

        nodeBridge.Initialize(fromNode, toNode);

        return nodeBridge;
    }

    public void CreateOutputNode(Node fromNode)
    {
        CreateOutputNode(fromNode.transform.position,fromNode);
    }

    public void CreateOutputNode(Vector3 targetPos, Node fromNode)
    {
        var toNode = CreateNode();
        toNode.transform.position = targetPos;

        var nodeBridge = CreateNodeBridge(fromNode, toNode);


        nodes.Add(toNode);
        nodeBridges.Add(nodeBridge);

        Selection.activeObject = toNode;
    }

    public IEnumerable<NodeBridge> GetNodeBridgesByNode(Node node)
    {
        return nodeBridges.Where(bridge => bridge.From == node || bridge.To == node);
    }
}

[CustomEditor(typeof(NodeRegistry))]
public class NodeRigistryEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        var nodesTranformPF = new PropertyField(serializedObject.FindProperty("nodesTransform"));
        var nodeBridgesTranformPF = new PropertyField(serializedObject.FindProperty("nodeBridgesTransform"));

        var setupBtn = new Button() { text = "Setup"};

        var nodesPF = new PropertyField(serializedObject.FindProperty("nodes"));
        var nodeBridgesPF = new PropertyField(serializedObject.FindProperty("nodeBridges"));

        container.Add(Layout.GetDefaultScriptPropertyField(serializedObject));
        container.Add(nodesTranformPF);
        container.Add(nodeBridgesTranformPF);
        container.Add(setupBtn);
        container.Add(nodesPF);
        container.Add(nodeBridgesPF);

        setupBtn.clicked += (target as NodeRegistry).Setup;

        return container;
    }
}
