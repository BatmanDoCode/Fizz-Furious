using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health;

    private MeshRenderer _meshRenderer;
    private Color _originalColor;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalColor = _meshRenderer.material.color;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        StartCoroutine(FlashDamage());

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
