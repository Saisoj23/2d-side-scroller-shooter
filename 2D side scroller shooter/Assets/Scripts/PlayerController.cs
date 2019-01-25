using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //variables internas
    bool grounded;
    bool lastGround;
    bool jumping;
    bool groundReferenceRight;
    bool colidingLeft;
    bool colidingRight;
    float jumpingTime;
    float hotizontalInput;
    float groundDistance;
    float ceilingDistance;
    float rightDistance;
    float leftDistance;
    LayerMask playerLayer;
    Vector2 velocity;
    Vector2 groundNormal;
    RaycastHit2D hitL;
    RaycastHit2D hitR;

    //componentes y objetos
    Rigidbody2D rb;
    Transform rayL;
    Transform rayR;
    Transform rayUL;
    Transform rayUR;
    Transform rayML;
    Transform rayMR;

    //variables publicas
    [Header("Movemenet")]
    public float movementSpeed;
    public float movementAceleration;
    public float movementDesaceleracion;
    public float movementSensivility;
    public float aMultiplierOnAir;
    public float dMultiplierOnAir;
    public float maxWalkHeight;
    public float sliceMultiplier;
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
        rayML = transform.Find("RayML");
        rayMR = transform.Find("RayMR");
        playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        //input
        hotizontalInput = Input.GetAxisRaw("Horizontal");
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
        colidingLeft = GetLeftWall();
        colidingRight = GetRightWall();
        if (colidingLeft && colidingRight)
        {
            Debug.Log("Die");
        }
        else if (colidingRight && velocity.x > 0f)
        {
            velocity.x = 0f;
            if (rightDistance > 0.02f)
            {
                velocity.x = rightDistance;
            }
        }
        else if (colidingLeft && velocity.x < 0f)
        {
            velocity.x = 0f;
            if (rightDistance > 0.02f)
            {
                velocity.x = -rightDistance;
            }
        }
        grounded = GetGround();
        if (grounded)
        {
            MovementOnGround();
            if (!lastGround)
            {
                velocity.y = 0f;
                if (groundDistance > 0.02f)
                {
                    velocity.y = groundDistance;
                }
            }
            if (GetCeiling())
            {
                Debug.Log("Die");
            }
        }
        else 
        {
            MovementOnAir();
            velocity.y -= jumpDesaceleration;
            if (GetCeiling())
            {
                jumping = false;
            }
        }
        if (jumping)
        {
            velocity.y += jumpAceleration * Mathf.InverseLerp(jumpTime, 0f, jumpingTime);
        }
        velocity.x = Mathf.Clamp(velocity.x, -movementSpeed, movementSpeed);
        velocity.y = Mathf.Clamp(velocity.y, gravity, jumpSpeed);
        Debug.Log(velocity);
        Debug.Log(colidingLeft + " left " + colidingRight + " right ");
        rb.MovePosition(rb.position + (velocity * Time.deltaTime));

        lastGround = grounded;
    }

    bool GetGround()
    {
        RaycastHit2D hitL = Physics2D.Raycast(rayL.position, new Vector2(0f,-1f), groundcheckDistance, ~playerLayer);
        RaycastHit2D hitR = Physics2D.Raycast(rayR.position, new Vector2(0f,-1f), groundcheckDistance, ~playerLayer);
        if (hitL.collider != null || hitR.collider != null)
        {
            grounded = true;
            groundNormal = new Vector2(1f,0f);
            groundReferenceRight = true;
            groundDistance = hitR.distance;
            if (hitR.normal != new Vector2(0f,0f) && hitR.normal != new Vector2(0f, -1f))
            {
                groundNormal = Vector2.Perpendicular(hitR.normal);
                groundReferenceRight = true;
                groundDistance = hitR.distance;
            }
            else if(hitL.normal != new Vector2(0f,0f) && hitL.normal != new Vector2(0f, -1f))
            {
                groundNormal = Vector2.Perpendicular(hitL.normal);
                groundReferenceRight = false;
                groundDistance = hitL.distance;
            }
            groundNormal = new Vector2(Mathf.Abs(groundNormal.x), -groundNormal.y);
        }
        else
        {
            grounded = false;
            groundNormal = new Vector2(0f,0f);
            groundReferenceRight = true;
            groundDistance = 0f;
        }
        return grounded;
    }

    bool GetCeiling()
    {
        hitL = Physics2D.Raycast(rayUL.position, new Vector2(0f,1f), groundcheckDistance, ~playerLayer, 0,5f);
        hitR = Physics2D.Raycast(rayUR.position, new Vector2(0f,1f), groundcheckDistance, ~playerLayer, 0,5f);
        ceilingDistance = 1f;
        if (hitR.collider != null)
        {
            ceilingDistance = hitR.distance;
        }
        if (hitL.collider != null && hitL.distance < ceilingDistance)
        {
            ceilingDistance = hitL.distance;
        }
        return ceilingDistance < 1f;
    }

    void MovementOnGround()
    {
        if (Mathf.Abs(hotizontalInput) > movementSensivility)
        {
            if (hotizontalInput > 0 && (groundNormal.y < maxWalkHeight || groundReferenceRight) && !colidingRight)
            {
                velocity += groundNormal * movementAceleration;
                if (velocity.x < 0)
                {
                    velocity += groundNormal * movementDesaceleracion;
                }
            }
            else if (hotizontalInput < 0 && (groundNormal.y < maxWalkHeight || !groundReferenceRight) && !colidingLeft)
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
            if (Mathf.Abs(velocity.x) < movementDesaceleracion * groundNormal.x)
            {
                velocity.x = 0f;
            }
            if (velocity.x > 0)
            {
                velocity -= groundNormal * movementDesaceleracion;
            }
            else if (velocity.x < 0)
            {
                velocity += groundNormal * movementDesaceleracion;
            }
        }
        if (velocity.x == 0 && velocity.y != 0)
        {
            velocity.y = 0f;
        }
        if (groundNormal.y > maxWalkHeight && !groundReferenceRight)
        {
            velocity += groundNormal * movementAceleration;
            if (velocity.x < 0)
            {
                velocity += groundNormal * (movementDesaceleracion * -groundNormal.y * sliceMultiplier);
            }
        }
        else if ((groundNormal.y > maxWalkHeight && groundReferenceRight))
        {
            velocity -= groundNormal * movementAceleration;
            if (velocity.x > 0)
            {
                velocity -= groundNormal * (movementDesaceleracion * -groundNormal.y * sliceMultiplier);
            }
        }
    }

    void MovementOnAir()
    {
        if (Mathf.Abs(hotizontalInput) > movementSensivility && groundNormal.y < maxWalkHeight)
        {
            if (hotizontalInput > 0)
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
        else
        {
            
            if (Mathf.Abs(velocity.x) < movementDesaceleracion)
            {
                velocity.x = 0f;
            }
            if (velocity.x > 0)
            {
                velocity.x -= movementDesaceleracion * dMultiplierOnAir;
            }
            else if (velocity.x < 0)
            {
                velocity.x += movementDesaceleracion * dMultiplierOnAir;
            }
        }
    }

    bool GetRightWall()
    {
        bool wallCheck = false;
        hitR = Physics2D.Raycast(rayUR.position, new Vector2(1f, 0f), groundcheckDistance, ~playerLayer);
        if (hitR.collider != null) 
        {
            rightDistance = hitR.distance;
            wallCheck = true;
        }
        hitR = Physics2D.Raycast(rayMR.position, new Vector2(1f, 0f), groundcheckDistance, ~playerLayer);
        if (hitR.collider != null && hitR.distance < leftDistance) 
        {
            rightDistance = hitR.distance;
            wallCheck = true;
        }
        hitR = Physics2D.Raycast(rayR.position, new Vector2(1f, 0f), groundcheckDistance, ~playerLayer);
        if (hitR.collider != null && hitR.distance < rightDistance && hitR.normal.y > maxWalkHeight) 
        {
            rightDistance = hitR.distance;
            wallCheck = true;
        }
        return wallCheck;
    }

    bool GetLeftWall()
    {
        bool wallCheck = false;
        hitL = Physics2D.Raycast(rayUL.position, new Vector2(-1f, 0f), groundcheckDistance, ~playerLayer);
        if (hitL.collider != null) 
        {
            leftDistance = hitL.distance;
            wallCheck = true;
        }
        hitL = Physics2D.Raycast(rayML.position, new Vector2(-1f, 0f), groundcheckDistance, ~playerLayer);
        if (hitL.collider != null && hitL.distance < leftDistance) 
        {
            leftDistance = hitL.distance;
            wallCheck = true;
        }
        hitL = Physics2D.Raycast(rayL.position, new Vector2(-1f, 0f), groundcheckDistance, ~playerLayer);
        if (hitL.collider != null && hitL.distance < leftDistance && hitL.normal.y > maxWalkHeight) 
        {
            leftDistance = hitL.distance;
            wallCheck = true;
        }
        return wallCheck;
    }
}
