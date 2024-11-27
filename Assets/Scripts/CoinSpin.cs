
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class CoinSpin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public Vector3 _rotation;
    [SerializeField] public float _speed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotation * _speed * Time.deltaTime); //rotation
    }
}
