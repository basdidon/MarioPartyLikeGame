using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraVisualizer : MonoBehaviour
{
    [field: SerializeField] public float Y = 0.01f;
    [field: SerializeField] public bool IsDrawGizmosOnSelected { get; set; }

    private void OnDrawGizmosSelected()
    {
        if (IsDrawGizmosOnSelected)
            DrawCamaraOnHorizontalPlane();
    }

    Vector3 GetRayPositionAtY(Ray ray, float y = 0) => GetRayPositionAtY(ray.origin, ray.direction, y);
    Vector3 GetRayPositionAtY(Vector3 origin, Vector3 direction, float y = 0)
    {
        //target = origin + (direction * sceler)
        float scaler = (y - origin.y) / direction.y;
        return new Vector3(origin.x + (direction.x * scaler), y, origin.z + (direction.z * scaler));
    }

    void DrawCamaraOnHorizontalPlane()
    {
        var viewportVertices = new Vector2[] { Vector2.zero, Vector2.right, Vector3.one, Vector3.up };
        var rectVertices = new Vector3[4];

        // find vertices
        for (int i = 0; i < 4; i++)
        {
            rectVertices[i] = GetRayPositionAtY(Camera.main.ViewportPointToRay(viewportVertices[i]), Y);
        }

        // draw rect
        for (int i = 0; i < 4; i++)
        {
            if (i == 3)
            {
                Debug.DrawLine(rectVertices[i], rectVertices[0],Color.black);
            }
            else
            {
                Debug.DrawLine(rectVertices[i], rectVertices[i + 1],Color.black);
            }
        }
    }


}
