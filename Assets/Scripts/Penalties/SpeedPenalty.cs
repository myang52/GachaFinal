using UnityEngine;
using TMPro;
using System.Collections;

public class SpeedPenalty : MonoBehaviour
{
    [SerializeField]
    private int cost = 0; // Cost in coins
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to UI text displaying the prompt
    [SerializeField]
    private float penaltyDuration = 5f; // Duration of the speed penalty in seconds

    private bool playerNearby = false;
    private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerNearby = true;
                promptText.text = "Speed penalty: " + cost + " coins. Press E to buy.";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerNearby && playerController != null && playerController.interactionAction.triggered)
        {
            if (playerController.SpendCoins(cost))
            {
                StartCoroutine(ApplySpeedPenalty());
                promptText.gameObject.SetActive(false); // Hide prompt after purchase
                // Do not destroy the object here to ensure coroutine completes.
            }
            else
            {
                promptText.text = "Not enough coins!";
            }
        }
    }

    private IEnumerator ApplySpeedPenalty()
    {
        float originalSpeed = playerController.playerSpeed; // Store the original speed
        playerController.playerSpeed -= 4; // Apply the speed penalty

        yield return new WaitForSeconds(penaltyDuration); // Wait for the penalty duration

        playerController.playerSpeed = originalSpeed; // Restore the original speed

        Destroy(gameObject); // Remove the SpeedPenalty object after restoring speed
    }
}
