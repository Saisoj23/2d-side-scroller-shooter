using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetFeatures : MonoBehaviour
{
    PlayerController player;

    public Toggle doubleJump;
    public Toggle wallJump;
    public Toggle dash;

    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
    }

    public void SetDoubleJump()
    {
        player.dobleJump = doubleJump.isOn;
    }

    public void SetWallJump()
    {
        player.wallJump = wallJump.isOn;
    }

    public void SetDash()
    {
        player.dash = dash.isOn;
    }
}
