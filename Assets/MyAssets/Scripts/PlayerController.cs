using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //-----Components-----
    public Rigidbody rb;

    //-----Player Movement-----
    private Vector2 _inputVector;

    public float speed;

    //-----Player Jump-----
    public float jumpForce;
    public Transform groundCheck;
    public float groundRadius;
    public LayerMask groundLayer;

    private bool _isGrounded;

    //-----Player Hit-----
    public float basicHitDamage;
    public float heavyHitDamage;

    private bool _canHitEnemy = false;

    private EnemyController _enemyInRange;

    public float health;

    void FixedUpdate()
    {
        Vector3 moveDirection = default;
        MovePlayer(moveDirection);

        _isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
    }

    private void MovePlayer(Vector3 moveDirection)
    {
        moveDirection = (transform.forward * _inputVector.y) + (transform.right * _inputVector.x);
        rb.MovePosition(rb.position + moveDirection * (speed * Time.fixedDeltaTime));
    }

    private void DoBasicHit()
    {
        // TODO: animación de golpe

        if (_canHitEnemy && _enemyInRange != null)
            _enemyInRange.TakeDamage(basicHitDamage);
    }

    private void DoHeavyHit()
    {
        // TODO: animación de golpe

        if (_canHitEnemy && _enemyInRange != null)
            _enemyInRange.TakeDamage(heavyHitDamage);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            _inputVector = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    public void OnBasicHit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DoBasicHit();
        }
    }

    public void OnHeavyHit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DoHeavyHit();
        }
    }

    public void NotifyEnemyEnter(Collider enemy)
    {
        _canHitEnemy = true;
        _enemyInRange = enemy.GetComponent<EnemyController>();
    }

    public void NotifyEnemyExit(Collider enemy)
    {
        _canHitEnemy = false;
        _enemyInRange = null;
    }
}