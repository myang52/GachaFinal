using UnityEngine;

public class Jog : MonoBehaviour
{
    private Animator animator; // Animator for the child object
    private PlayerController playerController; // Reference to the parent PlayerController script

    private void Awake()
    {
        // Get the Animator on the child object
        animator = GetComponent<Animator>();

        // Find the PlayerController script on the parent object
        playerController = GetComponentInParent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found on parent object.");
        }
    }

    private void Update()
    {
        if (playerController != null)
        {
            // Check the player's movement state from PlayerController
            bool isMoving = playerController.GetIsMoving();

            // Update the Animator's isMoving parameter
            animator.SetBool("isMoving", isMoving);
        }
    }
}
