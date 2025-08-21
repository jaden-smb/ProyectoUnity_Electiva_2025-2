using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class Movable : MonoBehaviour
{
    public int targetSlotId = 1;
    public float placeSearchRadius = 0.6f;
    public bool placed { get; private set; }

    Vector3 startPos;
    Quaternion startRot;
    Vector3 startScale;
    bool startCaptured;
    Slot placedIn;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        if (!startCaptured)
        {
            startPos = transform.position;
            startRot = transform.rotation;
            startScale = transform.localScale;
            startCaptured = true;
        }
    }

    Vector3 dragOffset;
    float zPlane;

    void OnMouseDown()
    {
        if (placed) return;
        if (GameManager.Instance != null && (!GameManager.Instance.IsRunning || !GameManager.Instance.IsDragDropMode)) return;
            zPlane = cam.WorldToScreenPoint(transform.position).z;
            var mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPlane));
            dragOffset = transform.position - mouseWorld;
    }

    void OnMouseDrag()
    {
        if (placed) return;
        if (GameManager.Instance != null && (!GameManager.Instance.IsRunning || !GameManager.Instance.IsDragDropMode)) return;
            var sw = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zPlane);
            var target = cam.ScreenToWorldPoint(sw) + dragOffset;
            transform.position = new Vector3(target.x, 0.25f, target.z);
    }

    void OnMouseUp()
    {
        if (placed) return;
        if (GameManager.Instance != null && (!GameManager.Instance.IsRunning || !GameManager.Instance.IsDragDropMode)) return;
        TryPlaceNearestSlot(notify: true);
    }

    public void TryPlaceNearestSlot(bool notify)
    {
        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.IsRunning) return;
            if (!GameManager.Instance.IsDragDropMode) return;
        }
        var hits = Physics.OverlapSphere(transform.position, placeSearchRadius);
        var slot = hits.Select(h => h.GetComponentInParent<Slot>())
                       .Where(s => s != null && s.IsFree)
                       .OrderBy(s => Vector3.SqrMagnitude(s.transform.position - transform.position))
                       .FirstOrDefault();

        if (slot != null)
        {
            bool correct;
            if (slot.TryPlace(this, out correct))
            {
                if (notify) GameManager.Instance.OnPlaced(this, correct);
                return;
            }
        }
        if (notify && GameManager.Instance != null) GameManager.Instance.OnPlaceFailed(this);
    }

    public void MoveTo(Vector3 worldPos)
    {
        if (placed) return;
        if (GameManager.Instance != null && !GameManager.Instance.IsRunning) return;
            transform.position = new Vector3(worldPos.x, 0.25f, worldPos.z);
    }

    public void MarkPlaced(Slot slot, bool correct)
    {
        placed = true;
        placedIn = slot;
        GameManager.Instance.Bounce(transform);
    }

    public void ResetToStart()
    {
        placed = false;
        placedIn = null;
        if (!startCaptured)
        {
            startPos = transform.position;
            startRot = transform.rotation;
            startScale = transform.localScale;
            startCaptured = true;
        }
        transform.position = startPos;
        transform.rotation = startRot;
        transform.localScale = startScale;
    }
}
