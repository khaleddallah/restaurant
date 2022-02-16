using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{

    public int score;
    public float health;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "item")
        {
            health -= 1;
        }
    }


    public void AddScore(int amount)
    {
        score += amount;
    }


    public void Hurt(float amount)
    {
        health -= amount;
    }


    public void Heal(float amount)
    {
        health += amount;
    }




}
