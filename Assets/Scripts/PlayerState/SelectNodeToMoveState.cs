using BasDidon.PathFinder.NodeBase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectNodeToMoveState : IState<Player>
{
    public Player StateActor { get; }
    List<NodePath<Node>> NodePaths { get; set; }

    RaycastHit[] hits = new RaycastHit[100];
    Node focusNode;

    public SelectNodeToMoveState(Player player,List<NodePath<Node>> nodePaths)
    {
        StateActor = player;
        NodePaths = nodePaths;
    }

    public void EnterState()
    {
        NodePaths.ForEach(path => path.Last.Selectable = true);

        StateActor.InputProvider.ClickToMoveAction.performed += OnClick;
    }


    public void ExitState()
    {
        NodeRegistry.Instance.ResetCanMoveTo();

        StateActor.InputProvider.ClickToMoveAction.performed -= OnClick;
    }

    public void UpdateState()
    {
        var screenPoint = StateActor.InputProvider.CursorPositionAction.ReadValue<Vector2>();
        var camRay = Camera.main.ScreenPointToRay(screenPoint);

        int hitsNum = Physics.RaycastNonAlloc(camRay.origin, camRay.direction, hits, 100);
        if (hitsNum > 0 && hits.Take(hitsNum).Any(hit => hit.transform.CompareTag("Node")))
        {
            var nodeTransfrom = hits.Where(hit => hit.transform.CompareTag("Node")).First().transform;
            var node = nodeTransfrom.GetComponentInParent<Node>();
            focusNode = node;
        }
        else
        {
            focusNode = null;
        }
    }

    public void OnClick(InputAction.CallbackContext ctx) {
        Debug.Log("onClick");
        if (NodePaths.Select(path => path.Last).Contains(focusNode))
        {
            Debug.Log("success");
            StateActor.State = new MoveState(StateActor, NodePaths.First(path => path.Last == focusNode).Nodes.GetEnumerator());
        }
    }
}

public class AutoSelectNodeState : IState<Player>
{
    public Player StateActor { get; }
    NodePath<Node> NodePath { get; set; }

    Node focusNode;

    float AutoSelectDelay => 0.5f;
    float timeElapsed;

    public AutoSelectNodeState(Player player,NodePath<Node> nodePath)
    {
        StateActor = player;

        NodePath = nodePath;
    }

    public void EnterState()
    {
        timeElapsed = 0;
        focusNode = NodePath.Last;
        focusNode.Selectable = true;
    }

    public void ExitState()
    {
        NodeRegistry.Instance.ResetCanMoveTo();
    }

    public void UpdateState()
    {
        if (timeElapsed >= AutoSelectDelay)
        {
            StateActor.State = new MoveState(StateActor, NodePath.Nodes.GetEnumerator());
        }
        else
        {
            timeElapsed += Time.deltaTime;
        }
    }
}
