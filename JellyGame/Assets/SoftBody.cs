using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    public Rigidbody2D[,] points;
    public int row;
    public int col;
    public float frequency;
    public float distanceBetween;
    public float radius;
    public GameObject massPoint;
    public Vector3 center;
    public Collider2D[] colliders;
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Awake()
    {
        points = new Rigidbody2D[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                points[i, j] = Instantiate(massPoint, transform.position + new Vector3(j-((col-1)/2), i - ((row - 1) / 2))*distanceBetween, Quaternion.identity).GetComponent<Rigidbody2D>();
                points[i, j].transform.parent = transform;
                points[i, j].constraints = RigidbodyConstraints2D.FreezeRotation;
                points[i, j].GetComponent<CircleCollider2D>().radius = radius;
            }
        }
        for (int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {
                if(j - 1 >= 0) //왼쪽 스프링
                {
                    if(i - 1 >= 0)//왼쪽 위 스프링
                    {
                        SpringJoint2D spring1;
                        spring1 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                        spring1.connectedBody = points[i - 1, j - 1];
                    }if(i + 1 < row) //왼쪽 아래 스프링
                    {
                        SpringJoint2D spring2;
                        spring2 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                        spring2.connectedBody = points[i + 1, j - 1];
                    }
                    SpringJoint2D spring3;
                    spring3 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                    spring3.connectedBody = points[i, j - 1];
                }
                if (j + 1 < col) //오른쪽 스프링
                {
                    if (i - 1 >= 0)//왼쪽 위 스프링
                    {
                        SpringJoint2D spring1;
                        spring1 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                        spring1.connectedBody = points[i - 1, j + 1];
                    }
                    if (i + 1 < row) //왼쪽 아래 스프링
                    {
                        SpringJoint2D spring2;
                        spring2 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                        spring2.connectedBody = points[i + 1, j + 1];
                    }
                    SpringJoint2D spring3;
                    spring3 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                    spring3.connectedBody = points[i, j + 1];
                }
                if (i - 1 >= 0)//중간 위 스프링
                {
                    SpringJoint2D spring1;
                    spring1 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                    spring1.connectedBody = points[i - 1, j];
                }
                if (i + 1 < row) //중간 아래 스프링
                {
                    SpringJoint2D spring2;
                    spring2 = points[i, j].gameObject.AddComponent<SpringJoint2D>();
                    spring2.connectedBody = points[i + 1, j];
                }
            }
        }
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                for(int k = 0; k < points[i,j].GetComponents<SpringJoint2D>().Length; k++)
                {
                    SpringJoint2D spring = points[i, j].GetComponents<SpringJoint2D>()[k];
                    spring.frequency = frequency;
                }
            }
        }
        points[col / 2, row / 2].gameObject.AddComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
        StartCoroutine(RepeatResettingPos());
        gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        colliders = new Collider2D[row * col];
        for(int i = 0,k=0; i < row; i++)
        {
            for(int j  =0; j < col; j++)
            {
                colliders[k] = points[i, j].GetComponent<Collider2D>();
                k++;
            }
        }
    }
    IEnumerator RepeatResettingPos()
    {
        while (true)
        {
            bool isOnGround = false;
            foreach(Collider2D pCol in colliders)
            {
                if (pCol.IsTouchingLayers(groundLayer))
                {
                    isOnGround = true;
                }
            }
            if(!DragManager.isDragging && isOnGround)
                ResetPointPos();
            yield return new WaitForSeconds(3);
        }
    }
    void ResetPointPos()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {                
                points[i, j].transform.position = transform.position + new Vector3(j - ((col - 1) / 2), i - ((row - 1) / 2)) * distanceBetween;
            }
        }
    }
}
