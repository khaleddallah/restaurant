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

        GetComponent<Hitted>().RagdollOffBasic();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // Destroy(tmp);
        if(health>0){
            StartCoroutine(GetComponent<Hitted>().HurtAnimation());
        }
        else{
            GetComponent<Hitted>().Dead();
        }


    }


}
