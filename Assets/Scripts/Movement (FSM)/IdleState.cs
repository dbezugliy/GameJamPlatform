using UnityEngine;

public class IdleState : MovementState
{
    public IdleState(Movement movement) : base(movement) { }

    public override void FixedUpdate()
    {
        movement.ApplyMovement();
    }

    public override void Update()
    {
        if (movement.IsMoving())
            movement.TransitionToState(movement.runState);

        if (movement.IsJumpPressed() && movement.isGrounded)
            movement.TransitionToState(movement.jumpState);
    }
}