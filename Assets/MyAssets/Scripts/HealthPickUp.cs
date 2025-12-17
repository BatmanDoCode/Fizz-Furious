using System;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private float maxHealthReduction = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.Heal(healAmount, maxHealthReduction);
            Destroy(gameObject);
        }
    }
}
