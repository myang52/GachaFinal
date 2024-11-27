using UnityEngine;
using TMPro;
using System.Collections;

public class FloatPenalty : MonoBehaviour
{
    [SerializeField]
    private int cost = 0; // Cost in coins
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to UI text displaying the prompt
    [SerializeField]
    private float penaltyDuration = 5f; // Duration of the gravity penalty in seconds
    [SerializeField]
    private float gravityReduction = -10000f; // Amount by which to reduce gravity

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
                promptText.text = "Float penalty: " + cost + " coins. Press E to buy.";
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
                StartCoroutine(ApplyFloatPenalty());
                promptText.gameObject.SetActive(false); // Hide prompt after purchase
                // Do not destroy the object here
            }
            else
            {
                promptText.text = "Not enough coins!";
            }
        }
    }

    private IEnumerator ApplyFloatPenalty()
    {
        float originalGravity = playerController.gravityValue; // Store the original gravity value
        playerController.gravityValue += gravityReduction; // Apply the gravity penalty

        yield return new WaitForSeconds(penaltyDuration); // Wait for the penalty duration

        playerController.gravityValue = originalGravity; // Restore the original gravity value

        Destroy(gameObject); // Now safely destroy the FloatPenalty object
    }
}
