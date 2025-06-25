using UnityEngine;

public class RunState : MovementState
{
    public RunState(Movement movement) : base(movement) { }

    public override void FixedUpdate()
    {
        movement.ApplyMovement();
    }

    public override void Update()
    {
        if (!movement.IsMoving())
            movement.TransitionToState(movement.idleState);

        if (movement.IsJumpPressed() && movement.isGrounded)
            movement.TransitionToState(movement.jumpState);
    }
}
