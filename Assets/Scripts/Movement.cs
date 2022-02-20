using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;
public class Movement : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float smoothTime;
    [SerializeField] private float angleSmoothTime;

    CinemachineVirtualCamera normalCvc;

    Rigidbody rb;
    Vector3 input;
    Vector3 velocity;
    float directionVelocity;
    float angleVelocity;
    float smoothMagnitude;
    
    Vector3 position;
    Vector3 angle;
    Camera cam;


    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        if(!isLocalPlayer) return;
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        normalCvc = GameObject.FindGameObjectWithTag("Cam0").GetComponent<CinemachineVirtualCamera>();
        normalCvc.Follow = transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 
    }


    void Update()
    {
        if(!isLocalPlayer) return;
        Vector3 input0 = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        input = input0.normalized;
        float directionMagnitude = input.magnitude;
        smoothMagnitude = Mathf.SmoothDamp(smoothMagnitude, directionMagnitude, ref directionVelocity, smoothTime);

        float angleTemp = transform.eulerAngles.y + Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;
        Vector3 dir = new Vector3(Mathf.Sin(angleTemp * Mathf.Deg2Rad), 0, Mathf.Cos(angleTemp * Mathf.Deg2Rad));
        velocity = dir.normalized * speed * directionMagnitude;

        float targetAngle = cam.transform.eulerAngles.y;
        angle = Vector3.up * Mathf.SmoothDampAngle(angle.y, targetAngle, ref angleVelocity, angleSmoothTime);


        anim.SetFloat("VelocityX", Vector3.Dot(input0, Vector3.right), 0.1f, Time.deltaTime);
        anim.SetFloat("VelocityZ", Vector3.Dot(input0, Vector3.forward), 0.1f, Time.deltaTime);
    }

    void FixedUpdate()
    {
        if(!isLocalPlayer) return;
        rb.MovePosition(rb.position + velocity*Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(angle));
    }

}
