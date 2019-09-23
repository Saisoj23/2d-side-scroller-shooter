using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    GameObject player;
    public float speed;
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
    }

    void Update()
    {
        Vector3 target = player.transform.position;
        target.z = -1;
        target.y += 1.5f;
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
    }
}
