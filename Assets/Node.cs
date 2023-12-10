using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fusion;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utils.UIElements;

[ExecuteInEditMode]
public class Node : MonoBehaviour
{
    [SerializeField] Node nextNode;
    [field: SerializeField] public NodeData NodeData { get; set; }
    [SerializeField] List<NodeBridge> nodeBridges;

    public float shieldArea = 10;
    public bool tick = false;

    private void OnEnable()
    {
        nodeBridges = new(NodeRegistry.Instance.GetNodeBridgesByNode(this));
        transform.hasChanged = false;
    }

    public void AdjustAllNodeBridges()
    {
        foreach (var bridge in nodeBridges)
        {
            bridge.ToCenter();
        }
    }
}

[CustomEditor(typeof(Node))]
public class NodeEditor: Editor
{
    bool isSwap;

    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        container.Add(Layout.GetDefaultScriptPropertyField(serializedObject));

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

        // Connect to exits node
        var connectNodesContainer = new VisualElement(); 
        var thisNodeOF = new ObjectField() { objectType = typeof(Node)};
        var targetNodeOF = new ObjectField() { objectType = typeof(Node)};
        var directionLabel = new Label() { text = isSwap?"<-":"->"};
        var swapBtn = new Button() { text = "Swap"};

        thisNodeOF.value = target;
        thisNodeOF.SetEnabled(false);

        var shieldAreaPF = new PropertyField(serializedObject.FindProperty("shieldArea"));

        connectNodesContainer.style.flexDirection = FlexDirection.Row;
        connectNodesContainer.Add(thisNodeOF);
        connectNodesContainer.Add(directionLabel);
        connectNodesContainer.Add(targetNodeOF);
        connectNodesContainer.Add(swapBtn);

        swapBtn.clicked += () =>
        {
            isSwap = !isSwap;
            directionLabel.text = isSwap ? "<-" : "->";

            connectNodesContainer.Remove(thisNodeOF);
            connectNodesContainer.Remove(targetNodeOF);


        };


        // List of bridges
        var nodeBridgesPF = new PropertyField(serializedObject.FindProperty("nodeBridges"));
        //
        container.Add(createOutputNodeBtn);
        container.Add(connectNodesContainer);
        container.Add(nodeBridgesPF);
        container.Add(shieldAreaPF);
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
