using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    Rigidbody rb;

    Vector3 startPos;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable(){
        startPos = transform.position;
        // Time.timeScale = 1f;

    }
    
    void OnDisable(){
        transform.position = startPos;
    }



    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

}
