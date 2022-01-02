using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragManager : MonoBehaviour
{
    Vector3 mousePos;
    Vector3 orientedMousePos;
    public static bool isDragging;
    Rigidbody2D pRb;
    public float followSpeed;
    int currentTouchIdx;
    // Start is called before the first frame update
    void Start()
    {
        pRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        mousePos = Input.mousePosition;
        mousePos.z = 10;
        orientedMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Collider2D col = Physics2D.OverlapPoint(orientedMousePos);
        if (col)
        {
            if(col.CompareTag("Player") && Input.GetMouseButtonDown(0))
            {
                isDragging = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
#endif
#if !UNITY_EDITOR
        for (int i = 0; i < Input.touchCount; i++)
        {
            mousePos = Input.touches[i].position;
            mousePos.z = 10;
            orientedMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Collider2D col2 = Physics2D.OverlapPoint(orientedMousePos);
            if (col2)
            {
                if (col2.CompareTag("Player") && Input.touches[i].phase == TouchPhase.Began)
                {
                    isDragging = true;
                    currentTouchIdx = i;
                }
            }
        }
        if (isDragging)
        {
            if (Input.touches[currentTouchIdx].phase == TouchPhase.Ended)
                isDragging = false;
        }
#endif        
    }
    private void FixedUpdate()
    {
        if (isDragging && Vector2.Distance(orientedMousePos, pRb.position) > .25f)
        {
            pRb.velocity = ((Vector2)orientedMousePos - pRb.position).normalized * followSpeed;
        }
    }
}
