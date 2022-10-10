using UnityEngine;
using UnityEngine.Video;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    public Transform character;
    public GameObject firstPersonController;
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    public bool isEnteringCutscene = false;
    public enum CameraModes
    {
        IntroCutscene,
        FirstPerson,
        Cutscene,
        EndCutscene
    }
    public CameraModes cameraMode = CameraModes.IntroCutscene;

    Vector2 velocity;
    Vector2 frameVelocity;
    Camera cutsceneCamera;
    Animator animator;
    FirstPersonMovement movementController;


    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        //character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        character = firstPersonController.transform;
        animator = GetComponent<Animator>();
        movementController = firstPersonController.GetComponent<FirstPersonMovement>();
    }

    void LateUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("IntroCutscene"))
        {
            cameraMode = CameraModes.IntroCutscene;
            movementController.canMove = false;
            velocity.x = character.localEulerAngles.y; //useful when FirstPerson camera begins
        } 
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("FirstPerson")) 
        {
            cameraMode = CameraModes.FirstPerson;
            movementController.canMove = true;
        }

        switch (cameraMode)
        {
            case CameraModes.IntroCutscene:
                break;
            case CameraModes.Cutscene:
                if (isEnteringCutscene)
                {
                    print("ENTERING CUTSCENE");
                    UpdateCameraAnimation(character, cutsceneCamera.transform);
                    GetComponent<Camera>().fieldOfView = 20f;
                    //print("Setting camera position to cutsceneCamera : " + cutsceneCamera.transform.position);
                }
                else
                {
                    UpdateCameraAnimation(cutsceneCamera.transform, character);
                    transform.localPosition = Vector3.up * 1.5f;
                    cameraMode = CameraModes.FirstPerson;
                    GetComponent<Camera>().fieldOfView = 60f;
                    print("END OF CUTSCENE");
                }
                break;
            case CameraModes.EndCutscene:
                break;
            case CameraModes.FirstPerson:
                UpdateFirstPersonCamera();
                break;
        }
    }

    void UpdateFirstPersonCamera()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Rotate camera up-down and controller left-right from velocity.
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }

    void UpdateCameraAnimation(Transform origin, Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    public void EnterCutscene(Camera camera)
    {
        cameraMode = CameraModes.Cutscene;
        isEnteringCutscene = true;
        cutsceneCamera = camera;
    }

    public void ExitCutscene()
    {
        //print(GetComponent<Camera>() + " FOV 60");
        isEnteringCutscene = false;
    }
}
