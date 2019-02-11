using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    CharacterController chara;

    void Awake ()
    {
        chara = GetComponentInParent<CharacterController>();
    }

    void OnCollisionStay2D (Collision2D col)
    {
        chara.grounded = true;
    }

    void OnCollisionExit2D (Collision2D col)
    {
        chara.grounded = false;
    }
}
