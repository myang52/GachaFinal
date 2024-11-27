using UnityEngine;

public class JumpAnim : MonoBehaviour
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
        if (playerController != null && animator != null)
        {
            // Check if the jump action was triggered
            if (playerController.GetJumpActionTriggered())
            {
                // Set the isJumping trigger in the Animator
                animator.SetTrigger("isJumping");
            }
        }
    }
}
