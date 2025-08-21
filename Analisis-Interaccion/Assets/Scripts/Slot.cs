using UnityEngine;

public class Slot : MonoBehaviour
{
    public int slotId = 1;
    public Transform snapPoint; 
    [HideInInspector] public Movable current;
    public bool IsFree => current == null;
    public bool TryPlace(Movable m, out bool isCorrect)
    {
        isCorrect = (m.targetSlotId == slotId);
        if (!IsFree) return false;

        current = m;
        m.transform.position = snapPoint ? snapPoint.position : transform.position + Vector3.up * 0.25f;
        m.transform.rotation = Quaternion.identity;
        if (m != null) m.MarkPlaced(this, isCorrect);
        return true;
    }
    public void Clear()
    {
        if (current != null) current = null;
    }
}
