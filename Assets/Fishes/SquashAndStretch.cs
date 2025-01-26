using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [Header("Animation Settings")]
    public float squashAmount = 0.2f; // How much the object squashes/stretches
    public float speed = 2f; // Speed of the effect

    [Header("Axis Settings")]
    public bool squashX = true;
    public bool squashY = true;
    public bool squashZ = false;

    private Vector3 originalScale;
    private float time;
    

    void Start()
    {
        // Store the original scale of the object
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Update time to create an oscillating effect
        time += Time.deltaTime * speed;

        // Calculate squash/stretch values using a sine wave
        float squashValue = Mathf.Sin(time) * squashAmount;

        // Apply squash/stretch on selected axes
        float scaleX = squashX ? originalScale.x * (1 - squashValue) : originalScale.x;
        float scaleY = squashY ? originalScale.y * (1 + squashValue) : originalScale.y;
        float scaleZ = squashZ ? originalScale.z * (1 - squashValue) : originalScale.z;

        // Update the object's scale
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}