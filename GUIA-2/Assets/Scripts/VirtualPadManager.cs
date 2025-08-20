using UnityEngine;
using TMPro;

public class VirtualPadManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI directionText;
    public TextMeshProUGUI touchCountText;

    private Vector2 startPos;
    private bool isTouching = false;

    void Update()
    {
        int touchCount = Input.touchCount;
        touchCountText.text = $"Touches: {touchCount}";

        if (touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    startPos = t.position;
                    isTouching = true;
                    directionText.text = "Direction: Tapped";
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    Vector2 delta = t.position - startPos;

                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        // Movimiento horizontal
                        if (delta.x > 0)
                            directionText.text = "Direction: Right";
                        else
                            directionText.text = "Direction: Left";
                    }
                    else
                    {
                        // Movimiento vertical
                        if (delta.y > 0)
                            directionText.text = "Direction: Up";
                        else
                            directionText.text = "Direction: Down";
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isTouching = false;
                    directionText.text = "Direction: Released";
                    break;
            }
        }
        else
        {
            if (!isTouching)
                directionText.text = "Direction: None";
        }

#if UNITY_EDITOR
        // --- Simulación con mouse en el editor ---
        SimulateWithMouse();
#endif
    }

#if UNITY_EDITOR
    private void SimulateWithMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isTouching = true;
            directionText.text = "Direction: Tapped (Mouse)";
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - startPos;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                    directionText.text = "Direction: Right (Mouse)";
                else
                    directionText.text = "Direction: Left (Mouse)";
            }
            else
            {
                if (delta.y > 0)
                    directionText.text = "Direction: Up (Mouse)";
                else
                    directionText.text = "Direction: Down (Mouse)";
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            directionText.text = "Direction: Released (Mouse)";
            isTouching = false;
        }
    }
#endif
}
