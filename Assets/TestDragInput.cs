using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestDragInput : MonoBehaviour
{
    Inputs Inputs { get; set; }
    public bool isTouch;

    public bool isHold;
    public float dragSpeed = 10;

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

        Inputs.TouchScreen.Hold.performed += _ =>
        {
            isHold = true;
            Debug.Log("Hold performed");
        };

        Inputs.TouchScreen.Hold.canceled += _ =>
        {
            isHold = false;
            Debug.Log("Hold cancled");
        };


        Inputs.TouchScreen.Tap.performed += _ =>
        {
            Debug.Log("tap perform");
        };

        Inputs.TouchScreen.Tap.canceled += _ =>
        {
            Debug.Log("tap cancle");
        };

        Inputs.TouchScreen.Press.performed += _ =>
        {
            Debug.Log(_.phase);

            Debug.Log($"press performed ");
        };
        Inputs.TouchScreen.Press.canceled += _ => Debug.Log("press cancled");
    }

    private void Update()
    {
        if (isHold)
        {
            var holdInput = Inputs.TouchScreen.Delta.ReadValue<Vector2>();
            Camera.main.transform.position += new Vector3(-holdInput.x * dragSpeed, 0, -holdInput.y * dragSpeed);
        }
    }

    void PressHandle(InputAction.CallbackContext ctx)
    {

    }
}
