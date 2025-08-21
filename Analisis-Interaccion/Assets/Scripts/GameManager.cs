using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Refs")]
    public List<Movable> movables;
    public List<Slot> slots;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI instructionText;
    public TMP_Dropdown modeDropdown;
    public Button startButton, resetButton, exportButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successClip, errorClip;

    [Header("Config")]
    public int totalToPlace = 5;
    bool running;
    float tStart, tNow;

    public bool IsRunning => running;
    public int CurrentMode => modeDropdown != null ? modeDropdown.value : 0; // 0=DragDrop, 1=TapToMove
    public bool IsDragDropMode => CurrentMode == 0;
    public bool IsTapToMoveMode => CurrentMode == 1;

    int placedCount;
    int wrongAttempts;
    List<string> perObjectLog = new List<string>();

    Movable selected;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    void Start()
    {
        if (timerText == null) Debug.LogError("timerText no está asignado");
        if (countText == null) Debug.LogError("countText no está asignado");
        if (instructionText == null) Debug.LogError("instructionText no está asignado");
        if (modeDropdown == null) Debug.LogError("modeDropdown no está asignado");
        
        UpdateUI();
        
        if (instructionText != null) {
            instructionText.text = "Selecciona un modo y pulsa Iniciar.";
        }
        
        if (startButton != null) startButton.onClick.AddListener(StartRun);
        if (resetButton != null) resetButton.onClick.AddListener(ResetRun);
        if (exportButton != null) exportButton.onClick.AddListener(ExportCSV);
        if (modeDropdown != null) modeDropdown.onValueChanged.AddListener(OnModeChanged);
    }

    void Update()
    {
        if (!running) return;
        tNow = Time.time - tStart;
        
        if (timerText != null) {
            timerText.text = $"Tiempo: {FormatTime(tNow)}";
            if (Time.frameCount % 60 == 0) { 
                Debug.Log($"Actualizando timerText: {FormatTime(tNow)}");
            }
        } else {
            Debug.LogError("timerText no está asignado. Verifica la referencia en el Inspector.");
        }

        if (modeDropdown != null && modeDropdown.value == 1)
        {
            HandleTapToMove();
        }
    }

    void HandleTapToMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 100f))
            {
                var m = hit.collider.GetComponentInParent<Movable>();
                var s = hit.collider.GetComponentInParent<Slot>();

                if (m != null && !m.placed)
                {
                    selected = m;
                    if (instructionText != null)
                        instructionText.text = $"Seleccionado {m.name}. Toca un Slot para colocar.";
                    return;
                }

                if (s != null)
                {
                    if (selected != null)
                    {
                        bool dummyCorrect;
                        if (s.IsFree && s.TryPlace(selected, out dummyCorrect))
                        {
                            OnPlaced(selected, dummyCorrect);
                            selected = null;
                            return;
                        }
                        else
                        {
                            OnPlaceFailed(selected);
                            return;
                        }
                    }
                    else
                    {
                        if (instructionText != null)
                            instructionText.text = "Primero selecciona un objeto.";
                        return;
                    }
                }

                if (selected != null)
                {
                    selected = null;
                    if (instructionText != null)
                        instructionText.text = "Selecciona un objeto para colocarlo en un Slot.";
                }
            }
        }
    }

    public void OnPlaced(Movable m, bool correct)
    {
        placedCount++;
        if (audioSource && successClip) audioSource.PlayOneShot(successClip);
        perObjectLog.Add($"{m.name},{correct},{FormatTime(Time.time - tStart)}");
        UpdateUI();

        if (placedCount >= totalToPlace)
        {
            running = false;
            instructionText.text = "¡Completado! Exporta el CSV para registrar.";
        }
    }

    public void OnPlaceFailed(Movable m)
    {
        wrongAttempts++;
        if (audioSource && errorClip) audioSource.PlayOneShot(errorClip);
        instructionText.text = "Colocación inválida. Intenta de nuevo.";
    }

    public void StartRun()
    {
        ResetRun();
        running = true;
        tStart = Time.time;
        instructionText.text = ((modeDropdown != null && modeDropdown.value == 0)
            ? "Arrastra cada objeto desde A hasta un Slot en B."
            : "Toca un objeto para seleccionarlo y luego un Slot para colocarlo.");
    }

    public void ResetRun()
    {
        running = false;
        placedCount = 0;
        wrongAttempts = 0;
        perObjectLog.Clear();
        
        if (slots != null && slots.Count > 0) {
            foreach (var s in slots) {
                if (s != null) s.Clear();
            }
        }
        if (movables != null && movables.Count > 0) {
            foreach (var m in movables) {
                if (m != null) m.ResetToStart();
            }
        }

        if (slots == null || slots.Count == 0)
        {
            foreach (var s in FindObjectsOfType<Slot>()) s.Clear();
        }
        if (movables == null || movables.Count == 0)
        {
            foreach (var m in FindObjectsOfType<Movable>()) m.ResetToStart();
        }
        
        tNow = 0;
        
        if (timerText != null) {
            timerText.text = "Tiempo: 00:00.000";
            Debug.Log("Reset timerText a: Tiempo: 00:00.000");
        }
        
        UpdateUI();
        selected = null;
    }

    void OnModeChanged(int value)
    {
        ResetRun();
        if (instructionText != null)
            instructionText.text = "Modo cambiado. Pulsa Iniciar para comenzar.";
    }

    void UpdateUI()
    {
        if (countText != null) {
            countText.text = $"Progreso: {placedCount}/{totalToPlace}";
        }
    }

    public void ExportCSV()
    {
        var sb = new StringBuilder();
        sb.AppendLine("userId,mode,totalTime,wrongAttempts");
        string userId = System.DateTime.Now.ToString("yyyyMMdd_HHmm"); 
        string mode = (modeDropdown.value == 0) ? "DragDrop" : "TapToMove";
        sb.AppendLine($"{userId},{mode},{FormatTime(tNow)},{wrongAttempts}");

        sb.AppendLine("objectName,correct,timeAtPlacement");
        foreach (var line in perObjectLog) sb.AppendLine(line);

        string folder =
        #if UNITY_EDITOR
            Application.dataPath + "/Data";
        #else
            Application.persistentDataPath;
        #endif
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, $"run_{userId}.csv");
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

        instructionText.text = $"CSV exportado en:\n{path}";
        Debug.Log("CSV: " + path);
    }

    public void Bounce(Transform t)
    {
        StartCoroutine(BounceCR(t));
    }

    IEnumerator BounceCR(Transform t)
    {
        Vector3 a = t.localScale;
        Vector3 b = a * 1.1f;
        float d = 0.08f, e = 0.06f, t0 = 0;
        while (t0 < d) { t.localScale = Vector3.Lerp(a, b, t0 / d); t0 += Time.deltaTime; yield return null; }
        t0 = 0;
        while (t0 < e) { t.localScale = Vector3.Lerp(b, a, t0 / e); t0 += Time.deltaTime; yield return null; }
        t.localScale = a;
    }

    string FormatTime(float t)
    {
        int minutes = (int)(t / 60f);
        float seconds = t % 60f;
        return $"{minutes:00}:{seconds:00.000}";
    }
}
