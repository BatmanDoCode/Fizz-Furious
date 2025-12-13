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

    public float knockbackBasicForce = 2f;
    public float knockbackHeavyForce = 5f;
    public float knockbackUpForce = 1.5f;

    public float heavyChargeTime = 1f;

    private float _heavyChargeTimer;
    private bool _isChargingHeavy;
    private bool _heavyHitExecuted;

    public ParticleSystem basicHitFX;
    public ParticleSystem heavyHitFX;

    //-----Other player-----
    private MeshRenderer _meshRenderer;
    private Color _originalColor;

    private IDamageable _targetInRange;

    //-----Audio Source-----
    [Header("Audio")] 
    public AudioSource audioSource;

    public AudioClip basicHitsFX;
    public AudioClip heavyHitsFX;
    public AudioClip missHitsFX;
    public AudioClip heavyChargesFX;

    [Header("Hit Stop")] 
    public float hitStopDuration = 0.07f;
    
    public float health;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalColor = _meshRenderer.material.color;
    }
    
    private void Update()
    {
        if (!_isChargingHeavy || _heavyHitExecuted)
            return;

        _heavyChargeTimer += Time.deltaTime;

        if (_heavyChargeTimer >= heavyChargeTime)
        {
            ExecuteHeavyHit();
        }
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
        {
            audioSource.PlayOneShot(basicHitsFX);

            StartCoroutine(HitsStop(hitStopDuration));
            
            _targetInRange.TakeDamage(basicHitDamage, transform, knockbackBasicForce);
        }
        else
        {
            audioSource.PlayOneShot(missHitsFX);
        }
    }

    private void DoHeavyHit()
    {
        // TODO: animación de golpe

        if (_canHitEnemy && _targetInRange != null)
            _targetInRange.TakeDamage(heavyHitDamage, transform, knockbackHeavyForce);
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
            StartHeavyCharge();
        }

        if (context.canceled)
        {
            CancelHeavyCharge();
        }
    }
    
    private void StartHeavyCharge()
    {
        _isChargingHeavy = true;
        _heavyHitExecuted = false;
        _heavyChargeTimer = 0f;

        // TODO: animación de carga
        if (heavyChargesFX != null)
        {
            audioSource.clip = heavyChargesFX;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    private void ExecuteHeavyHit()
    {
        _heavyHitExecuted = true;
        _isChargingHeavy = false;
        
        audioSource.Stop();
        
        if (_canHitEnemy && _targetInRange != null)
        {
            audioSource.PlayOneShot(heavyHitsFX);

            StartCoroutine(HitsStop(hitStopDuration * 1.5f));

            DoHeavyHit();
        }
        else
        {
            audioSource.PlayOneShot(missHitsFX);
        }

        _heavyChargeTimer = 0f;

        // TODO: animación fuerte
        // TODO: cámara shake
        // TODO: sonido potente
    }
    
    private void CancelHeavyCharge()
    {
        if (_heavyHitExecuted)
            return;

        _isChargingHeavy = false;
        _heavyChargeTimer = 0f;

        // TODO: animación de cancelación
        audioSource.Stop();
        audioSource.loop = false;
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

    public void TakeDamage(float damage, Transform attacker, float knockbackForce)
    {
        health -= damage;

        PlayHitImpact(attacker, knockbackForce);
        
        StartCoroutine(FlashDamage());
        ApplyKnockback(attacker, knockbackForce);

        Debug.Log($"{gameObject.name} recibió daño. Vida: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void PlayHitImpact(Transform attraker, float knockbackForce)
    {
        ParticleSystem fxToUse = null;

        if (Mathf.Approximately(knockbackForce, knockbackHeavyForce))
        {
            fxToUse = heavyHitFX;
        }
        else
        {
            fxToUse = basicHitFX;
        }

        if (fxToUse == null) return;

        Vector3 hitDirection = (transform.position - attraker.position).normalized;
        Vector3 spawnPosition = transform.position + hitDirection * 0.5f;

        ParticleSystem fx = Instantiate(fxToUse, spawnPosition, Quaternion.LookRotation(hitDirection));
        
        Destroy(fx.gameObject, 1f);
    }

    private IEnumerator FlashDamage()
    {
        _meshRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _meshRenderer.material.color = _originalColor;
    }
    
    private void ApplyKnockback(Transform attacker, float knockbackForce)
    {
        if (rb == null) return;

        Vector3 direction = (transform.position - attacker.position).normalized;

        // empuje horizontal
        Vector3 force = direction * knockbackForce;

        // pequeño empuje hacia arriba
        force.y = knockbackUpForce;

        rb.AddForce(force, ForceMode.Impulse);
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }

    private IEnumerator HitsStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}