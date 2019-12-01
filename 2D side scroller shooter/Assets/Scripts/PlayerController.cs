using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    //can
    bool canJump;
    bool canDash = true;

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
    bool lastGrounded;

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
    Vector2 direccion;

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
    public float coyoteTime;
    public float wallJumpTime;
    public float wallJumpMultiplier;

    [Header("Level")]
    public Vector2 startPosition;

    Rigidbody2D rb;
    SpriteRenderer spr;
    BoxCollider2D col;
    ParticleSystem groundPcle;
    public ParticleSystem wallPcle;
    TrailRenderer trl;
    Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        groundPcle = GetComponent<ParticleSystem>();
        //wallPcle = GetComponentInChildren<ParticleSystem>();
        spr = GetComponentInChildren<SpriteRenderer>();
        trl = GetComponentInChildren<TrailRenderer>();
        anim = GetComponent<Animator>();
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

        if (dashing)
        {
            vInput = direccion.y;
            hInput = direccion.x;
        }

        direccion = new Vector2 (hInput, vInput).normalized;

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
                trl.emitting = true;
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
                trl.emitting = false;
            }
        }
        else if (Input.GetButtonUp("Fire3") && dash)
        {
            dashing = false;
            firstDashPress = false;
            trl.emitting = false;
        }
    }

    void FixedUpdate()
    {
        //recuperar velocidad
        Vector2 velocity = rb.velocity;

        if (Mathf.Abs(velocity.x) < 0.01f)
        {
            velocity.x = 0f;
        }

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

        if (!lastGrounded && grounded)
        {
            groundPcle.Play(false);
        }
        else if (lastGrounded && !grounded)
        {
            groundPcle.Stop(false);
        }

        lastGrounded = grounded;

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
            StartCoroutine("CancelCoyote");
        }

        //aplicar movimiento
        velocity.x += hInput * movementSpeed; 
        velocity.x = Mathf.Clamp(velocity.x, -movementMaxSpeed, movementMaxSpeed);
        if (hInput == 0)
        {
            velocity.x *= simulatedFriccion;
        }

        if (wallPcle.transform.localPosition.x > 0 && velocity.x < 0)
        {
            wallPcle.transform.localPosition = new Vector3(-0.5f, 0f, 0);
        }
        if (wallPcle.transform.localPosition.x < 0 && velocity.x > 0)
        {
            wallPcle.transform.localPosition = new Vector3(0.5f, 0f, 0);
        }

        //aplicar deslizamiento
        if (sliding)
        {
            if (!lastSliding)
            {
                wallPcle.Play();
            }
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
            if (lastSliding)
            {
                wallPcle.Stop();
            }
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
            //transform.localScale = new Vector3(1,0.5f,1);
            //transform.localEulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(new Vector2(0,0), direccion));
            jumping = false;
            secondJumping = false;
            //velocity.y = 0;
            rb.gravityScale = 0;
            //velocity.x = dashSpeed * lastSide;
            if (direccion == Vector2.zero)
            {
                velocity.x = dashSpeed * lastSide;
            }
            else
            {
                velocity = dashSpeed * direccion;
            }
            dashingTime += Time.deltaTime;
            framesJumping++;
        }
        else 
        {
            rb.gravityScale = 1;
            //transform.localScale = new Vector3(1,1,1);
            //transform.localEulerAngles = new Vector3(0,0,0);
        }

        //animaciones y flip
        anim.SetFloat("HSpeed", Mathf.Abs(velocity.x));
        anim.SetFloat("VSpeed", velocity.y);
        anim.SetBool("Grounded", grounded);
        anim.SetBool("Sliding", sliding);
        anim.SetBool("Dashing", dashing);
        if (hInput > 0)
        {
            spr.flipX = false;
        }
        else if (hInput < 0)
        {
            spr.flipX = true;
        }

        //aplicar velocidad
        rb.velocity = velocity;
        Debug.Log(velocity.x);
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.CompareTag("Damage"))
        {
            rb.transform.position = startPosition;
        }
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

    IEnumerator CancelCoyote ()
    {
        yield return new WaitForSeconds(coyoteTime);
        canJump = false;
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