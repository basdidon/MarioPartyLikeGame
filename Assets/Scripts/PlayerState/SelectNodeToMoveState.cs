using BasDidon.PathFinder.NodeBase;
using BasDidon.TargetSelector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectNodeToMoveState : IState<Player>
{
    public Player StateActor { get; }
    int RollResult { get; }
    List<NodePath<Node>> NodePaths { get; set; }

    RaycastHit[] hits = new RaycastHit[100];
    Node focusNode;

    public SelectNodeToMoveState(Player player, int rollResult)
    {
        StateActor = player;
        RollResult = rollResult;

        NodePaths = PathFinder.FindPathByMove(StateActor.CurrentNode, RollResult).ToList();
    }

    public void EnterState()
    {
        NodePaths.ForEach(path => path.Last.CanMoveTo = true);

        StateActor.InputProvider.ClickToMoveAction.Enable();
        StateActor.InputProvider.ClickToMoveAction.performed += OnClick;
    }

    public void ExitState()
    {
        NodeRegistry.Instance.ResetCanMoveTo();

        StateActor.InputProvider.ClickToMoveAction.performed -= OnClick;
        StateActor.InputProvider.ClickToMoveAction.Disable();
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
