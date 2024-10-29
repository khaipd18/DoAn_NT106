using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Public
    public float speed = 1;
    bool facingRight = true;
    
    //Private
    Rigidbody2D rb;
    float horizontalValue;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        Move(horizontalValue);
    }

    void Move(float dir)
    {
        //Move and speed of the movement
        float xVal = dir * speed * 100 * Time.deltaTime;
        Vector2 targetVelocity = new Vector2(xVal,rb.velocity.y);
        rb.velocity = targetVelocity;

        //flipping sprite with movement
        //If looking right and clicked left (flip to the left)
        if (facingRight && dir < 0)
        {
            transform.localScale = new Vector3(-6, 6, 1);
            facingRight = false;
        }
        //If looking left and clicked right (flip to rhe right)
        else if (!facingRight && dir > 0)
        {
            transform.localScale = new Vector3(6, 6, 1);
            facingRight = true;
        }
    }
}
