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

[ExecuteInEditMode]
public class Node : MonoBehaviour, INode<Node>
{
    [SerializeField] Transform nodeMarkerTransform;
    [SerializeField] bool canMoveTo;
    public bool CanMoveTo{
        get => canMoveTo;
        set
        {
            canMoveTo = value;

            OnCanMoveToChanged?.Invoke(CanMoveTo);
        }
    }



    [field: SerializeField] public NodeData NodeData { get; set; }
    [SerializeField] List<NodeBridge> nodeBridges;

    public List<Node> NextNodes => nodeBridges.Select(b => b.IsOutputOf(this, out Node other) ? other : null).Where(n=>n != null).ToList();

    // Events
    public event Action<bool> OnCanMoveToChanged;


    private void OnEnable()
    {
        nodeBridges = new(NodeRegistry.Instance.GetNodeBridgesByNode(this));

        NodeRegistry.Instance.OnAddedNodeBridge += OnAddedNodeBridgeHandle;
        NodeRegistry.Instance.OnRemovedNodeBridge += OnRemovedNodeBridgeHandle;

        transform.hasChanged = false;

        CanMoveTo = false;
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

[CustomEditor(typeof(Node))]
public class NodeEditor: Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        container.Add(BD_PropertyField.GetDefaultScriptRef(serializedObject));

        var canMoveToPF = new PropertyField(serializedObject.FindProperty("canMoveTo"));

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
        container.Add(canMoveToPF);
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
