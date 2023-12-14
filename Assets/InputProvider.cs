using BasDidon.PathFinder.NodeBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    Inputs Inputs { get; set; }
    public Player Player { get; private set; }

    private void OnEnable()
    {
        Inputs.Enable();
    }

    private void OnDisable()
    {
        Inputs.Disable();
    }

    private void Awake()
    {
        Inputs = new();

        Inputs.Player.Roll.performed += _ => Player.RollADice();

        Inputs.Player.MouseLeftClick.performed += _ =>
        {
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
            }
        };
    }
}
