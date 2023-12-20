using UnityEngine;
using UnityEngine.InputSystem;
using BasDidon.PathFinder.NodeBase;
using System.Collections.Generic;
using System.Linq;

public class IdleState : IState<Player>
{
    public Player StateActor { get; }

    public IdleState(Player player)
    {
        StateActor = player;
    }

    public void EnterState()
    {
        ActionMapper.Instance.OnRolled += OnRollAction;
    }

    public void UpdateState() { }

    public void ExitState()
    {
        ActionMapper.Instance.OnRolled -= OnRollAction;
    }

    // Action Handle
    void OnRollAction()
    {
        var rollResult = Random.Range(1, 6);

        // add raise on rolled events upter this
        Debug.Log($"RollResult : {rollResult}");

        List<NodePath<Node>> NodePaths = PathFinder.FindPathByMove(StateActor.CurrentNode, rollResult).ToList();

        
        if(NodePaths.Count == 1)
        {
            StateActor.State = new AutoSelectNodeState(StateActor, NodePaths[0]);
        }
        else if (NodePaths.Count > 1)
        {
            StateActor.State = new SelectNodeToMoveState(StateActor, NodePaths);
        }
        else
        {
            throw new System.ArgumentOutOfRangeException();
        }
    }
}