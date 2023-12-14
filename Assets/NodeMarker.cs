using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMarker : MonoBehaviour
{
    public Node Node { get; private set; }

    private void Awake()
    {
        Node = GetComponentInParent<Node>();
    }

    private void OnEnable()
    {
        OnCanMoveToChangedHandle(Node.CanMoveTo);
        Node.OnCanMoveToChanged += OnCanMoveToChangedHandle;
    }

    private void OnDisable()
    {
        Node.OnCanMoveToChanged -= OnCanMoveToChangedHandle;
    }

    public void OnCanMoveToChangedHandle(bool canMoveTo)
    {
        gameObject.SetActive(canMoveTo);
    }
}
