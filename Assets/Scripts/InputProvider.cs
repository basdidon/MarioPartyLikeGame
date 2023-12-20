using BasDidon.PathFinder.NodeBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputProvider : MonoBehaviour
{
    public Player Player { get; private set; }

    // To Read Value
    [field: SerializeField] InputActionReference CursorPositionActionRef { get; set; }
    public InputAction CursorPositionAction => CursorPositionActionRef.action;

    // Ui Relate Input
    [field: SerializeField] InputActionReference RollActionRef { get; set; }
    InputAction RollAction => RollActionRef.action;

    // Non UI Relate input
    [field: SerializeField] InputActionReference ClickToMoveActionRef { get; set; }
    public InputAction ClickToMoveAction => ClickToMoveActionRef.action;

    private void OnEnable()
    {
        RollAction.Enable();
        CursorPositionAction.Enable();
        ClickToMoveAction.Enable();
    }

    private void OnDisable()
    {
        RollAction.Disable();
        CursorPositionAction.Disable();
        ClickToMoveAction.Disable();
    }

    Action RollActionEvent;

    private void Awake()
    {
        RollActionEvent = ActionMapper.Instance.GetActionRaiser(ActionMapper.EventName.Roll);

        RollAction.performed += _ => RollActionEvent?.Invoke();
    }
}
