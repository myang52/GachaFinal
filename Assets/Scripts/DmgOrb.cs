using UnityEngine;

public class DmgOrb : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // Damage dealt to the player upon collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the PlayerController component from the player
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // Apply damage to the player
                playerController.RemovePlayerHealth(damageAmount);
                Debug.Log($"Player took {damageAmount} damage from DmgOrb.");

                // Destroy the orb after applying damage
                Destroy(gameObject);
            }
        }
    }
}
