using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.PathFinder.NodeBase;
using System.Linq;

public class Player : MonoBehaviour
{
    public int RollResult { get; private set; }

    Node currentNode;
    public Node CurrentNode
    {
        get => currentNode;
        set
        {
            currentNode = value;
            transform.position = CurrentNode.transform.position;
        }
    }

    public List<NodePath<Node>> NodePaths { get; set;}

    void Start()
    {
        CurrentNode = NodeRegistry.Instance.startNode;
    }

    public void RollADice()
    {
        RollResult = Random.Range(1, 6);

        Debug.Log(RollResult);
        Debug.Log($"nextNodes.Count : {CurrentNode.NextNodes.Count}");

        NodePaths = PathFinder.FindPathByMove(CurrentNode, RollResult).ToList();

        foreach (var path in NodePaths)
        {
            path.Last.CanMoveTo = true;
        }
    }

    public void PredictMove()
    {

    }
}
