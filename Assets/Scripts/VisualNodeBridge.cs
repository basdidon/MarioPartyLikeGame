using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class VisualNodeBridge : MonoBehaviour
{
    [SerializeField] Material OneDirectionArrowMat;
    [SerializeField] Material BothDirectionArrowMat;

    NodeBridge NodeBridge { get; set; }
    Renderer Renderer { get; set; }

    private void OnEnable()
    {
        Renderer = GetComponent<MeshRenderer>();
        NodeBridge = GetComponentInParent<NodeBridge>();

        if (NodeBridge == null)
        { 
            Debug.LogWarning("not found : NodeBridge in parent.");
            return;
        }

        OnIsOneWaychanged(NodeBridge.IsOneWay);
        NodeBridge.OnIsOneWayChanged += OnIsOneWaychanged;
    }

    private void OnDisable()
    {
        NodeBridge.OnIsOneWayChanged -= OnIsOneWaychanged;
    }

    public void OnIsOneWaychanged(bool newValue)
    {
        if(newValue == true)
        {
            Renderer.material = OneDirectionArrowMat;
        }
        else
        {
            Renderer.material = BothDirectionArrowMat;
        }
    }
}
