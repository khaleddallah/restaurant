using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System;

public class Hitted : MonoBehaviour
{
    private Collider[] colliders;
    private Rigidbody[] rbs;

    public float speed=5f;
    public float _forceVal = 10f;

    public Transform[] transforms;
    public Vector3[] positions;
    public Vector3[] rotations;

    public Vector3[] positionsLastRagdoll;
    public Vector3[] rotationsLastRagdoll;

    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();
        
        colliders = colliders.Where((source, index) => index != 0).ToArray();


        transforms = new Transform[colliders.Length];
        for(int i = 0; i<colliders.Length; i++){
            transforms[i] = colliders[i].transform;
            colliders[i].gameObject.AddComponent<Compono>();
            colliders[i].gameObject.GetComponent<Compono>().forceVal = _forceVal;
        }

        positions = new Vector3[colliders.Length];
        rotations = new Vector3[colliders.Length];
        positionsLastRagdoll = new Vector3[colliders.Length];
        rotationsLastRagdoll = new Vector3[colliders.Length];

        RagdollOffBasic();
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
        
        for(int i = 0; i<colliders.Length; i++){
            positions[i] = transforms[i].position;
            rotations[i] = transforms[i].rotation.eulerAngles;
        }


        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Animator>().enabled = false;

        // Time.timeScale = 0.2f;

    }


    public void RagdollOffBasic(){
        Debug.Log("ragdoll_Off_Basic");
        foreach(Collider c in colliders){
            c.enabled = false;
        }
        foreach(Rigidbody r in rbs){
            r.isKinematic = true;
            r.useGravity = false;
        }
    

        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Animator>().enabled = true;
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

        for(int i = 0; i<transforms.Length; i++){
            positionsLastRagdoll[i] = transforms[i].position;
            rotationsLastRagdoll[i] = transforms[i].rotation.eulerAngles;
        }
        StartCoroutine(GetBackFromRagdoll());
        // GetComponent<Collider>().enabled = true;
        // GetComponent<Rigidbody>().isKinematic = false;
        // GetComponent<Animator>().enabled = true;
    }





    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("bullet")){
            Debug.Log("ENTER");
            // other.gameObject.SetActive(false);
            RagdollOn();
            StartCoroutine(Wait());
        }
    }



    IEnumerator Wait(){
        yield return new WaitForSeconds(0.5f);
        RagdollOff();
    }


    IEnumerator GetBackFromRagdoll(){
        
        float det = 0f;
        while(det<=1)
        {
            det += Time.deltaTime*speed;
            for(int i = 0; i<transforms.Length; i++)
            {
                transforms[i].position = Vector3.Lerp(positionsLastRagdoll[i], positions[i], det);
                // transforms[i].rotation = Quaternion.Euler(Vector3.Lerp(rotationsLastRagdoll[i], rotations[i], det));
                // transforms[i].eulerAngles = Vector3.Lerp(rotationsLastRagdoll[i], rotations[i], det);
                transforms[i].eulerAngles = new Vector3(
                    Mathf.LerpAngle(rotationsLastRagdoll[i].x, rotations[i].x, det),
                    Mathf.LerpAngle(rotationsLastRagdoll[i].y, rotations[i].y, det),
                    Mathf.LerpAngle(rotationsLastRagdoll[i].z, rotations[i].z, det)
                );


            }
            yield return null;
        }

        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        // yield return new WaitForSeconds(2f);

        GetComponent<Animator>().enabled = true;
        yield return null;
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
