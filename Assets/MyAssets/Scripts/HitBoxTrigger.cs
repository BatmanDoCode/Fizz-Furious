using UnityEngine;

public class HitBoxTrigger : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject)
            return;

        var damageable = other.GetComponent<IDamageable>();

        if (damageable != null) player.NotifyEnemyEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();

        if (damageable != null) player.NotifyEnemyExit(other);
    }
}