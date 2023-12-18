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

    bool IsAutoSelect { get; }
    float AutoSelectDelay => 0.5f;
    float timeElapsed;

    public SelectNodeToMoveState(Player player, int rollResult)
    {
        StateActor = player;
        RollResult = rollResult;

        NodePaths = PathFinder.FindPathByMove(StateActor.CurrentNode, RollResult).ToList();
        IsAutoSelect = NodePaths.Count == 1;
    }

    public void EnterState()
    {
        NodePaths.ForEach(path => path.Last.CanMoveTo = true);

        if (IsAutoSelect)
        {
            timeElapsed = 0;
            focusNode = NodePaths[0].Last;
        }
        else
        {
            StateActor.InputProvider.ClickToMoveAction.Enable();
            StateActor.InputProvider.ClickToMoveAction.performed += OnClick;
        }
    }

    public void ExitState()
    {
        NodeRegistry.Instance.ResetCanMoveTo();

        if (!IsAutoSelect)
        {
            StateActor.InputProvider.ClickToMoveAction.performed -= OnClick;
            StateActor.InputProvider.ClickToMoveAction.Disable();
        }
    }
    
    public void UpdateState()
    {
        if (IsAutoSelect)
        {
            if(timeElapsed >= AutoSelectDelay)
            {
                StateActor.State = new MoveState(StateActor, NodePaths.First(path => path.Last == focusNode).Nodes.GetEnumerator());
            }
            else
            {
                timeElapsed += Time.deltaTime;
            }
        }
        else
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
