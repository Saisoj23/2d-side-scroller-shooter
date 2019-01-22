using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool grounded;
    bool lastGround;
    bool jumping;
    float jumpingTime;
    float hMovement;
    LayerMask playerLayer;
    Vector2 velocity;
    Vector2 groundNormal;

    Rigidbody2D rb;
    Transform rayL;
    Transform rayR;
    Transform rayUL;
    Transform rayUR;
    RaycastHit2D hitL;
    RaycastHit2D hitR;
    RaycastHit2D hitUL;
    RaycastHit2D hitUR;

    [Header("Movemenet")]
    public float movementSpeed;
    public float movementAceleration;
    public float movementDesaceleracion;
    public float movementSensivility;
    public float aMultiplierOnAir;
    public float dMultiplierOnAir;
    public Vector2 maxWalkAngle;
    [Header("Jump and Gravity")]
    public float gravity;
    public float jumpSpeed;
    public float jumpTime;
    public float jumpAceleration;
    public float jumpDesaceleration;
    public float groundcheckDistance;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rayL = transform.Find("RayL");
        rayR = transform.Find("RayR");
        rayUL = transform.Find("RayUL");
        rayUR = transform.Find("RayUR");
        playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        hMovement = Input.GetAxisRaw("Horizontal");
        if (grounded && Input.GetButtonDown("Jump"))
        {
            jumping = true;
            jumpingTime = 0f;
        } 
        else if (!grounded && Input.GetButton("Jump"))
        {
            jumpingTime += Time.deltaTime;
        }
        if (Input.GetButtonUp("Jump") || jumpingTime > jumpTime)
        {
            jumping = false;
        }
    }

    void FixedUpdate()
    {
        //GrounCheck
        hitL = Physics2D.Raycast(rayL.position, new Vector2(0f,-1f), groundcheckDistance, ~playerLayer);
        hitR = Physics2D.Raycast(rayR.position, new Vector2(0f,-1f), groundcheckDistance, ~playerLayer);
        hitUL = Physics2D.Raycast(rayUL.position, new Vector2(0f,1f), groundcheckDistance, ~playerLayer, 0,5f);
        hitUR = Physics2D.Raycast(rayUR.position, new Vector2(0f,1f), groundcheckDistance, ~playerLayer, 0,5f);
        if (hitL.collider != null || hitR.collider != null)
        {
            grounded = true;
            velocity.y -= hitL.collider != null ? rayL.position.y + hitL.point.y : rayR.position.y + hitR.point.y;
            if (hitR.normal != new Vector2(0f,0f) && hitR.normal != new Vector2(0f, -1f))
            {
                groundNormal = Vector2.Perpendicular(hitR.normal);
            }
            else if(hitL.normal != new Vector2(0f,0f) && hitL.normal != new Vector2(0f, -1f))
            {
                groundNormal = Vector2.Perpendicular(hitL.normal);
            }
            else
            {
                groundNormal = new Vector2(1f,0f);
            }
            groundNormal = new Vector2(Mathf.Abs(groundNormal.x), -groundNormal.y);
        } 
        else
        {
            grounded = false;
        }
        //Debug.Log(groundNormal);
        if (hitUL.collider != null || hitUR.collider != null)
        {
            if (!grounded)
            {
                jumping = false;
            }
        }
        //Horizontal Move
        if (grounded)
        {
            velocity.y = 0f;
        }
        if (Mathf.Abs(hMovement) > movementSensivility)
        {
            if (grounded)
            {
                if (hMovement > 0)
                {
                    velocity += groundNormal * movementAceleration;
                    if (velocity.x < 0)
                    {
                        velocity += groundNormal * movementDesaceleracion;
                    }
                }
                else
                {
                    velocity -= groundNormal * movementAceleration;
                    if (velocity.x > 0)
                    {
                        velocity -= groundNormal * movementDesaceleracion;
                    }
                }
            }
            else 
            {
                if (hMovement > 0)
                {
                    velocity.x += movementAceleration * aMultiplierOnAir;
                    if (velocity.x < 0)
                    {
                        velocity.x += movementDesaceleracion * aMultiplierOnAir;
                    }
                }
                else
                {
                    velocity.x -= movementAceleration * aMultiplierOnAir;
                    if (velocity.x > 0)
                    {
                        velocity.x -= movementDesaceleracion * aMultiplierOnAir;
                    }
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
                if (grounded)
                {
                    velocity -= groundNormal * movementDesaceleracion;
                }
                else
                {
                    velocity.x -= movementDesaceleracion * dMultiplierOnAir;
                } 
            }
            else
            {
                if (grounded)
                {
                    velocity += groundNormal * movementDesaceleracion;
                }
                else
                {
                    velocity.x += movementDesaceleracion * dMultiplierOnAir;
                }
            }
        }
        //Gravity and Jump
        if (!grounded)
        {
            velocity.y -= jumpDesaceleration;
        }
        if (jumping)
            {
                velocity.y += jumpAceleration;
            }
        velocity.x = Mathf.Clamp(velocity.x, -movementSpeed, movementSpeed);
        velocity.y = Mathf.Clamp(velocity.y, gravity, jumpSpeed);
        Debug.Log(velocity);
        rb.MovePosition(rb.position + (velocity * Time.deltaTime));

        lastGround = grounded;
    }
}
