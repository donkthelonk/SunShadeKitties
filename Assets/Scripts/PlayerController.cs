using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum KittyState { Sun, Shade }

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 14f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Feel")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    public KittyState State { get; private set; } = KittyState.Sun;

    // Subscribe to this to react to Sun/Shade switches (lighting, interactables, visuals, etc.)
    public static event Action<KittyState> OnStateChanged;

    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpCooldown;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += HandleJump;
        inputActions.Player.SwitchState.performed += HandleSwitchState;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= HandleJump;
        inputActions.Player.SwitchState.performed -= HandleSwitchState;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        jumpCooldown -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;

        // Only refresh coyote timer when grounded and not in the post-jump cooldown window
        if (isGrounded && jumpCooldown <= 0f)
            coyoteTimer = coyoteTime;
        else if (!isGrounded)
            coyoteTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            jumpCooldown = 0.2f;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (moveInput.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1f, 1f);
    }

    private void HandleJump(InputAction.CallbackContext ctx)
    {
        jumpBufferTimer = jumpBufferTime;
    }

    private void HandleSwitchState(InputAction.CallbackContext ctx)
    {
        State = State == KittyState.Sun ? KittyState.Shade : KittyState.Sun;
        OnStateChanged?.Invoke(State);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
