using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Slider healthBar;
    [SerializeField] private int maxHP = 100;
    [SerializeField] private float healthDamage = 25f; // Damage to player health
    [SerializeField] private float aoeDamage = 10f;    // AOE damage to enemy
    [SerializeField] private float aoeInterval = 1f;   // Interval for AOE damage
    [SerializeField] private float aoeRange = 5f;      // Range within which AOE damage is applied
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float stoppingDistanceOffset = 1f;
    [SerializeField] private float lungeDistance = 1.5f;
    [SerializeField] private float lungeSpeed = 8f;
    [SerializeField] private float lungeCooldown = 1.5f;

    AudioManager audioManager;

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private EnemySpawner enemySpawner;
    private Vector3 stoppingPosition;
    private bool isLunging = false;
    private bool isInCooldown = false;
    private int HP;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<Slider>();
        }

        HP = maxHP; // Initialize HP to maximum health

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing from the enemy object.");
            return;
        }

        navMeshAgent.speed = chaseSpeed;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player GameObject not found. Ensure it is tagged as 'Player'.");
        }

        enemySpawner = FindFirstObjectByType<EnemySpawner>();

        // Start the auto-attack coroutine
        StartCoroutine(AutoAttackAOE());
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)HP / maxHP;
        }
    }

    private void Update()
    {
        healthBar.value = HP;

        if (playerTransform == null || navMeshAgent == null || !navMeshAgent.isOnNavMesh || isLunging)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            ChaseAndPrepareLunge();
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }

    private void ChaseAndPrepareLunge()
    {
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            stoppingPosition = playerTransform.position - directionToPlayer * stoppingDistanceOffset;

            navMeshAgent.SetDestination(stoppingPosition);

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!isInCooldown)
                {
                    StartCoroutine(LungeTowardsPlayer());
                }
            }
        }
    }

    private IEnumerator LungeTowardsPlayer()
    {
        isLunging = true;
        isInCooldown = true;

        // Fully disable NavMeshAgent to prevent interference
        navMeshAgent.enabled = false;

        // Calculate the lunge target in front of the enemy
        Vector3 lungeTarget = transform.position + (playerTransform.position - transform.position).normalized * lungeDistance;

        // Move forward toward the player with a smooth lunge
        while (Vector3.Distance(transform.position, lungeTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, lungeTarget, lungeSpeed * Time.deltaTime);
            yield return null;
        }

        // Brief pause at the end of the lunge
        yield return new WaitForSeconds(0.2f);

        // Return to the stopping position smoothly
        while (Vector3.Distance(transform.position, stoppingPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, stoppingPosition, lungeSpeed * Time.deltaTime);
            yield return null;
        }

        // Reactivate the NavMeshAgent and reset lunge state
        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        isLunging = false;

        // Wait for cooldown before allowing the next lunge
        yield return new WaitForSeconds(lungeCooldown);
        isInCooldown = false;
    }
    private IEnumerator AutoAttackAOE()
    {
        // Inflict AOE damage only if within range of the player
        while (HP > 0)
        {
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                // Check if enemy is within AOE range of the player
                if (distanceToPlayer <= aoeRange)
                {
                    float damage = aoeDamage;

                    // Check if the player has an attack boost
                    if (playerTransform.GetComponent<PlayerController>()?.hasATKboost == true && playerTransform.GetComponent<PlayerController>()?.atkBoostCount > 0)
                    {
                        damage *= Mathf.Pow(2, playerTransform.GetComponent<PlayerController>().atkBoostCount); // Stack boosts exponentially
                    }

                    HP -= (int)damage;
                    UpdateHealthBar();

                    if (HP <= 0)
                    {
                        DestroyAndSpawnCoin(); // Trigger coin spawn and destroy enemy
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(aoeInterval); // Wait for the specified interval
        }
    }

    public void TakeDamage(float damage, bool isBoosted)
    {
        if (isBoosted)
        {
            damage *= 2; // Double the damage if attack boost is active
        }

        HP -= (int)damage;
        UpdateHealthBar();

        if (HP <= 0)
        {
            DestroyAndSpawnCoin();
        }




    }

    public void DestroyAndSpawnCoin()
    {
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.Euler(0, 0, 90));
        }

        if (enemySpawner != null)
        {
            audioManager.PlaySFX(audioManager.orcDeath); //jump sound effect.
            enemySpawner.EnemyDestroyed();
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                float damage = healthDamage;

                // Check if the player has an attack boost
                if (playerController.hasATKboost)
                {
                    damage *= 2; // Double the damage
                }

                playerController.RemovePlayerHealth(damage);
                Debug.Log("Player took damage from enemy!");
            }
        }
    }
}
