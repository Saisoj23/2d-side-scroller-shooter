using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float hMovement;

    Rigidbody2D rb;

    [Header("Movemenet")]
    public float speed;
    public float aceleration;
    [Header("Jump and Gravity")]
    public float gravity;
    public float jumpPower;
    public float JumpTime;
    public float JumpAceleration;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        hMovement = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        
    }
}
