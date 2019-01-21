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
    Vector3 groundNormal;

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
        if (hitL.collider != null || hitR.collider != null)
        {
            grounded = true;
            velocity.y -= hitL.collider != null ? rayL.position.y + hitL.point.y : rayR.position.y + hitR.point.y;
            if(hitL.normal != new Vector2(0f, 1f))
            {
                groundNormal = Quaternion.AngleAxis(90f, hitL.normal).eulerAngles;
            }
            else if (hitR.normal != new Vector2(0f, 1f))
            {
                groundNormal = Quaternion.AngleAxis(-90f, hitR.normal).eulerAngles;
            }
            else
            {
                groundNormal = Vector2.right;
            }
        } 
        else
        {
            grounded = false;
        }
        Debug.Log("left: " + hitL.normal +"  right: " + hitR.normal);
        //Horizontal Move
        if (Mathf.Abs(hMovement) > movementSensivility)
        {
            if (hMovement > 0)
            {
                velocity.x +=  movementAceleration;
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
            velocity.y -= jumpDesaceleration;
            velocity.y = Mathf.Clamp(velocity.y, gravity, jumpSpeed);
        }
        else
        {
            velocity.y = 0f;
        }
        if (jumping)
            {
                velocity.y += jumpAceleration;
            }
        velocity.x = Mathf.Clamp(velocity.x, -movementSpeed, movementSpeed);
        rb.MovePosition(rb.transform.position + (velocity * Time.deltaTime));
    }
}
