using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 10);
    }


    public float jumpPower;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            Rigidbody2D rb = FindObjectOfType<DashDashController>().rb;
            rb.velocity = Vector2.zero;
            rb.AddForce(jumpPower*Vector2.up,ForceMode2D.Impulse);

        }
    }
}
