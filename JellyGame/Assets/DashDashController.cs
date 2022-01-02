using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDashController : MonoBehaviour
{
    public Rigidbody2D rb;
    public DashDashManager manager;
    public float jumpPower;
    public bool onGround;

    public float dashDuration;
    public float dashTimer;
    public float dashSpeed;
    public GameObject clonePrefab;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.hp <= 0)
            return;
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);        
    }
    public void Jump()
    {
        if (manager.hp <= 0)
            return;
        if (onGround && dashTimer <= 0)
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
    public void Dash()
    {
        if (manager.hp <= 0)
            return;
        if (dashTimer <= 0)
            StartCoroutine(DashRoutine());
    }
    IEnumerator DashRoutine()
    {
        dashTimer = dashDuration;
        while(dashTimer > 0)
        {
            manager.obstacleHolder.position += new Vector3(-dashSpeed * Time.fixedDeltaTime, 0, 0);
            rb.velocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
            dashTimer -= Time.fixedDeltaTime;
            Instantiate(clonePrefab, transform.position, Quaternion.identity);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            onGround = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            onGround = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            onGround = false;
    }
}
