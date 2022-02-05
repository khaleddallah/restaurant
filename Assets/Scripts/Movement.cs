using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    [SerializeField] private Transform table;
    [SerializeField] private float tableOffset; 
    [SerializeField] private float speed;

    float radius;
    float position_angle;
    Vector3 position;
    Rigidbody rb;
    float input;
    Camera cam;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        radius = table.localScale.x/2 + tableOffset;

        Vector3 directionOfTable = transform.position - table.position;
        position_angle = Mathf.Atan2(directionOfTable.z, directionOfTable.x)*Mathf.Rad2Deg;
        Debug.Log("start pos_angle:"+position_angle);

        cam = Camera.main;
    }


    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        position_angle += input * speed * Time.fixedDeltaTime;
        position = table.position;
        position.x = radius * Mathf.Cos(position_angle*Mathf.Deg2Rad);
        position.z = radius * Mathf.Sin(position_angle*Mathf.Deg2Rad);
        rb.MovePosition(position);
        rb.MoveRotation(Quaternion.Euler(Vector3.up*(90+cam.transform.eulerAngles.y)));
    }

}
