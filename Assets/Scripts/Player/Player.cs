using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家角色相关
/// </summary>
public class Player : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;

    public float moveSpeed;

    private float inputX;
    private float inputY;

    private Vector2 movementInput;


    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerInput();
    }
    private void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// 获取玩家输入 创建移动方向
    /// </summary>
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        //限制斜向移动距离，将每次斜向移动距离减小
        if(inputX!=0&& inputY!=0 )
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }

        movementInput = new Vector2(inputX, inputY);
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Movement()
    {
        rigidbody2d.MovePosition(rigidbody2d.position + movementInput * moveSpeed * Time.deltaTime);
    }
}
