using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    [Header("UI (TextMeshPro)")]
    public TextMeshProUGUI PhaseText;
    public TextMeshProUGUI TouchCountText;
    public TextMeshProUGUI DurationText;

    [Header("Opcional: Volver al menú")]
    public int ReturnToMenuSceneIndex = -1; // si no quieres botón, déjalo -1

    // Estado interno
    private float touchStartTime = 0f;
    // Removed unused field: private bool isTouching = false;
    private Vector2 lastPosition = Vector2.zero;

    void Update()
    {
        // Actualizar el contador de toques
        int tc = Input.touchCount;
        TouchCountText.text = $"Touches: {tc}";

        if (tc > 0)
        {
            // Tomamos el primer touch
            Touch t = Input.GetTouch(0);
            HandleTouch(t.phase, t.position);
        }
        else
        {
            // --- SIMULACIÓN EN EDITOR CON MOUSE (útil para probar sin dispositivo) ---
            #if UNITY_EDITOR
            SimulateMouseAsTouch();
            #else
            // Sin eventos en dispositivo
            SetPhase("None");
            DurationText.text = $"Duration: {0f:0.00}s";
            #endif
        }
    }

    void HandleTouch(TouchPhase phase, Vector2 position)
    {
        switch (phase)
        {
            case TouchPhase.Began:
                touchStartTime = Time.time;
                lastPosition = position;
                SetPhase("Began");
                break;
            case TouchPhase.Stationary:
                SetPhase("Stationary");
                DurationText.text = $"Duration: {Time.time - touchStartTime:0.00}s";
                break;
            case TouchPhase.Moved:
                SetPhase("Moved");
                DurationText.text = $"Duration: {Time.time - touchStartTime:0.00}s";
                lastPosition = position;
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                SetPhase("Ended");
                DurationText.text = $"Duration: {Time.time - touchStartTime:0.00}s";
                break;
            default:
                SetPhase("None");
                break;
        }
    }

    // Helper para actualizar el texto de fase
    void SetPhase(string s)
    {
        if (PhaseText != null) PhaseText.text = $"Phase: {s}";
    }

    #if UNITY_EDITOR
    // Simula un touch con el mouse (Began, Moved/Stationary, Ended).
    void SimulateMouseAsTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartTime = Time.time;
            lastPosition = Input.mousePosition;
            SetPhase("Began (mouse)");
            DurationText.text = $"Duration: {0f:0.00}s";
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 current = (Vector2)Input.mousePosition;
            float dist = Vector2.Distance(current, lastPosition);

            if (dist > 5f) // pequeño umbral para considerar movimiento
            {
                SetPhase("Moved (mouse)");
                lastPosition = current;
            }
            else
            {
                SetPhase("Stationary (mouse)");
            }

            DurationText.text = $"Duration: {Time.time - touchStartTime:0.00}s";
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetPhase("Ended (mouse)");
            DurationText.text = $"Duration: {Time.time - touchStartTime:0.00}s";
        }
        else
        {
            SetPhase("None");
            DurationText.text = $"Duration: {0f:0.00}s";
        }
    }
    #endif

    // Función pública para volver al menú (útil para vincular al botón)
    public void LoadSceneByIndex(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(index);
        else
            Debug.LogWarning("Scene index out of range: " + index);
    }
}
