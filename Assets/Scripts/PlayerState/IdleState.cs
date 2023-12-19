using UnityEngine;
using UnityEngine.InputSystem;

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

        StateActor.State = new SelectNodeToMoveState(StateActor, rollResult);
    }
}