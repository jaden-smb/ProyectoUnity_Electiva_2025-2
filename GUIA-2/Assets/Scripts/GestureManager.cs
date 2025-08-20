using UnityEngine;
using TMPro;

public class GestureManager : MonoBehaviour
{
    [Header("Referencia al objeto 3D")]
    public Transform targetObject;

    [Header("UI")]
    public TextMeshProUGUI infoText;

    private float rotationSpeed = 0.2f;
    private float moveSpeed = 0.01f;
    private float zoomSpeed = 0.01f;

    void Update()
    {
        // Sin toques
        if (Input.touchCount == 0)
        {
            if (infoText != null) infoText.text = "Sin gestos";
            return;
        }

        // Un solo toque → rotación
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                float rotX = t.deltaPosition.y * rotationSpeed;
                float rotY = -t.deltaPosition.x * rotationSpeed;
                targetObject.Rotate(rotX, rotY, 0, Space.World);

                if (infoText != null) infoText.text = "Rotando objeto";
            }
        }

        // Dos toques → zoom o mover en Y
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // --- Zoom con pinch ---
            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = (prev0 - prev1).magnitude;
            float currDist = (t0.position - t1.position).magnitude;
            float diff = currDist - prevDist;

            if (Mathf.Abs(diff) > 2f)
            {
                float scaleChange = diff * zoomSpeed;
                targetObject.localScale += Vector3.one * scaleChange;
                targetObject.localScale = Vector3.Max(targetObject.localScale, Vector3.one * 0.1f);

                if (infoText != null) infoText.text = diff > 0 ? "Zoom In" : "Zoom Out";
            }
            else
            {
                // --- Movimiento en Y ---
                Vector2 avgDelta = (t0.deltaPosition + t1.deltaPosition) / 2f;
                targetObject.position += new Vector3(0, avgDelta.y * moveSpeed, 0);

                if (infoText != null) infoText.text = "Moviendo en Y";
            }
        }
    }
}
