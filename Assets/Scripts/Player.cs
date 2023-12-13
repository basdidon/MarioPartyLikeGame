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

            CurrentNode =  PathFinder.FindPathByMove(CurrentNode, rollResult).First().Last;
        };
    }

    void Start()
    {
        CurrentNode = NodeRegistry.Instance.startNode;
    }
}
