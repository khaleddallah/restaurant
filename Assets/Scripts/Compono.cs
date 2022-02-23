using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compono : MonoBehaviour
{
    public float forceVal = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("bullet")){
            Debug.Log("my name is "+gameObject.name);
            Debug.DrawRay(collision.GetContact(0).point, collision.relativeVelocity, Color.green, 100f);
            GetComponent<Rigidbody>().AddForce(collision.relativeVelocity * forceVal, ForceMode.Impulse);
            collision.gameObject.SetActive(false);
        }
    }
}
