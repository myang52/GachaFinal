using UnityEngine;
using TMPro;

public class AttackRangeBoost : MonoBehaviour
{
    [SerializeField] public Vector3 _rotation;
    [SerializeField] public float _speed;

    AudioManager audioManager;
    [SerializeField]
    private int cost = 0; // Cost in coins
    [SerializeField]
    private TextMeshProUGUI promptText; // Reference to UI text displaying the prompt
    [SerializeField] private TextMeshProUGUI dmgText;

    //AudioManager audioManager; //audio

    private bool playerNearby = false;
    private PlayerController playerController;


    private void Awake()
    {

         audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerNearby = true;
                promptText.text = "Attack Range boost! " +" Press E to use.";
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
        transform.Rotate(_rotation * _speed * Time.deltaTime); //rotation

        if (playerNearby && playerController != null && playerController.interactionAction.triggered)
        {
            if (playerController.SpendCoins(cost))
            {
                audioManager.PlaySFX(audioManager.upgrade);
                playerController.hasATKboost = true;
                playerController.atkBoostCount++;
                dmgText.text = "Dmg Multiplier: x" + playerController.atkBoostCount;
               
                promptText.gameObject.SetActive(false); // Hide prompt after purchase
                Destroy(gameObject); // Remove the  item from the scene
            }
            else
            {
                promptText.text = "Not enough coins!";

            }
        }
    }
}
