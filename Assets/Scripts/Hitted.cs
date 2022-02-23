using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System;

public class Hitted : MonoBehaviour
{
    public float speed=5f;
    public float _forceVal = 10f;
    public float timeOfHitting = 0.25f;


    public Collider[] colliders;
    public Rigidbody[] rbs;
    private Transform[] transforms;
    private Vector3[] localpositions;
    private Vector3[] localrotations;
    private Vector3[] positionsLastRagdoll;
    private Vector3[] rotationsLastRagdoll;
    private float lastVal;


    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();
        colliders = colliders.Where((source, index) => index > 1).ToArray();
        rbs = rbs.Where((source, index) => index != 0).ToArray();
        transforms = new Transform[colliders.Length];

        for(int i = 0; i<colliders.Length; i++){
            transforms[i] = colliders[i].transform;
            colliders[i].gameObject.AddComponent<Compono>();
            colliders[i].gameObject.GetComponent<Compono>().forceVal = _forceVal;
        }

        localpositions = new Vector3[colliders.Length];
        localrotations = new Vector3[colliders.Length];
        positionsLastRagdoll = new Vector3[colliders.Length];
        rotationsLastRagdoll = new Vector3[colliders.Length];

        RagdollOffBasic();

        lastVal = _forceVal;
    }



    public void RagdollOffBasic(){
        Debug.Log("ragdoll_Off_Basic");
        for(int i=0; i<colliders.Length; i++){
            colliders[i].enabled = false;
            rbs[i].isKinematic = true;
            rbs[i].useGravity = false;
        }
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Animator>().enabled = true;
    }


    public void Dead(){
        Debug.Log("dead");
        rbs[0].constraints = RigidbodyConstraints.None;
        RagdollOn();
    }


    void Update()
    {
        if(_forceVal != lastVal){
            lastVal = _forceVal; 
            for(int i = 0; i<colliders.Length; i++){
                colliders[i].gameObject.GetComponent<Compono>().forceVal = _forceVal;
            }
            StartCoroutine(HurtAnimation());
        }

    }



    // void OnCollisionEnter(Collision other){
    //     if(other.gameObject.CompareTag("Items")){
    //         StartCoroutine(HurtAnimation());
    //     }
    // }

    public IEnumerator HurtAnimation(){
        RagdollOn();
        yield return new WaitForSeconds(timeOfHitting);
        RagdollOff();
    }


    public void RagdollOn(){
        for(int i=0; i<colliders.Length; i++){
            colliders[i].enabled = true;
            rbs[i].isKinematic = false;
            rbs[i].useGravity = true;

            localpositions[i] = transforms[i].localPosition;
            localrotations[i] = transforms[i].rotation.eulerAngles;
        }

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Animator>().enabled = false;
    }


    public void RagdollOff(){
        for(int i=0; i<colliders.Length; i++){
            colliders[i].enabled = false;
            rbs[i].isKinematic = true;
            rbs[i].useGravity = false;

            positionsLastRagdoll[i] = transforms[i].localPosition;
            rotationsLastRagdoll[i] = transforms[i].rotation.eulerAngles;
        }
        StartCoroutine(GetBackFromRagdoll());
    }


    IEnumerator GetBackFromRagdoll(){
        float det = 0f;
        while(det<=1)
        {
            det += Time.deltaTime*speed;
            for(int i = 0; i<transforms.Length; i++)
            {
                transforms[i].eulerAngles = new Vector3(
                    Mathf.LerpAngle(rotationsLastRagdoll[i].x, localrotations[i].x, det),
                    Mathf.LerpAngle(rotationsLastRagdoll[i].y, localrotations[i].y, det),
                    Mathf.LerpAngle(rotationsLastRagdoll[i].z, localrotations[i].z, det)
                );
            }
            yield return null;
        }

        for(int i = 0; i<transforms.Length; i++){
            positionsLastRagdoll[i] = transforms[i].localPosition;
        }
        det = 0f;
        while(det<=1)
        {
            det += Time.deltaTime*speed*4;
            for(int i = 0; i<transforms.Length; i++)
            {
                if(i==0){
                    continue;
                }
                transforms[i].localPosition = Vector3.Lerp(positionsLastRagdoll[i], localpositions[i], det);
            }
            yield return null;
        }

        transform.position = new Vector3(transforms[0].position.x-localpositions[0].y , 0f,  transforms[0].position.z-localpositions[0].x);
        transforms[0].localPosition = new Vector3(localpositions[0].x , localpositions[0].y,  localpositions[0].z);

        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Animator>().enabled = true;
        yield return null;
    }

}
