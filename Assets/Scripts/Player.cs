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

    public IdleState(Player player)
    {
        StateActor = player;
    }

    public void EnterState()
    {
        StateActor.InputProvider.RollAction.Enable();
        StateActor.InputProvider.RollAction.performed += OnRollAction;
    }

    public void ExitState()
    {
        StateActor.InputProvider.RollAction.Disable();
        StateActor.InputProvider.RollAction.performed -= OnRollAction;
    }

    public void UpdateState() { }

    // Actions
    public void OnRollAction(InputAction.CallbackContext ctx)
    {
        var rollResult = Random.Range(1, 6);
        Debug.Log($"RollResult : {rollResult}");

        StateActor.State = new SelectNodeToMove(StateActor,rollResult);
    }
}

public class SelectNodeToMove : IState<Player>
{
    public Player StateActor { get; }
    int RollResult { get; }
    List<NodePath<Node>> NodePaths { get; set; }

    RaycastHit[] hits = new RaycastHit[100];

    TargetSelector<Node> Selector { get; }

    public SelectNodeToMove(Player player,int rollResult)
    {
        StateActor = player;
        RollResult = rollResult;

        NodePaths = PathFinder.FindPathByMove(StateActor.CurrentNode, RollResult).ToList();

        Selector = new TargetSelector<Node>(
            NodeRegistry.Instance.Nodes,
            (node) => NodePaths.Select(path => path.Last).Contains(node),
            () =>
            {
                var screenPoint = StateActor.InputProvider.CursorPositionAction.ReadValue<Vector2>();
                var camRay = Camera.main.ScreenPointToRay(screenPoint);

                int hitsNum = Physics.RaycastNonAlloc(camRay.origin, camRay.direction, hits, 20);
                if (hitsNum > 0)
                {
                    if (hits.Take(hitsNum).Any(hit => hit.transform.CompareTag("Node")))
                    {
                        var nodeTransfrom = hits.Where(hit => hit.transform.CompareTag("Node")).First().transform;
                        var node = nodeTransfrom.GetComponentInParent<Node>();
                        return node;
                    }
                }
                return null;
            },
            false
        );

        Selector.OnSelectionStart += () => NodePaths.ForEach(path => path.Last.CanMoveTo = true);
        Selector.OnSelectionEnd += () => NodeRegistry.Instance.ResetCanMoveTo();
        Selector.OnSelect += (node) =>
        {
            StateActor.CurrentNode = node;
            StateActor.State = null;        // to Idle State
        };
    }

    public void EnterState()
    {
        Selector.Start();

        StateActor.InputProvider.ClickToMoveAction.Enable();
        StateActor.InputProvider.ClickToMoveAction.performed += OnClick;
    }

    public void ExitState()
    {
        StateActor.InputProvider.ClickToMoveAction.performed -= OnClick;
        StateActor.InputProvider.ClickToMoveAction.Disable();
    }

    public void UpdateState()
    {
        Selector.Update();
    }

    public void OnClick(InputAction.CallbackContext ctx) => Selector.Select();
}