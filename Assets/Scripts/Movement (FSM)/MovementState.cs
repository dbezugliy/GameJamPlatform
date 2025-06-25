using UnityEngine;

public class MovementState
{
    protected Movement movement;
    protected Rigidbody2D rb;

    public MovementState(Movement movement)
    {
        this.movement = movement;
        this.rb = movement.GetComponent<Rigidbody2D>();
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
