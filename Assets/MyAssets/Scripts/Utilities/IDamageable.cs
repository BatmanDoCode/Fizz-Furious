using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damageAmount, Transform attacker, float knockbackForce);
}
