using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour, IDamageable
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

    //-----Other player-----
    private MeshRenderer _meshRenderer;
    private Color _originalColor;

    private IDamageable _targetInRange;

    public float health;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalColor = _meshRenderer.material.color;
    }

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

        if (_canHitEnemy && _targetInRange != null)
            _targetInRange.TakeDamage(basicHitDamage);
    }

    private void DoHeavyHit()
    {
        // TODO: animación de golpe

        if (_canHitEnemy && _targetInRange != null)
            _targetInRange.TakeDamage(heavyHitDamage);
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

    public void NotifyEnemyEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null && other.gameObject != gameObject)
        {
            _canHitEnemy = true;
            _targetInRange = damageable;
        }
    }

    public void NotifyEnemyExit(Collider other)
    {
        if (other.GetComponent<IDamageable>() == _targetInRange)
        {
            _canHitEnemy = false;
            _targetInRange = null;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        StartCoroutine(FlashDamage());

        Debug.Log($"{gameObject.name} recibió daño. Vida: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashDamage()
    {
        _meshRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _meshRenderer.material.color = _originalColor;
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
}