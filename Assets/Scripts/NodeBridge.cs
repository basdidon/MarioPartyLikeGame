using UnityEngine;
using System;

[ExecuteInEditMode]
public class NodeBridge : MonoBehaviour
{
    readonly Vector3 nodeBridgeOffset = new (0,-.498f);
    [SerializeField] Node from;
    [SerializeField] Node to;
    [SerializeField] bool isOneWay;
    [SerializeField] bool isActive;

    public Node From {
        get => from;
        set
        {
            from = value;
        }
    }
    public Node To { 
        get => to; 
        set => to = value; 
    }

    public bool IsOneWay {
        get => isOneWay;
        set
        {
            isOneWay = value;
            OnIsOneWayChanged?.Invoke(isOneWay);
        }
    }
    public bool IsActive {
        get => isActive;
        set => isActive = value;
    }

    public void Initialize(Node from, Node to, bool isOneWay = true, bool isActive = true)
    {
        From = from;
        To = to;
        IsOneWay = isOneWay;
        IsActive = isActive;

        ToCenter();
    }

    // Events
    public event Action<bool> OnIsOneWayChanged;

    public bool IsOutputOf(Node node, out Node otherNode)
    {
        if(node == from)
        {
            otherNode = to;
            return true;
        }
        else if(!isOneWay && node == to)
        {
            otherNode = from;
            return true;
        }

        otherNode = null;
        return false;
    }

    public bool GetOtherNode(Node node,out Node otherNode)
    {
        otherNode = null;

        if(node == from)
        {
            otherNode = to;
            return true;
        }
        else if(node == to)
        {
            otherNode = from;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ToCenter()
    {
        if (From == null || To == null)
            return;

        var newPos = Vector3.Lerp(From.transform.position, To.transform.position, 0.5f);
        transform.position = newPos;
        transform.LookAt(to.transform.position);
        transform.position += nodeBridgeOffset;
    }

    private void OnDestroy()
    {
        NodeRegistry.Instance.RemoveNodeBridge(this);
    }
}


/*
[CustomPropertyDrawer(typeof(NodeBridge))]
public class NodeBridgePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var conntainer = new VisualElement();
        var txt = new Label() { text = "afg" };

        conntainer.Add(txt);

        return conntainer;
    }
}*/