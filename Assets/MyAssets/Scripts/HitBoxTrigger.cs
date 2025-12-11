using UnityEngine;

public class HitBoxTrigger : MonoBehaviour
{
    public PlayerController player;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsConstants.Enemy))
        {
            player.NotifyEnemyEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagsConstants.Enemy))
        {
            player.NotifyEnemyExit(other);
        }
    } 
}
