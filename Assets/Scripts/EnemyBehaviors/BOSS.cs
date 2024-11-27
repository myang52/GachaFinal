using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public class BOSS : MonoBehaviour
{
    [SerializeField] public Slider healthBar;
    [SerializeField] private int maxHP = 100;
    [SerializeField] private float healthDamage = 25f; // Damage to player health
    [SerializeField] private float aoeDamage = 10f;    // AOE damage to enemy
    [SerializeField] private float aoeInterval = 1f;   // Interval for AOE damage
    [SerializeField] private float aoeRange = 5f;      // Range within which AOE damage is applied
    [SerializeField] private float detectionRange = 50f; // Increased detection range
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private GameObject coinPrefab;



    AudioManager audioManager;
    // Shooting mechanics
    [SerializeField] private GameObject dmgOrbPrefab; // Prefab for the damage orb
    [SerializeField] private float shootingInterval = 2f; // Time between orb shots
    [SerializeField] private float orbSpeed = 10f; // Speed of the orb

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
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
            Debug.LogError("NavMeshAgent component is missing from the boss object.");
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

        // Start the auto-attack coroutine
        StartCoroutine(AutoAttackAOE());

        // Start the orb shooting coroutine
        StartCoroutine(ShootOrbAtPlayer());
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

        if (playerTransform == null || navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }

    private void ChasePlayer()
    {
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(playerTransform.position);
        }
    }

    private IEnumerator AutoAttackAOE()
    {
        while (HP > 0)
        {
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                if (distanceToPlayer <= aoeRange)
                {
                    float damage = aoeDamage;

                    if (playerTransform.GetComponent<PlayerController>()?.hasATKboost == true && playerTransform.GetComponent<PlayerController>()?.atkBoostCount > 0)
                    {
                        damage *= Mathf.Pow(2, playerTransform.GetComponent<PlayerController>().atkBoostCount); // Stack boosts exponentially
                    }

                    HP -= (int)damage;
                    UpdateHealthBar();

                    if (HP <= 0)
                    {
                        DestroyAndSpawnCoin();
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(aoeInterval);
        }
    }

    private IEnumerator ShootOrbAtPlayer()
    {
        while (HP > 0)
        {
            if (playerTransform != null && dmgOrbPrefab != null)
            {
                audioManager.PlaySFX(audioManager.dmgOrbshoot);
                // Instantiate the damage orb
                GameObject orb = Instantiate(dmgOrbPrefab, transform.position + Vector3.up, Quaternion.identity);

                // Calculate direction toward the player
                Vector3 direction = (playerTransform.position - transform.position).normalized;

                // Apply velocity to the orb
                Rigidbody orbRb = orb.GetComponent<Rigidbody>();
                if (orbRb != null)
                {
                    orbRb.linearVelocity = direction * orbSpeed;
                }

                // Destroy the orb instance after 5 seconds
                Destroy(orb, 5f);
            }

            yield return new WaitForSeconds(shootingInterval);
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
            audioManager.PlaySFX(audioManager.Bossdeath);
        }
    }

    public void DestroyAndSpawnCoin()
    {
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        PlayerController playerController = playerTransform?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.hasKey = true; // Set the player's hasKey property to true
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

                if (playerController.hasATKboost)
                {
                    damage *= 2;
                }

                playerController.RemovePlayerHealth(damage);
                Debug.Log("Player took damage from the boss!");
            }
        }
    }
}
