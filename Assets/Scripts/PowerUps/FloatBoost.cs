using UnityEngine;
using TMPro;

public class FloatBoost : MonoBehaviour
{
    [SerializeField]
    private int cost = 0; // Cost in coins
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to UI text displaying the prompt

    private bool playerNearby = false;
    private PlayerController playerController;

    private float originalGravityValue; // To store original gravity for reset if needed
    private float boostedGravityValue = -2.0f; // Adjust to control the floatiness


    AudioManager audioManager;

    private void Awake()
    {
        // Initialization code can go here if needed
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerNearby = true;
                promptText.text = "Float Boost: " + cost + " coins. Press E to buy.";
                promptText.gameObject.SetActive(true);

                // Store original gravity value to reset later if needed
                originalGravityValue = playerController.gravityValue;
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
                // Apply the float boost effect by setting a lower gravity value
                playerController.gravityValue = boostedGravityValue;

                promptText.text = "Float Boost activated!";
                promptText.gameObject.SetActive(false); // Hide prompt after activation

                Destroy(gameObject); // Remove the FloatBoost item from the scene
                audioManager.PlaySFX(audioManager.upgrade);
            }
            else
            {
                promptText.text = "Not enough coins!";
            }
        }
    }
}
