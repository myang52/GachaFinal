using UnityEngine;
using TMPro;
using System.Collections;

public class FreezeTrap : MonoBehaviour
{
    [SerializeField]
    private float freezeDuration = 5f; // Duration to freeze the player in seconds
    [SerializeField]
    private TextMeshProUGUI trapMessageText; // Reference to the UI text for trap message

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                StartCoroutine(FreezePlayer(playerController));
            }
        }
    }

    private IEnumerator FreezePlayer(PlayerController playerController)
    {
        playerController.enabled = false; // Disable PlayerController to freeze the player

        // Display the trap message
        if (trapMessageText != null)
        {
            trapMessageText.text = "You fell for a freeze trap! Wait 5 seconds!";
            trapMessageText.gameObject.SetActive(true); // Show the message
        }

        yield return new WaitForSeconds(freezeDuration); // Wait for the freeze duration

        // Re-enable player movement and hide the message
        playerController.enabled = true;
        if (trapMessageText != null)
        {
            trapMessageText.gameObject.SetActive(false); // Hide the message
        }

        Destroy(gameObject); // Destroy the trap after it activates
    }
}
