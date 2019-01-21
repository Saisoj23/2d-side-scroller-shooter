using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool grounded;
    bool jumping;
    float jumpingTime;
    float hMovement;
    LayerMask playerLayer;
    Vector3 velocity;

    Rigidbody2D rb;
    Transform rayL;
    Transform rayR;
    RaycastHit2D hitL;
    RaycastHit2D hitR;

    [Header("Movemenet")]
    public float movementSpeed;
    public float movementAceleration;
    public float movementDesaceleracion;
    public float movementSensivility;
    [Header("Jump and Gravity")]
    public float gravity;
    public float jumpSpeed;
    public float jumpTime;
    public float jumpAceleration;
    public float jumpDesaceleration;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rayL = transform.Find("RayL");
        rayR = transform.Find("RayR");
        playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        hMovement = Input.GetAxisRaw("Horizontal");
        hitL = Physics2D.Raycast(rayL.position, new Vector2(0f,-1f), 0.05f, ~playerLayer);
        hitR = Physics2D.Raycast(rayR.position, new Vector2(0f,-1f), 0.05f, ~playerLayer);
        if (hitL.collider != null || hitR.collider != null)
        {
            grounded = true;
        } 
        else
        {
            grounded = false;
        }
        Debug.Log(grounded);
    }

    void FixedUpdate()
    {
        //Horizontal Move
        if (Mathf.Abs(hMovement) > movementSensivility)
        {
            if (hMovement > 0)
            {
                velocity.x += movementAceleration;
                if (velocity.x < 0)
                {
                    velocity.x += movementDesaceleracion;
                }
            }
            else
            {
                velocity.x -= movementAceleration;
                if (velocity.x > 0)
                {
                    velocity.x -= movementDesaceleracion;
                }
            }
        }
        else 
        {
            if (Mathf.Abs(velocity.x) < movementDesaceleracion)
            {
                velocity.x = 0f;
            }
            else if (velocity.x > 0)
            {
                velocity.x -= movementDesaceleracion;
            }
            else
            {
                velocity.x += movementDesaceleracion;
            }
        }
        //Gravity and Jump
        if (!grounded)
        {

        }
        
        velocity.x = Mathf.Clamp(velocity.x, -movementSpeed, movementSpeed);
        rb.MovePosition(rb.transform.position + (velocity * Time.deltaTime));
    }
}
