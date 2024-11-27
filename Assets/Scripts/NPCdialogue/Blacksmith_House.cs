using UnityEngine;
using TMPro;

public class Blacksmith_House : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to the UI text displaying the prompt
    [SerializeField]
    [TextArea]
    private string npcDialogue = "Welcome, traveler! Do you seek the finest blades in the land?"; // Customizable NPC dialogue

    private bool playerNearby = false; // Tracks if the player is near the Blacksmith House

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            DisplayDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            HideDialogue();
        }
    }

    private void DisplayDialogue()
    {
        if (promptText != null)
        {
            promptText.text = npcDialogue; // Set the NPC dialogue
            promptText.gameObject.SetActive(true); // Show the prompt text
        }
    }

    private void HideDialogue()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false); // Hide the prompt text
        }
    }
}
