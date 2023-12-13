using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.PathFinder.NodeBase;
using System.Linq;

public class Player : MonoBehaviour
{
    Inputs Inputs { get; set; }
    Node currentNode;
    Node CurrentNode
    {
        get => currentNode;
        set
        {
            currentNode = value;
            transform.position = CurrentNode.transform.position;
        }
    }

    IEnumerable<NodePath<Node>> NodePaths;

    private void OnEnable()
    {
        Inputs.Enable();
    }

    private void OnDisable()
    {
        Inputs.Disable();
    }

    private void Awake()
    {
        Inputs = new();

        Inputs.Player.Roll.performed += _ =>
        {
            var rollResult = Random.Range(1, 6);
            
            Debug.Log(rollResult);
            Debug.Log($"nextNodes.Count : {CurrentNode.NextNodes.Count}");

            NodePaths = PathFinder.FindPathByMove(CurrentNode, rollResult);
            CurrentNode =  NodePaths.First().Last;
        };

        Inputs.Player.MouseLeftClick.performed += _ =>
        {
            RaycastHit[] hits = new RaycastHit[100];
            var screenPoint = Inputs.Player.MousePosition.ReadValue<Vector2>();
            var camRay = Camera.main.ScreenPointToRay(screenPoint);

            int hitsNum = Physics.RaycastNonAlloc(camRay.origin, camRay.direction, hits, 20);
            if (hitsNum > 0)
            {
                if(hits.Take(hitsNum).Any(hit => hit.transform.CompareTag("Node")))
                {
                    var nodeTransfrom = hits.Where(hit => hit.transform.CompareTag("Node")).First().transform;
                    var node = nodeTransfrom.GetComponentInParent<Node>();
                    CurrentNode = node;
                }
            }
        };
    }

    void Start()
    {
        CurrentNode = NodeRegistry.Instance.startNode;
    }
}
