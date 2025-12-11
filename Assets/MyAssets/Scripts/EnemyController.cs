using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
}
