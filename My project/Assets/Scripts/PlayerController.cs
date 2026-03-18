using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("Force applied to move the ball.")]
    private float speed = 10f;

    [SerializeField, Tooltip("Maximum horizontal speed (m/s).")]
    private float maxSpeed = 6f;

    [Header("Input (new Input System)")]
    [SerializeField, Tooltip("Optional: assign the InputActionAsset if you don't use a PlayerInput component on the same GameObject.")]
    private InputActionAsset actionsAsset = null;

    [SerializeField, Tooltip("Action map name inside the InputActionAsset (default: 'Player').")]
    private string actionMapName = "Player";

    [SerializeField, Tooltip("Move action name inside the action map (default: 'Move').")]
    private string moveActionName = "Move";

    // Cached components
    private Rigidbody rb;

    // The InputAction we will read from
    private InputAction moveAction;

    // Cached input value (x = left/right, y = forward/back)
    private Vector2 moveInput = Vector2.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Smooth visual motion by default
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        // Prefer a PlayerInput component if present (easier in-editor wiring)
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            // Try direct lookup by name first
            moveAction = playerInput.actions?.FindAction(moveActionName);

            // If not found, try current action map
            if (moveAction == null)
                moveAction = playerInput.currentActionMap?.FindAction(moveActionName);
        }

        // Fallback to explicit InputActionAsset provided in the inspector
        if (moveAction == null && actionsAsset != null)
        {
            var map = actionsAsset.FindActionMap(actionMapName);
            if (map != null)
                moveAction = map.FindAction(moveActionName);
        }

        if (moveAction == null)
        {
            Debug.LogWarning($"PlayerController: Move action not found. Please add a PlayerInput component or assign an InputActionAsset and ensure action map '{actionMapName}' contains action '{moveActionName}'.");
        }
    }

    void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
        }
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            moveAction.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // ReadValue works for both performed and canceled
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Convert 2D input to world XZ vector (x => right, y => forward)
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

        if (input.sqrMagnitude > 0.0001f)
        {
            rb.AddForce(input * speed, ForceMode.Force);
        }

        // Clamp horizontal velocity to maxSpeed
        Vector3 horizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float hVel = horizontal.magnitude;
        if (hVel > maxSpeed)
        {
            Vector3 limited = horizontal.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

    // Public setters/getters for runtime tuning
    public float Speed { get => speed; set => speed = Mathf.Max(0f, value); }
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = Mathf.Max(0f, value); }
}

