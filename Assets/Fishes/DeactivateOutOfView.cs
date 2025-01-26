using UnityEngine;

public class DeactivateOutOfView : MonoBehaviour
{
    private Camera mainCamera;
    private bool isVisible = true;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check if the object is within the camera's view frustum
        isVisible = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(mainCamera), 
                                                 GetComponent<Renderer>().bounds);

        // Deactivate if not visible
        if (!isVisible)
        {
            gameObject.SetActive(false);
        }
        // Reactivate if previously deactivated and now visible
        else if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
}