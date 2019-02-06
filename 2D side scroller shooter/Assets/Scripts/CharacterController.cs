using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    BoxCollider2D body;
    CircleCollider2D foots; 
       
    void Start()
    {
        body = GetComponent<BoxCollider2D>();
        foots = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("wall");
    }
}