using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody2D theRigidBody;
    public float velocidadHorizontal = 8f;
    public float impulso = 6f;
    private Vector2 velocidadMovimiento;
    // Start is called before the first frame update
    void Start()
    {
        theRigidBody = GetComponent<Rigidbody2D>();
    }

    private float horizontal;
    // Update is called once per frame
    void Update()
    {
        Debug.Log(horizontal);
        horizontal = Input.GetAxis("Horizontal");
        horizontal = horizontal * velocidadHorizontal;
        velocidadMovimiento = new Vector2(horizontal, theRigidBody.velocity.y);
    }

    private void FixedUpdate()
    {
        theRigidBody.MovePosition(theRigidBody.position + velocidadMovimiento * Time.fixedDeltaTime);
    }
}
