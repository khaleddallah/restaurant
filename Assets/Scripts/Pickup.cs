using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject presentItem;
    public GameObject pickudItem;

    [SerializeField] private GameObject highlightSign;
    [SerializeField] private float throwSpeed;
    private float highlightSignHight;
    private Transform arm;

    Vector3 armRotation;

    Camera cam;
    bool mouseDown;

    // Start is called before the first frame update
    void Start()
    {
        mouseDown = false;
        highlightSignHight = highlightSign.transform.position.y;
        arm = transform.GetChild(0);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 

        cam = Camera.main;
        armRotation = arm.eulerAngles;
    }


    void Update(){
        mouseDown = Input.GetMouseButtonDown(0);
        ArmMovement();
        PickItem();
        ThrowItem();
    }


    void ArmMovement(){
        arm.eulerAngles = new Vector3(
            armRotation.x,
            armRotation.y,
            armRotation.z + cam.transform.eulerAngles.x);
    }


    void PickItem(){
        if( presentItem && mouseDown ){
            mouseDown = false;
            pickudItem = presentItem;
            presentItem = null;
            highlightSign.SetActive(false);
            pickudItem.GetComponent<Rigidbody>().isKinematic = true;
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
            if(mouseDown){
                mouseDown = false;
                pickudItem.GetComponent<Rigidbody>().isKinematic = false;
                pickudItem.GetComponent<Rigidbody>().detectCollisions = true;
                pickudItem.GetComponent<Rigidbody>().AddForce(cam.transform.forward*throwSpeed, ForceMode.Impulse);
                pickudItem = null;
            }
        }
    }

    void OnTriggerEnter(Collider other){
        if(pickudItem) return;
        if(other.transform.CompareTag("Item")){
            highlightSign.SetActive(true);
            highlightSign.transform.position = GetHighlightSignPosition(other.transform.position)+Vector3.up*0.01f;
            presentItem = other.gameObject;
        }
    }

    Vector3 GetHighlightSignPosition(Vector3 itemPosition){
        Ray ray = new Ray(itemPosition, Vector3.down);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo)){
            Debug.Log("GetPosItem:"+hitInfo.point);
            return hitInfo.point;
        }
        else{
            Debug.Log("error");
            return itemPosition;
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
