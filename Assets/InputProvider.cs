using BasDidon.PathFinder.NodeBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
    Inputs Inputs { get; set; }
    public Player Player { get; private set; }

    // To Read Value
    [field: SerializeField] InputActionReference CursorPositionActionRef { get; set; }
    public InputAction CursorPositionAction => CursorPositionActionRef.action;

    // To Invoke Action
    [field: SerializeField] InputActionReference RollActionRef { get; set; }
    public InputAction RollAction => RollActionRef.action;
    [field: SerializeField] InputActionReference ClickToMoveActionRef { get; set; }
    public InputAction ClickToMoveAction => ClickToMoveActionRef.action;

    [field: SerializeField] InputActionReference TouchActionRef { get; set; }
    public InputAction TouchAction => TouchActionRef.action;
    [field: SerializeField] InputActionReference TouchStartAtActionRef { get; set; }
    public InputAction TouchStartAtAction => TouchStartAtActionRef.action;
    private void OnEnable()
    {
        CursorPositionAction.Enable();

        TouchAction.Enable();
        TouchStartAtAction.Enable();
    }

    private void OnDisable()
    {
        CursorPositionAction.Disable();

        TouchAction.Disable();
        TouchStartAtAction.Disable();
    }

    private void Awake()
    {
        Inputs = new();

        TouchAction.performed += ctx => {
            Debug.Log($"Start : {ctx.action}");
        };

        TouchAction.canceled += ctx =>
        {
            Debug.Log("End");
        };

        TouchStartAtAction.performed += ctx =>
        {
            Debug.Log($"Start at : {ctx.ReadValue<Vector2>()}");
        };

        Inputs.Player.Roll.performed += _ => Player.RollADice();

        Inputs.Player.MouseLeftClick.performed += _ =>
        {
            /*
            RaycastHit[] hits = new RaycastHit[100];
            var screenPoint = Inputs.Player.MousePosition.ReadValue<Vector2>();
            var camRay = Camera.main.ScreenPointToRay(screenPoint);

            int hitsNum = Physics.RaycastNonAlloc(camRay.origin, camRay.direction, hits, 20);
            if (hitsNum > 0)
            {
                if (hits.Take(hitsNum).Any(hit => hit.transform.CompareTag("Node")))
                {
                    var nodeTransfrom = hits.Where(hit => hit.transform.CompareTag("Node")).First().transform;
                    var node = nodeTransfrom.GetComponentInParent<Node>();

                    if (node.CanMoveTo)
                    {
                        CurrentNode = node;
                        NodeRegistry.Instance.ResetCanMoveTo();
                    }
                    else
                    {
                        Debug.Log("you need to select node that you can move to.");
                    }
                }
            }*/
        };
    }
}
