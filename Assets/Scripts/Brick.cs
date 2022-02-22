using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public bool isThrowing;
    public bool reachedHighSpeed;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isThrowing = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(rb.velocity.magnitude<0.1f && isThrowing && reachedHighSpeed){
            isThrowing = false;
            reachedHighSpeed = false;
            Debug.Log("# After Throwing : velocity down to 0.1");
        }

        if(!reachedHighSpeed && isThrowing && rb.velocity.magnitude > 1f){
            reachedHighSpeed = true;
            Debug.Log("# After Throwing : Reached High Speed");
        }
    }


}
