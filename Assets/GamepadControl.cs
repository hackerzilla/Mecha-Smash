using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class GamepadControl : MonoBehaviour
{
    public float horizontal_move_speed = 1.0f;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        string[] joysticks = Input.GetJoystickNames();
        foreach (string joystick in joysticks) {
            print(joystick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal_axis = Input.GetAxis("Horizontal");
        // print("Horizontal=" + horizontal_axis.ToString());
        if (horizontal_axis != 0.0)
        {
            rb.velocity = new Vector2(horizontal_axis * horizontal_move_speed, rb.velocity.y);
        }
    }
}
