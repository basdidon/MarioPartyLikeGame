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
        StateActor.InputProvider.RollAction.Enable();
        StateActor.InputProvider.RollAction.performed += OnRollAction;

        ActionMapper.Instance.OnRolled += OnRollAction;
    }

    public void ExitState()
    {
        StateActor.InputProvider.RollAction.Disable();
        StateActor.InputProvider.RollAction.performed -= OnRollAction;

        ActionMapper.Instance.OnRolled -= OnRollAction;
    }

    public void UpdateState() { }

    // Actions
    public void OnRollAction(InputAction.CallbackContext ctx) => OnRollAction();

    public void OnRollAction()
    {
        var rollResult = Random.Range(1, 6);
        Debug.Log($"RollResult : {rollResult}");

        StateActor.State = new SelectNodeToMoveState(StateActor, rollResult);
    }
}