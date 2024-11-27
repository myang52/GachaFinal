using UnityEngine;
using TMPro;

public class GachaPlatform : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnableObjects; // Array of all objects to spawn
    [SerializeField]
    private Transform spawnPoint; // Location where the object will spawn
    [SerializeField]
    private TextMeshProUGUI promptText; // UI text for interaction prompt
    [SerializeField]
    private int gachaCost = 5; // Cost to use the gacha mechanic
    [SerializeField]
    private float elevationHeight = 2f; // Height above the platform to spawn the object

    AudioManager audioManager;

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
                promptText.text = "Press E to spend 5 coins for a random boost!";
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
            if (playerController.SpendCoins(gachaCost))
            {
                audioManager.PlaySFX(audioManager.summon);
                SpawnRandomObject();
            }
            else
            {
                promptText.text = "Not enough coins!";
                audioManager.PlaySFX(audioManager.tooPoor);
            }
        }
    }

    private void SpawnRandomObject()
    {
        if (spawnableObjects == null || spawnableObjects.Length == 0)
        {
            Debug.LogError("Spawnable objects array is empty or not assigned!");
            return;
        }

        // Randomly select an object from the array
        int randomIndex = Random.Range(0, spawnableObjects.Length);
        GameObject selectedObject = spawnableObjects[randomIndex];

        // Calculate the elevated spawn position
        Vector3 elevatedPosition = spawnPoint.position + Vector3.up * elevationHeight;

        // Instantiate the selected object at the elevated position
        GameObject spawnedObject = Instantiate(selectedObject, elevatedPosition, Quaternion.identity);

        // Ensure the spawned object has a Rigidbody for falling
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
        }

        // Update the prompt text
        promptText.text = "You got a " + selectedObject.name + "!";
    }
}
