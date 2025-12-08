using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //-----Components-----
    public Rigidbody rb;
    //-----Player Movement-----
    private Vector2 _inputVector;
    
    public float health;
    public float speed;

    void FixedUpdate()
    {
        Vector3 moveDirection = default;
        MovePlayer(moveDirection);
    }

    private void MovePlayer(Vector3 moveDirection)
    {
        moveDirection = (transform.forward * _inputVector.y) + (transform.right * _inputVector.x);
        rb.MovePosition(rb.position + moveDirection * (speed * Time.fixedDeltaTime));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            _inputVector = context.ReadValue<Vector2>();
        }
    }
}
