using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMarker : MonoBehaviour
{
    public Node Node { get; private set; }
    public Renderer Renderer { get; private set; }

    private void Awake()
    {
        Node = GetComponentInParent<Node>();
        Renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        OnSelectableChangedHandle(Node.Selectable);
        Node.OnSelectableChanged += OnSelectableChangedHandle;
    }

    private void OnDisable()
    {
        Node.OnSelectableChanged -= OnSelectableChangedHandle;
    }

    public void OnSelectableChangedHandle(bool selectable)
    {
        Renderer.enabled = selectable;
    }
}
