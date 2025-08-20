using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameObject manipulatedObject;
    public float movementSpeed = 380.0f;
    public float rotationSpeed = 45f;
    public float scaleSpeed = 0.15f;
    public float minScale = 0.1f;
    public float maxScale = 5f;
    
    void Start()
    {
        if (manipulatedObject == null)
        {
            manipulatedObject = this.gameObject;
            Debug.LogWarning("No object assigned to manipulate. Using this GameObject instead.");
        }
    }

    private void TranslateObject(Vector3 direction)
    {
        if (manipulatedObject == null) return;

        float scaleFactor = Mathf.Max(1f, manipulatedObject.transform.localScale.magnitude);
        manipulatedObject.transform.Translate(direction * movementSpeed * scaleFactor, Space.World);
    }

    #region Movement Methods
    public void MoveUp()
    {
        TranslateObject(Vector3.up);
    }
    
    public void MoveDown()
    {
        TranslateObject(Vector3.down);
    }
    
    public void MoveRight()
    {
        TranslateObject(Vector3.right);
    }
    
    public void MoveLeft()
    {
        TranslateObject(Vector3.left);
    }
    #endregion
    
    #region Rotation Methods
    public void RotateXPositive()
    {
        if (manipulatedObject != null)
        {
            manipulatedObject.transform.Rotate(Vector3.up * rotationSpeed, Space.World);
        }
    }
    
    public void RotateXNegative()
    {
        if (manipulatedObject != null)
        {
            manipulatedObject.transform.Rotate(Vector3.down * rotationSpeed, Space.World);
        }
    }
    #endregion
    
    #region Scale Methods
    public void IncreaseSize()
    {
        if (manipulatedObject != null)
        {
            Vector3 currentScale = manipulatedObject.transform.localScale;

            float factor = 1f + Mathf.Abs(scaleSpeed); 
            Vector3 newScale = currentScale * factor;

            manipulatedObject.transform.localScale = newScale;
            Debug.Log($"Increased size -> {newScale} (factor {factor})");
        }
    }
    
    public void DecreaseSize()
    {
        if (manipulatedObject != null)
        {
            Vector3 currentScale = manipulatedObject.transform.localScale;

            float factor = 1f + Mathf.Abs(scaleSpeed); 
            Vector3 newScale = currentScale / factor;

            newScale.x = Mathf.Max(newScale.x, minScale);
            newScale.y = Mathf.Max(newScale.y, minScale);
            newScale.z = Mathf.Max(newScale.z, minScale);

            manipulatedObject.transform.localScale = newScale;
            Debug.Log($"Decreased size -> {newScale} (factor {factor})");
        }
    }
    #endregion
}
