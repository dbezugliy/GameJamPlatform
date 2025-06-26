using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    //private AirControl airControl;

    //Used for FSM movement
    private MovementState currentState;

    [Header("Movement Settings")]
    public float speed = 10f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask;

    [Header("Better Jump Settings")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Momentum Settings")]
    public float acceleration = 10f;
    public float deceleration = 30f;
    public float reverseDecelerationMultiplier = 1.5f;

    private InputSystem_Actions controls;
    private Vector2 moveInput;
    public bool isGrounded { get; private set; }

    public IdleState idleState { get; private set; }
    public RunState runState { get; private set; }
    public JumpState jumpState { get; private set; }
    public FallState fallState { get; private set; }

    //manual speed not using unitys
    private float horizontalSpeed = 0f;

    void Awake()
    {
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        //airControl = GetComponent<AirControl>();

        // Pre-create state instances
        idleState = new IdleState(this);
        runState = new RunState(this);
        jumpState = new JumpState(this);
        fallState = new FallState(this);

        //Initialize FSM
        TransitionToState(new IdleState(this));
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        //controls.Player.Move.performed += OnMovementPerformed;
        //controls.Player.Move.canceled += OnMovementCanceled;
        //controls.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        //controls.Player.Move.performed -= OnMovementPerformed;
        //controls.Player.Move.canceled -= OnMovementCanceled;
        //controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Disable();
    }

    //FSM
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        currentState.Update();
        //Debug.Log($"velocity {rb.linearVelocity}");
    }

    //private void OnMovementPerformed(InputAction.CallbackContext context)
    //{
    //    moveInput = context.ReadValue<Vector2>();
    //}
    //
    //private void OnMovementCanceled(InputAction.CallbackContext context)
    //{
    //    moveInput = Vector2.zero;
    //}
    //
    //private void OnJumpPerformed(InputAction.CallbackContext context)
    //{
    //    if (isGrounded)
    //    {
    //        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    //    }
    //}
    //
    //void FixedUpdate()
    //{
    //    //is grounded?
    //    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    //
    //    airControl.ApplyMovement(moveInput.x, speed, isGrounded);
    //    
    //    ApplyJumpPhysics();
    //}
    
    //Made to use FSM now
    void FixedUpdate() => currentState.FixedUpdate();

    //needed to transition fsm states
    public void TransitionToState(MovementState newState)
    {
        //Debug.Log($"Transitioning to {newState.GetType().Name}");
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public bool IsMoving() => Mathf.Abs(moveInput.x) > 0.1f;
    public bool IsJumpPressed() => controls.Player.Jump.IsPressed();

    public void ApplyMovement()
    {
        float targetSpeed = moveInput.x * speed;
        float currentSpeed = horizontalSpeed;

        bool reversing = Mathf.Sign(targetSpeed) != 0 && Mathf.Sign(currentSpeed) != Mathf.Sign(targetSpeed);

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? (reversing ? deceleration * reverseDecelerationMultiplier : acceleration): deceleration;

        horizontalSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(horizontalSpeed, rb.linearVelocity.y);

        //rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    //made public because lazy
    public void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            //apply fall multiplier
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        } 
        else if (rb.linearVelocity.y > 0 && !controls.Player.Jump.IsPressed())
        {
            //jump button not held
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
