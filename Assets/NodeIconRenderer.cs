using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NodeIconRenderer : MonoBehaviour
{
    public Node Node { get; private set; }
    public Renderer Renderer { get; private set; }

    private void OnEnable()
    {
        Node = GetComponentInParent<Node>();
        Renderer = GetComponent<MeshRenderer>();

        OnNodeDataChangedHandle(Node.NodeData);
        Node.OnNodeDataChanged += OnNodeDataChangedHandle;
    }

    private void OnDisable()
    {
        Node.OnNodeDataChanged -= OnNodeDataChangedHandle;
    }

    void OnNodeDataChangedHandle(NodeData nodeData) 
    {
        if(nodeData != null && nodeData.Material != null)
        {
            Debug.Log(nodeData.name);
            Renderer.enabled = true;
            Renderer.material = nodeData.Material;
        }
        else
        {
            Renderer.enabled = false;
        }
    }




}
