using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionController : MonoBehaviour
{
    [Header("Interaction")]
    public Vector3 interactionRayPoint = default;
    public float interactionDistance = default;
    public LayerMask interactionLayer = default;
    public KeyCode interactKey = KeyCode.E;
    public Camera playerCamera;

    private Interactable currentInteractable; 

    // Start is called before the first frame update
    void Start()
    {        
        //playerCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInteractionCheck();
        HandleInteractionInput();
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 6 
            && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null 
        && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer)) 
        {
            currentInteractable.OnInteract();
        }
    }
}
