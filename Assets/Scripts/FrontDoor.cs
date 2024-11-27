using UnityEngine;
using TMPro;

public class FrontDoor : MonoBehaviour
{




    AudioManager audioManager;
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to UI text displaying the prompt

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
                if (!playerController.hasKey)
                {
                    promptText.text = "You need the magic key. The boss must have it!";
                    promptText.gameObject.SetActive(true);
                }
                else
                {
                    promptText.text = "Press E to unlock the door.";
                    promptText.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            promptText.gameObject.SetActive(false); // Hide the prompt when the player leaves
        }
    }

    private void Update()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        if (playerNearby && playerController != null && playerController.interactionAction.triggered)
        {
            if (playerController.hasKey)
            {
                // Destroy the door and hide the prompt
                promptText.gameObject.SetActive(false); // Ensure the text disappears
                Destroy(gameObject); // Unlock the door and remove it
            }
            else
            {
                // Update the text to inform the player they still need the key
                promptText.text = "You still need the key.";
                audioManager.PlaySFX(audioManager.tooPoor);
            }
        }
    }
}
