using UnityEngine;

[ExecuteInEditMode]
public class Zoom : MonoBehaviour
{
    Camera playerCamera;
    public float defaultFOV = 60;
    public float maxZoomFOV = 15;
    [Range(0, 1)]
    public float currentZoom;
    public float sensitivity = 1;


    void Awake()
    {
        // Get the playerCamera on this gameObject and the defaultZoom.
        playerCamera = GetComponent<Camera>();
        if (playerCamera)
        {
            defaultFOV = playerCamera.fieldOfView;
        }
    }

    void Update()
    {
        // Update the currentZoom and the playerCamera's fieldOfView.
        currentZoom += Input.mouseScrollDelta.y * sensitivity * .05f;
        currentZoom = Mathf.Clamp01(currentZoom);
        playerCamera.fieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
    }
}
