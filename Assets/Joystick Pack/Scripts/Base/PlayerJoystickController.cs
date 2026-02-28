using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerJoystickController : MonoBehaviour
{
    [Header("Ustawienie ruchu")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;
    public float gravity = 10f;
    public float modelRotationOffset = 0f;

    [Header("Link")]
    public VariableJoystick joystick;
    public Animator animator;

    private CharacterController controller;
    private float verticalVelocity;
    private Transform camTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Базові налаштування фізики
        controller.center = new Vector3(0, 1, 0);
    }

    void Update()
    {
        if (joystick == null) return;

        float moveX = joystick.Horizontal;
        float moveZ = joystick.Vertical;

        // Розрахунок напрямку відносно камери
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;
        camForward.y = 0; camRight.y = 0;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDir = (camForward * moveZ) + (camRight * moveX);

        // Гравітація
        // Poprawiona grawitacja dla stabilności na podłożu
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Trzyma postać dociśniętą do ziemi, ale nie kumuluje prędkości
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 velocity = Vector3.zero;

        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            targetRotation *= Quaternion.Euler(0, modelRotationOffset, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            velocity = moveDir * moveSpeed;
            SetWalking(true);
        }
        else
        {
            SetWalking(false);
        }

        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void SetWalking(bool state)
    {
        if (animator != null) animator.SetBool("IsWalking", state);
    }
}