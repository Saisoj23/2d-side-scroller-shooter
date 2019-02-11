using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{   

    [HideInInspector] public bool grounded;
    bool jumping;
    float jumpingTime;
    float movement;
    int framesJumping;

    [Header("Movimiento Horizontal")]
    public float minMovementSensivility;
    public float movementSpeed;
    public float movementMaxSpeed;
    public float airControlMultiplier;
    public float simulatedFriccion;
    [Header("Salto")]
    public float jumpingForce;
    public float jumpingMaxTime;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        movement = Input.GetAxisRaw("Horizontal");
        if (movement > minMovementSensivility)
        {
            movement = 1;
        }
        else if (movement < -minMovementSensivility)
        {
            movement = -1;
        }
        else
        {
            movement = 0;
        }
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jumping = true;
            jumpingTime = 0f;
            framesJumping = 0;
        } 
        else if (Input.GetButton("Jump"))
        {
            if (jumpingTime > jumpingMaxTime)
            {
                jumping = false;
            }
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumping = false;
            Debug.Log(framesJumping);
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = new Vector2(movement * movementSpeed, 0f);
        if (jumping)
        {
            velocity += new Vector2(0f, jumpingForce * Mathf.InverseLerp(jumpingMaxTime, 0f, jumpingTime));
            jumpingTime += Time.deltaTime;
            framesJumping++;
            if (!grounded && rb.velocity.y < 0.001f)
            {
                jumping = false;
            }
        }
        if (!grounded)
        {
            velocity.x *= airControlMultiplier;
        }
        else
        {
            rb.velocity *= simulatedFriccion;
            jumpingTime = 0f;
        }
        rb.AddForce(velocity, ForceMode2D.Impulse);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -movementMaxSpeed, movementMaxSpeed), rb.velocity.y);
    }
}