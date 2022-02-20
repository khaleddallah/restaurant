using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pickup : NetworkBehaviour
{
    public GameObject presentItem;
    public GameObject pickudItem;

    [SerializeField] private GameObject highlightSign;
    [SerializeField] private float throwSpeed;
    private float highlightSignHight;
    public Transform arm;

    Vector3 armRotation;

    Camera cam;
    public bool mouseDown;

    public GameObject items;

    // Start is called before the first frame update
    void Start()
    {
        items = GameObject.FindGameObjectWithTag("Items");
        mouseDown = false;
        // arm = transform.GetChild(0);
        cam = Camera.main;
        armRotation = arm.eulerAngles;
        if(isLocalPlayer){
            highlightSign = GameObject.FindGameObjectWithTag("HighlightSign");
            highlightSignHight = highlightSign.transform.position.y;
        }

    }


    void Update(){
        if(isLocalPlayer){
            mouseDown = Input.GetMouseButtonDown(0);
        }
        PickItem();
        ThrowItem();
    }



    void PickItem(){
        if( presentItem && mouseDown ){
            mouseDown = false;
            pickudItem = presentItem;
            presentItem = null;
            pickudItem.GetComponent<Rigidbody>().isKinematic = true;
            pickudItem.GetComponent<Rigidbody>().detectCollisions = false;
            highlightSign.SetActive(false);
            int index0 = pickudItem.transform.GetSiblingIndex();
            PickItemOnServer(index0);
        }
        if(pickudItem){
            pickudItem.transform.position = new Vector3(
                arm.position.x,
                arm.position.y,//+arm.localScale.y+pickudItem.transform.localScale.y,
                arm.position.z);
            pickudItem.transform.rotation = transform.rotation;
        }
    }


    [Command]
    void PickItemOnServer(int index0){
        PickItemOnClient(index0);
    }

    [ClientRpc]
    void PickItemOnClient(int index0){
        if(isLocalPlayer) return;
        pickudItem = items.transform.GetChild(index0).gameObject;
        pickudItem.GetComponent<Rigidbody>().isKinematic = true;
        pickudItem.GetComponent<Rigidbody>().detectCollisions = false;
    }


    void ThrowItem(){
        if(isLocalPlayer){
            if(pickudItem){
                if(mouseDown){
                    mouseDown = false;
                    pickudItem.GetComponent<Rigidbody>().isKinematic = false;
                    pickudItem.GetComponent<Rigidbody>().detectCollisions = true;
                    pickudItem.GetComponent<Rigidbody>().AddForce(cam.transform.forward*throwSpeed, ForceMode.Impulse);
                    ThrowOnServer(pickudItem.transform.position, cam.transform.forward*throwSpeed);
                    pickudItem = null;
                }
            }

        }
    }



    [Command]
    void ThrowOnServer(Vector3 origin, Vector3 direction){
        ThrowOnClient(origin, direction);
    }


    [ClientRpc]
    void ThrowOnClient(Vector3 origin, Vector3 direction){
        if(isLocalPlayer) return;
        pickudItem.transform.position = origin;
        pickudItem.GetComponent<Rigidbody>().isKinematic = false;
        pickudItem.GetComponent<Rigidbody>().detectCollisions = true;
        pickudItem.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
        pickudItem = null;
    }



    void OnTriggerEnter(Collider other){
        if(!isLocalPlayer) return;
        if(pickudItem) return;
        if(other.transform.CompareTag("Item")){
            highlightSign.SetActive(true);
            highlightSign.transform.position = GetHighlightSignPosition(other.transform.position)+Vector3.up*0.01f;
            presentItem = other.gameObject;
        }
    }



    void OnTriggerExit(Collider other){
        if(!isLocalPlayer) return;
        if(pickudItem) return;
        if(other.transform.CompareTag("Item")){
            if(other.gameObject == presentItem){
                presentItem = null;
                highlightSign.SetActive(false);
            }
        }
    }


    Vector3 GetHighlightSignPosition(Vector3 itemPosition){
        Ray ray = new Ray(itemPosition, Vector3.down);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo)){
            return hitInfo.point;
        }
        else{
            Debug.Log("error");
            return itemPosition;
        }
    }




}
