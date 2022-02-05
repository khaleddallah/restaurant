using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] GameObject highlightSign;
    private float highlightSignHight;
    private Transform arm;

    public GameObject presentItem;
    public GameObject pickudItem;
    // Start is called before the first frame update
    void Start()
    {
        highlightSignHight = highlightSign.transform.position.y;
        arm = transform.GetChild(0);
    }


    void Update(){
        PickItem();
        ThrowItem();
    }


    void PickItem(){
        if( presentItem && Input.GetKeyDown("f") ){
            pickudItem = presentItem;
            presentItem = null;
            highlightSign.SetActive(false);
            pickudItem.GetComponent<Rigidbody>().isKinematic = false;
            pickudItem.GetComponent<Rigidbody>().detectCollisions = false;

        }
        if(pickudItem){
            pickudItem.transform.position = new Vector3(
                arm.position.x,
                arm.position.y+arm.localScale.y+pickudItem.transform.localScale.y,
                arm.position.z);
            pickudItem.transform.rotation = transform.rotation;
        }
    }


    void ThrowItem(){
        if(pickudItem){
            return;
        }
    }

    void OnTriggerEnter(Collider other){
        if(pickudItem) return;
        if(other.transform.CompareTag("Item")){
            highlightSign.SetActive(true);
            highlightSign.transform.position = new Vector3 (
                other.transform.position.x,
                highlightSignHight,
                other.transform.position.z
            );
            presentItem = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other){
        if(pickudItem) return;
        if(other.transform.CompareTag("Item")){
            if(other.gameObject == presentItem){
                presentItem = null;
                highlightSign.SetActive(false);
            }
        }
    }


}
