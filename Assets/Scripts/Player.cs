using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using BasDidon.PathFinder.NodeBase;
using BasDidon.TargetSelector;

public class Player : MonoBehaviour,IStateActor<Player>
{
    [field: SerializeField] public int RollResult { get; private set; }

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

    // public List<NodePath<Node>> NodePaths { get; set;}

    // State
    IState<Player> state;
    public IState<Player> State {
        get => state;
        set
        {
            State?.ExitState();
            state = value ?? IdleState;
            State.EnterState();
        }
    }
    IdleState IdleState { get; set; }

    // Input
    public InputProvider InputProvider { get; private set; }

    private void Awake()
    {
        if(TryGetComponent(out InputProvider inputProvider))
        {
            InputProvider = inputProvider;
            Debug.Log("Player: InputPrevider set");
        }

        IdleState = new IdleState(this);
        State = IdleState;
    }

    void Start()
    {
        CurrentNode = NodeRegistry.Instance.startNode;
    }

    private void Update()
    {
        State?.UpdateState();
    }
    /*
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
    */
    public void PredictMove()
    {

    }
}