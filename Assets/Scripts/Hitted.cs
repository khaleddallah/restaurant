using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitted : MonoBehaviour
{
    private Collider[] colliders;
    private Rigidbody[] rbs;

    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();
        RagdollOff();
        // AddOnCollision();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NFRagdoll(bool ragdoll){
        if(ragdoll){
            Debug.Log("ragdoll");

            foreach(Collider c in colliders){
                c.enabled = true;
            }
            foreach(Rigidbody r in rbs){
                r.isKinematic = false;
                r.useGravity = true;
            }
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Animator>().enabled = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
        else{
            Debug.Log("NOT ragdoll");
            foreach(Collider c in colliders){
                c.enabled = false;
            }
            foreach(Rigidbody r in rbs){
                r.isKinematic = true;
                r.useGravity = false;
            }
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<CapsuleCollider>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;
        }
    }

    // public void AddOnCollision(){
    //     foreach(Collider c in colliders){
    //         c.enabled = true;
    //         c.enter +=
    //     }
    // }

    public void RagdollOn(){
        Debug.Log("ragdoll_On");
        foreach(Collider c in colliders){
            c.enabled = true;
        }
        foreach(Rigidbody r in rbs){
            r.isKinematic = false;
            r.useGravity = true;
        }

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Animator>().enabled = false;
    }


    public void RagdollOff(){
        Debug.Log("ragdoll_Off");
        foreach(Collider c in colliders){
            c.enabled = false;
        }
        foreach(Rigidbody r in rbs){
            r.isKinematic = true;
            r.useGravity = false;
        }
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Animator>().enabled = true;
    }


    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("bullet")){
            Debug.Log("ENTER");
            Destroy(other.gameObject);
            RagdollOn();
            StartCoroutine(Wait());
        }
    }



    IEnumerator Wait(){
        yield return new WaitForSeconds(0.5f);
        RagdollOff();
    }



    // void NewOnCollisionEnter(Collision other){
    //     if(other.gameObject.CompareTag("bullet")){
    //         Debug.Log("ENTER");
    //         RagdollOn();
    //     }
    // }



    // void OnCollisionExit(Collision other){
    //     if(other.gameObject.CompareTag("bullet")){
    //         Debug.Log("EXIT");
    //         RagdollOff();
    //     }
    // }




}
