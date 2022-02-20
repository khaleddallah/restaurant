using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Animations.Rigging;


public class Pickup : NetworkBehaviour
{


    public GameObject presentItem;
    public GameObject pickudItem;

    [SerializeField] private GameObject highlightSign;
    [SerializeField] private float throwSpeed;
    private float highlightSignHight;
    public Transform arm;

    // Vector3 armRotation;

    Camera cam;
    public bool mouseDown;

    public GameObject items;

    public Rig rigStarter;
    public Rig rigFinal;
    public float rigStarterWeight;
    public float rigFinalWeight;
    public float throwSpeedAnimation;

    bool throwing;

    Vector3 originRemote, directionRemote;


    // Start is called before the first frame update
    void Start()
    {
        rigStarterWeight = 0;
        rigFinalWeight = 0;
        items = GameObject.FindGameObjectWithTag("Items");
        mouseDown = false;
        // arm = transform.GetChild(0);
        cam = Camera.main;
        // armRotation = arm.eulerAngles;
        if(isLocalPlayer){
            highlightSign = GameObject.FindGameObjectWithTag("HighlightSign");
            highlightSignHight = highlightSign.transform.position.y;
        }
        throwing = false;
    }


    void Update(){
        if(isLocalPlayer){
            mouseDown = Input.GetMouseButtonDown(0);
        }
        PickItem();
        ThrowItem();

        rigStarter.weight = Mathf.Lerp(rigStarter.weight, rigStarterWeight, Time.deltaTime * throwSpeedAnimation);
        rigFinal.weight = Mathf.Lerp(rigFinal.weight, rigFinalWeight, Time.deltaTime * throwSpeedAnimation);
        
        if(rigFinalWeight == 1 && rigFinal.weight > 0.9f ){
            rigFinal.weight = 1;
            if(throwing){
                GetComponent<Movement>().isEnabled = true;
                throwing=false;
                pickudItem.transform.position = originRemote;
                pickudItem.GetComponent<Rigidbody>().isKinematic = false;
                pickudItem.GetComponent<Rigidbody>().detectCollisions = true;
                pickudItem.GetComponent<Rigidbody>().AddForce(directionRemote, ForceMode.Impulse);
                pickudItem = null;
            }
        }
        if(rigFinalWeight == 0 && rigFinal.weight < 0.1f ){
            rigFinal.weight = 0;
        }

        if(rigStarterWeight == 1 && rigStarter.weight > 0.9f ){
            rigStarter.weight = 1;
        }
        if(rigStarterWeight == 0 && rigStarter.weight < 0.1f ){
            rigStarter.weight = 0;
        }


        if(rigFinalWeight==1){
            rigFinalWeight = 0;
        }
    }



    void PickItem(){
        if( presentItem && mouseDown ){
            mouseDown = false;
            pickudItem = presentItem;
            rigStarterWeight = 1;
            rigFinalWeight = 0;
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
        rigStarterWeight = 1;
        rigFinalWeight = 0;
        pickudItem.GetComponent<Rigidbody>().isKinematic = true;
        pickudItem.GetComponent<Rigidbody>().detectCollisions = false;
    }


    void ThrowItem(){
        if(isLocalPlayer){
            if(pickudItem){
                if(mouseDown){
                    mouseDown = false;
                    // Schedual AddForce
                    throwing = true;
                    GetComponent<Movement>().isEnabled = false;
                    // ThrowOnServer(pickudItem.transform.position, cam.transform.forward*throwSpeed);
                    originRemote = rigFinal.transform.GetChild(0).GetChild(0).position;
                    directionRemote = cam.transform.forward*throwSpeed;
                    ThrowOnServer(originRemote, directionRemote);
                    rigStarterWeight = 0;
                    rigFinalWeight = 1;
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
        throwing = true;
        originRemote = origin;
        directionRemote = direction;
        GetComponent<Movement>().isEnabled = false;
        rigStarterWeight = 0;
        rigFinalWeight = 1;
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
