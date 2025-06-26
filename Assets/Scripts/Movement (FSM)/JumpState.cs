using UnityEngine;

public class JumpState : MovementState
{
    public JumpState(Movement movement) : base(movement) { }

    public override void EnterState()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, movement.jumpForce);
    }

    public override void FixedUpdate()
    {
        movement.ApplyMovement();
        movement.ApplyJumpPhysics();
    }

    public override void Update()
    {
        if (rb.linearVelocity.y < 0)
            movement.TransitionToState(movement.fallState);
    }
}

public class FallState : MovementState
{
    public FallState(Movement movement) : base(movement) { }

    public override void FixedUpdate()
    {
        movement.ApplyMovement();
        movement.ApplyJumpPhysics();
    }

    public override void Update()
    {
        if (movement.isGrounded)
        {
            if (movement.IsMoving())
                movement.TransitionToState(movement.runState);
            else
                movement.TransitionToState(movement.idleState);
        }
    }
}