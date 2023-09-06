using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Cac_Enemy : MonoBehaviour
{
    
    [SerializeField] Transform _target;
    [SerializeField] float speed = 1f;
    [SerializeField] float minDistance = 1f;
    
    void Start()
    {
        //target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(target != null)
        {
            Debug.DrawLine(transform.position, transform.TransformDirection(Vector3.forward), Color.red);
        }*/


        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
    }
}
