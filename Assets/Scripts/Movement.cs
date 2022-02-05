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
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        radius = table.localScale.x/2 + tableOffset;

        Vector3 directionOfTable = transform.position - table.position;
        position_angle = Mathf.Atan2(directionOfTable.z, directionOfTable.x)*Mathf.Rad2Deg;
        Debug.Log("start pos_angle:"+position_angle);
    }

    // Update is called once per frame
    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        position_angle += input * speed * Time.deltaTime;
        Debug.Log("pos_angle:"+position_angle);
    }

    void FixedUpdate()
    {
        Vector3 position = table.position;
        position.x = radius * Mathf.Cos(position_angle*Mathf.Deg2Rad);
        position.z = radius * Mathf.Sin(position_angle*Mathf.Deg2Rad);
        Debug.Log("pos:"+position);
        rb.MovePosition(position);
        rb.MoveRotation(Quaternion.Euler(Vector3.up*(position_angle)));
    }
    
}