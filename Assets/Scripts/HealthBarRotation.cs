using UnityEngine;
using System.Collections.Generic;
using System.Collections.Generic;
public class HealthBarRotation : MonoBehaviour
{
    public Transform cam;


    void LateUpdate(){

        Vector3 direction = transform.position - cam.position;
        transform.rotation = Quaternion.LookRotation(direction);

    }



}
