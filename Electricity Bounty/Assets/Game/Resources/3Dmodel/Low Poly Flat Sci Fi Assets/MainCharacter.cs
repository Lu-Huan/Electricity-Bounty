using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{

    public float Speed;
    private Transform transform;
    private Animation Animation;
    private bool IsWalk=true;
    // Use this for initialization
    void Start()
    {
        transform = GetComponent<Transform>();
        Animation = GetComponent<Animation>();
        Animation.Play("idle");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Animation.Play("wallJump");
        }
        if (Input.GetKey(KeyCode.K))
        {
            Animation.Play("jumpAttack");
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (Mathf.Abs( x)+ Mathf.Abs(z) < 0.3)
        {
            if (IsWalk)
            {
                Animation.Play("idle");
                IsWalk = false;
            }
            return;
        }
        else if (!IsWalk)
        {
            Animation.Play("walk");
            IsWalk = true;
        }
        Vector3 dir = new Vector3(x, 0, z);
        Vector3 Start = new Vector3(0,0,1);
        transform.position += dir * Time.deltaTime * Speed;
        
        float angle = Vector3.Angle(Start, dir); //求出两向量之间的夹角
        Vector3 normal = Vector3.Cross(Start, dir);//叉乘求出法线向量  
        angle *= Mathf.Sign(Vector3.Dot(normal, new Vector3(0, 1, 0)));  //求法线向量与物体上方向向量点乘，结果为1或-1，修正旋转方向 
        //float current= Mathf.Lerp(transform.localRotation.y, angle, 0.5f);
        transform.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }
}
