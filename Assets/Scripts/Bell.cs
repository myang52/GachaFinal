using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Bell : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to the UI text displaying the prompt

    private bool playerNearby = false; // Tracks if the player is near the bell
    private PlayerController playerController; // Reference to the player's script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerNearby = true;
                promptText.text = "Press E to ring the Magic Bell!"; // Display prompt
                promptText.gameObject.SetActive(true); // Show the prompt
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
        if (playerNearby && playerController != null && playerController.interactionAction.triggered)
        {
            RingBell();
        }
    }

    private void RingBell()
    {
        promptText.gameObject.SetActive(false); // Hide the prompt
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("WinScreen"); // Load the WinScreen scene
    }
}
