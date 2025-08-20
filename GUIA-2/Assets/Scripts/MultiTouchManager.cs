using UnityEngine;
using TMPro;

public class MultiTouchManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI multiTouchInfo;

    void Update()
    {
        int touchCount = Input.touchCount;
        string info = $"Total touches: {touchCount}\n";

        for (int i = 0; i < touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            info += $"Finger {t.fingerId} - Phase: {t.phase} Pos: {t.position}\n";
        }

#if UNITY_EDITOR
        // Simulación con mouse en Editor
        if (Input.GetMouseButton(0))
        {
            info += $"[Simulación Editor] Mouse Press at {Input.mousePosition}\n";
        }
#endif

        multiTouchInfo.text = info;
    }
}
