using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Public
    public float speed = 1;
    
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
        float xVal = dir * speed * 100 * Time.deltaTime;
        Vector2 targetVelocity = new Vector2(xVal,rb.velocity.y);
        rb.velocity = targetVelocity;
    }
}
