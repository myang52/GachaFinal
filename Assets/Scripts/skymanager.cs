using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skymanager : MonoBehaviour
{
    public float skySpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skySpeed);
    }
}
