using UnityEngine;

public class HealthTrap : MonoBehaviour
{
    [SerializeField]
    private float damageAmount = 10f; // Amount of damage to deal to the player

    public void DealDamage()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.RemovePlayerHealth(damageAmount); // Deal damage to the player's health
            Destroy(gameObject); // Destroy the trap after it deals damage
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamage();
        }
    }
}

