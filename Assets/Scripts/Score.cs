using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Score : NetworkBehaviour
{
    public float money; 
    public float score;
    public float health;
    public GameObject items;

    public float hurtValue=34;

    private Collider[] colliders;
    private Rigidbody[] rbs;

    // Start is called before the first frame update
    void Start()
    {
        items = GameObject.FindGameObjectWithTag("Items");

        money = 0;
        score = 0;
        health = 100;
        colliders = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();

        NFRagdoll(false);
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


    void OnCollisionEnter(Collision other){
        if(!isLocalPlayer) return;
        if(other.gameObject.tag == "Item")
        {
            if(other.gameObject.GetComponent<Brick>().isThrowing){
                int index0 = other.gameObject.transform.GetSiblingIndex();
                Hurt(index0);
                CmdHurt(index0);
            }
        }
    }

    [Command]
    void CmdHurt(int index0){
        RpcHurt(index0);
    }

    [ClientRpc]
    void RpcHurt(int index0){
        if(isLocalPlayer) return;
        Hurt(index0);
    }


    void Hurt(int index0){
        GameObject tmp = items.transform.GetChild(index0).gameObject;
        Debug.Log("#-#");
        health -= hurtValue;
        Destroy(tmp);
        if(health<0){
            NFRagdoll(true);
            // Destroy(gameObject);
        }
    }


}
