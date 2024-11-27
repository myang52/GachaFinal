using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthRestore : MonoBehaviour
{

    [SerializeField] public Vector3 _rotation;
    [SerializeField] public float _speed;


    [SerializeField]
    private float playerHealthRestore = 100f;



    private void Update()
    {
        transform.Rotate(_rotation * _speed * Time.deltaTime); //rotation
    }
    public void ConsumeHealth()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.RestorePlayerHealth(playerHealthRestore);
            Destroy(gameObject); // Destroy the battery after use
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ConsumeHealth();
        }
    }
}
