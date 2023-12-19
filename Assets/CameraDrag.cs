using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraDrag : MonoBehaviour
{
    Camera Camera { get; set; }

    [field: SerializeField] InputActionReference DragActionRef { get; set; }
    InputAction PressAction => DragActionRef.action;

    [field: SerializeField, ReadOnly] bool IsDragging { get; set; }
    public Vector3 GetMousePosition => Vector3.Scale(Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()), new(1, 0, 1)); // freeze y axis

    [field: SerializeField,ReadOnly] public float YPos { get; set; }

    [field: SerializeField] public float MinX { get; set; }
    [field: SerializeField] public float MinZ { get; set; }
    
    [field: SerializeField] public float MaxX { get; set; }
    [field: SerializeField] public float MaxZ { get; set; }

    [field: SerializeField, ReadOnly] Vector3 Origin { get; set; }
    [field: SerializeField, ReadOnly] Vector3 Difference { get; set; }


    public void OnEnable()
    {
        PressAction.Enable();
    }

    private void OnDisable()
    {
        PressAction.Disable();
    }

    private void Awake()
    {
        Camera = GetComponent<Camera>();

        PressAction.started += _ =>
        {
            Origin = GetMousePosition;
            IsDragging = true;
        };
        PressAction.canceled += _ => IsDragging = false;

        YPos = transform.position.y;
    }

    private void LateUpdate()
    {
        if (!IsDragging)
            return;

        Difference = GetMousePosition - transform.position;
        transform.position = Origin - Difference;
        ClampPosition();
    }


    void ClampPosition() 
    {
        transform.position = new(
            Mathf.Clamp(transform.position.x,MinX,MaxX),
            YPos,
            Mathf.Clamp(transform.position.z,MinZ,MaxZ)
        ); 
    }
}