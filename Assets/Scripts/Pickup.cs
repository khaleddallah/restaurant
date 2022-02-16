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
    private Transform arm;

    Vector3 armRotation;

    Camera cam;
    public bool mouseDown;

    // Start is called before the first frame update
    void Start()
    {
        mouseDown = false;
        arm = transform.GetChild(0);
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
            if(mouseDown){
                SetMouseDownOnServer(mouseDown);
            }
        }
        // PickItem();
        // ThrowItem();
    }


    [Command]
    void SetMouseDownOnServer(bool x){
        Debug.Log("# netId"+netId+" :"+x);
        mouseDown = x;
        SetMouseDownOnClient(x);
    }

    [ClientRpc]
    void SetMouseDownOnClient(bool x){
        Debug.Log("netId"+netId+" :"+x);
        if(!isLocalPlayer){
            mouseDown = x;
        }
    }


    void PickItem(){
        if(isLocalPlayer){
            if( presentItem && mouseDown ){
                mouseDown = false;
                pickudItem = presentItem;
                presentItem = null;
            
                SetPickedItemOnServer(pickudItem);
                SetPresentItemOnServer(presentItem);
                highlightSign.SetActive(false);
                
                pickudItem.GetComponent<Rigidbody>().isKinematic = true;
                pickudItem.GetComponent<Rigidbody>().detectCollisions = false;

            }
        }
        if(pickudItem){
            pickudItem.transform.position = new Vector3(
                arm.position.x,
                arm.position.y+arm.localScale.y+pickudItem.transform.localScale.y,
                arm.position.z);
            pickudItem.transform.rotation = transform.rotation;
        }
    }


    [Command]
    void SetPickedItemOnServer(GameObject obj){
        presentItem = obj;
        SetPickedItemOnClient(obj);
    }


    [ClientRpc]
    void SetPickedItemOnClient(GameObject obj){
        presentItem = gameObject;
    }


    void ThrowItem(){
        if(isLocalPlayer){
            if(pickudItem){
                if(mouseDown){
                    mouseDown = false;
                    pickudItem.GetComponent<Rigidbody>().isKinematic = false;
                    pickudItem.GetComponent<Rigidbody>().detectCollisions = true;
                    pickudItem.GetComponent<Rigidbody>().AddForce(cam.transform.forward*throwSpeed, ForceMode.Impulse);
                    pickudItem = null;
                    ThrowOnServer(cam.transform.forward*throwSpeed);
                }
            }

        }
    }


    [Command]
    void ThrowOnServer(Vector3 direction){
        mouseDown = false;
        pickudItem.GetComponent<Rigidbody>().isKinematic = false;
        pickudItem.GetComponent<Rigidbody>().detectCollisions = true;
        pickudItem.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
        pickudItem = null;
        ThrowOnClient(direction);
    }


    [ClientRpc]
    void ThrowOnClient(Vector3 direction){
        mouseDown = false;
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
            SetPresentItemOnServer(presentItem);
        }
    }


    [Command]
    void SetPresentItemOnServer(GameObject obj){
        presentItem = obj;
        SetPresentItemOnClient(obj);
    }


    [ClientRpc]
    void SetPresentItemOnClient(GameObject obj){
        presentItem = gameObject;
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

    void OnTriggerExit(Collider other){
        if(!isLocalPlayer) return;
        if(pickudItem) return;
        if(other.transform.CompareTag("Item")){
            if(other.gameObject == presentItem){
                presentItem = null;
                SetPresentItemOnServer(presentItem);
                highlightSign.SetActive(false);
            }
        }
    }


}
