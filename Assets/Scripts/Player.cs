using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.PathFinder.NodeBase;
using System.Linq;
using UnityEngine.InputSystem;

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

    public List<NodePath<Node>> NodePaths { get; set;}

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
        IdleState = new IdleState(this);
        State = IdleState;

        if(TryGetComponent(out InputProvider inputProvider))
        {
            InputProvider = inputProvider;
        }
    }

    void Start()
    {
        CurrentNode = NodeRegistry.Instance.startNode;
    }

    private void Update()
    {
        State?.UpdateState();
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

public class IdleState : IState<Player>
{
    public Player StateActor { get; }

    int RollResult { get; set; }

    public IdleState(Player player)
    {
        StateActor = player;
    }

    public void EnterState()
    {
        StateActor.InputProvider.RollAction.Enable();
        StateActor.InputProvider.RollAction.performed += OnRollAction;

        RollResult = 0;
    }

    public void ExitState()
    {
        StateActor.InputProvider.RollAction.Disable();
        StateActor.InputProvider.RollAction.performed -= OnRollAction;
    }

    public void UpdateState()
    {

    }

    // Actions
    public void OnRollAction(InputAction.CallbackContext ctx)
    {
        RollResult = Random.Range(1, 6);
    }
}

public class 