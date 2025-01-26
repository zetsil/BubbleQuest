using UnityEngine;

public class FishBobbing : MonoBehaviour
{
    public float bobbingAmplitude = 0.1f; // How much the fish bobs up and down
    public float bobbingSpeed = 2f; // How fast the fish bobs

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // Calculate bobbing offset
        float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;

        // Apply bobbing to the fish's position
        transform.position = originalPosition + new Vector3(0, bobbingOffset, 0);
    }
}