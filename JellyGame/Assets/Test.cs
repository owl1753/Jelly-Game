using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI t;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(Input.acceleration.x*10, rb.velocity.y);
    }
    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }
}
