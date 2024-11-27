using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float playerSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] public float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 10f; // Smooth rotation speed
    [SerializeField] public bool hasATKboost = false; //win condition

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] public bool hasKey = false; //win condition


    [SerializeField] public int atkBoostCount = 0; // Tracks the number of active attack boosts

    AudioManager audioManager;


    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction aimAction;
    public InputAction interactionAction;

    public float playerHealth = 100f;
    public int coinCount = 0;

    private bool canDoubleJump = false; // Double jump ability flag
    private bool hasDoubleJumped = false; // Tracks if double jump was used


   
   

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        cameraTransform = Camera.main != null ? Camera.main.transform : null;

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        aimAction = playerInput.actions["Aim"];
        interactionAction = playerInput.actions["Interact"];

        Cursor.lockState = CursorLockMode.Locked;
        //anim
       // animator = GetComponent<Animator>();
    
    }

    private void OnEnable()
    {
        aimAction.Enable();
        interactionAction.Enable();
    }

    private void OnDisable()
    {
        aimAction.Disable();
        interactionAction.Disable();
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            playerVelocity.y = 0f;
            hasDoubleJumped = false; // Reset double jump on ground
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;

        // Move the player
        controller.Move(move * Time.deltaTime * playerSpeed);
     
       
  

        // Rotate the player to face the direction of movement
        if (move.magnitude > 0.1f) // Only rotate if there's input
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Handle jumping
        if (jumpAction.triggered)
        {
            
            if (groundedPlayer)
            {
                Jump();
         


            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                Jump();
                hasDoubleJumped = true; // Prevent further jumps until grounded
            }
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        audioManager.PlaySFX(audioManager.jump); //jump sound effect.
    }

    public void EnableDoubleJump()
    {
        canDoubleJump = true;
    }

    public void RemovePlayerHealth(float dmg)
    {
        playerHealth = Mathf.Clamp(playerHealth - dmg, 0, 100f);
        healthSlider.value = playerHealth / 100f;
        audioManager.PlaySFX(audioManager.damage);

        if (playerHealth <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("GameOver");
        }
    }

    public bool SpendCoins(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            coinText.text = "Coins: " + coinCount;
            return true;
        }
        return false;
    }


    public void RestorePlayerHealth(float hp)
    {
        playerHealth = Mathf.Clamp(playerHealth + hp, 0, 100f);
        healthSlider.value = playerHealth / 100f;
        audioManager.PlaySFX(audioManager.regainhealth);
    }


    public bool GetIsMoving()
    {
        return moveAction.ReadValue<Vector2>().magnitude > 0.1f;
    }

    public bool GetJumpActionTriggered()
    {
        return jumpAction.triggered;
    }




    public void AddCoin()
    {
        coinCount++;
        coinText.text = "Coins: " + coinCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy1"))
        {
            RemovePlayerHealth(25f);
        }

        if (other.CompareTag("Coin"))
        {
            AddCoin();
            Destroy(other.gameObject);
            audioManager.PlaySFX(audioManager.coin);
        }
    }
}
