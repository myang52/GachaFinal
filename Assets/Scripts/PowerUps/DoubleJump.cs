using UnityEngine;
using TMPro;

public class DoubleJump : MonoBehaviour
{
    [SerializeField]
    private int cost = 0; // Cost in coins
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to UI text displaying the prompt

    private bool playerNearby = false;
    private PlayerController playerController;
    AudioManager audioManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerNearby = true;
                promptText.text = "Double Jump. Press E to use.";
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
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        if (playerNearby && playerController != null && playerController.interactionAction.triggered)
        {
            if (playerController.SpendCoins(cost))
            {
                playerController.EnableDoubleJump(); // Enable double jump ability on player
                promptText.text = "Double Jump purchased!";
                promptText.gameObject.SetActive(false); // Hide prompt after purchase
                Destroy(gameObject); // Remove the DoubleJump item from the scene
                audioManager.PlaySFX(audioManager.upgrade);
            }
            else
            {
                promptText.text = "Not enough coins!";
            }
        }
    }
}
