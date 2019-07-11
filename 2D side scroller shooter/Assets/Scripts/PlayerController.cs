using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    //can
    bool canJump;
    bool canDash;

    //doing
    bool jumping;
    bool wallJumping;
    bool secondJumping;
    bool dashing;

    //has
    bool hasSecondJumped;
    bool hasDash;

    //time
    float jumpingTime;
    float dashingTime;

    //colisions
    bool grounded;

    bool ceiling;

    int wall;
    int lastSlide;

    bool sliding;
    bool lastSliding;

    //first press
    bool firstJumpPress;
    bool firstDashPress;

    //input
    float hInput;
    float vInput;
    int lastSide;

    int framesJumping;

    [Header("Activar / Descativar Extras")]
    public bool dobleJump;
    public bool wallJump;
    public bool dash;

    [Header("Movimiento")]
    public float minMovementSensivility;
    public float movementSpeed;
    public float movementMaxSpeed;
    public float airControlMultiplier;
    public float simulatedFriccion;
    public float castDistance;

    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    public float dashWait;

    [Header("Slide")]
    public float wallSliceVelocity;
    public float cancelSlidingTime;

    [Header("Salto")]
    public float jumpingForce;
    public float jumpingMaxTime;
    public float secondJumpingForce;
    public float secondJumpingMaxTime;
    public float jumpingReleaseStop;
    public float preGroundTime;
    public float wallJumpTime;
    public float wallJumpMultiplier;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        //obtener input horizontal
        hInput = Input.GetAxisRaw("Horizontal");
        if (hInput > minMovementSensivility) 
        {
            hInput = 1;
            lastSide = 1;
        }
        else if (hInput < -minMovementSensivility) 
        {
            hInput = -1;
            lastSide = -1;
        }
        else 
        {
            hInput = 0;
        }

        if (wallJumping && hInput >= 0 && lastSlide < 0)
        {
            hInput = -lastSlide;
        }
        else if (wallJumping && hInput <= 0 && lastSlide > 0)
        {
            hInput = -lastSlide;
        }
        else if (wallJumping)
        {
            hInput = -lastSlide * wallJumpMultiplier;
        }

        //obtener input vertical
        vInput = Input.GetAxisRaw("Vertical");
        if (vInput > minMovementSensivility) 
        {
            vInput = 1;
        }
        else if (vInput < -minMovementSensivility) 
        {
            vInput = -1;
        }
        else 
        {
            vInput = 0;
        }

        //obtener salto
        if (Input.GetButtonDown("Jump") && !dashing)
        {
            firstJumpPress = true;
            StartCoroutine("CancelFirstJump");
            if (!grounded && !sliding && !hasSecondJumped && dobleJump)
            {
                secondJumping = true;
                hasSecondJumped = true;
                jumpingTime = 0f;
            }
        } 
        if (Input.GetButton("Jump") && !dashing) 
        {
            if (canJump && firstJumpPress)
            {
                if (sliding)
                {
                    wallJumping = true;
                    StartCoroutine("CancelWallJump");
                }
                jumping = true;
                jumpingTime = 0f;
                firstJumpPress = false;
                framesJumping = 0;
                hasSecondJumped = false;
            }
            else if (jumping && jumpingTime > jumpingMaxTime)
            {
                jumping = false;
                firstJumpPress = false;
            }
            else if (secondJumping && jumpingTime > secondJumpingMaxTime)
            {
                secondJumping = false;
            }
        }
        else if (Input.GetButtonUp("Jump") && !dashing)
        {
            jumping = false;
            wallJumping = false;
            secondJumping = false;
            firstJumpPress = false;
        }

        //obtener dash
        if (Input.GetButtonDown("Fire3") && dash)
        {
            firstDashPress = true;
            StartCoroutine("CancelFirstDash");
            StartCoroutine("NewDash");
        } 
        if (Input.GetButton("Fire3") && dash)
        {
            if (canDash && !hasDash && firstDashPress)
            {
                dashing = true;
                hasDash = true;
                dashingTime = 0f;
                firstDashPress = false;
                canDash = false;
                StartCoroutine("NewDash");
            }
            else if (dashing && dashingTime > dashTime)
            {
                dashing = false;
                firstDashPress = false;
            }
        }
        else if (Input.GetButtonUp("Fire3") && dash)
        {
            dashing = false;
            firstDashPress = false;
        }
    }

    void FixedUpdate()
    {
        //recuperar velocidad
        Vector2 velocity = rb.velocity;

        //verificar suelo
        RaycastHit2D[] hits = new RaycastHit2D[3];
        if (rb.Cast(Vector2.down, hits, castDistance) > 0)
        {
            grounded = true;
            canJump = true;
            sliding = false;
        }
        else
        {
            grounded = false;
        }

        //verificar techo
        if (rb.Cast(Vector2.up, hits, castDistance) > 0)
        {
            jumping = false;
            secondJumping = false;
            ceiling = true;
        }
        else 
        {
            ceiling = false;
        }

        //verificar paredes
        if (rb.Cast(Vector2.right, hits, castDistance) > 0)
        {
            wall = 1;
            if (hInput > 0 && velocity.y < 0 && wallJump)
            {
                sliding = true;
            }
            else if(!lastSliding)
            {
                StartCoroutine("CancelSlide");
            }
        }
        else if (rb.Cast(Vector2.left, hits, castDistance) > 0)
        {
            wall = -1;
            if (hInput < 0 && velocity.y < 0 && wallJump)
            {
                sliding = true;
            }
            else if(!lastSliding)
            {
                StartCoroutine("CancelSlide");
            }
        }
        else
        {
            wall = 0;
            sliding = false;
        }

        //puede saltar?
        if (grounded || sliding)
        {
            canJump = true;
            hasDash = false;
        }
        else 
        {
            canJump = false;
        }

        //aplicar movimiento
        velocity.x += hInput * movementSpeed; 
        velocity.x = Mathf.Clamp(velocity.x, -movementMaxSpeed, movementMaxSpeed);
        if (hInput == 0)
        {
            velocity.x *= simulatedFriccion;
        }

        //aplicar deslizamiento
        if (sliding)
        {
            lastSlide = wall;
            lastSliding = true;
            canJump = true;
            rb.gravityScale = wallSliceVelocity;
            if (vInput < 0)
            {
                sliding = false;
            }
        }       
        else 
        {
            lastSliding = false;
            rb.gravityScale = 1;
        }

        //aplicar salto
        if (jumping)
        {
            velocity.y = jumpingForce * Mathf.InverseLerp(jumpingMaxTime, 0f, jumpingTime);
            velocity.x *= airControlMultiplier;
            jumpingTime += Time.deltaTime;
            framesJumping++;
        }
        else if (secondJumping)
        {
            velocity.y = secondJumpingForce * Mathf.InverseLerp(secondJumpingMaxTime, 0f, jumpingTime);
            velocity.x *= airControlMultiplier;
            jumpingTime += Time.deltaTime;
            framesJumping++;
        }
        else if (!grounded && velocity.y > 0f)
        {
            velocity.y *= jumpingReleaseStop;
        }

        //aplicar dash
        if (dashing)
        {
            transform.localScale = new Vector3(1,0.5f,1);
            jumping = false;
            secondJumping = false;
            velocity.y = 0;
            rb.gravityScale = 0;
            velocity.x = dashSpeed * lastSide;
            dashingTime += Time.deltaTime;
            framesJumping++;
        }
        else 
        {
            rb.gravityScale = 1;
            transform.localScale = new Vector3(1,1,1);
        }

        //aplicar velocidad
        rb.velocity = velocity;
        Debug.Log(dashing);
    }

    IEnumerator CancelFirstJump ()
    {
        yield return new WaitForSeconds(preGroundTime);
        firstJumpPress = false;
    }

    IEnumerator CancelFirstDash ()
    {
        yield return new WaitForSeconds(preGroundTime);
        firstDashPress = false;
    }

    IEnumerator CancelWallJump ()
    {
        yield return new WaitForSeconds(wallJumpTime);
        wallJumping = false;
    }

    IEnumerator CancelSlide ()
    {
        yield return new WaitForSeconds(cancelSlidingTime);
        sliding = false;
    }

    IEnumerator NewDash ()
    {
        yield return new WaitForSeconds(dashWait);
        canDash = true;
    }
}