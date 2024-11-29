using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家角色相关
/// </summary>
public class Player : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;

    public float moveSpeed = 5; //移动速度

    private float inputX;   //X轴输入
    private float inputY;   //Y轴输入

    private Vector2 movementInput;  //移动向量
    private Animator[] animatorArray;   //Animator数组，获取所有子物体的Animator
    private bool isMoving;  //是否移动

    private bool inputIsDisable;    //输入是否关闭    


    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animatorArray = GetComponentsInChildren<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPositionEvent += OnMoveToPositionEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPositionEvent -= OnMoveToPositionEvent;
    }

    private void Update()
    {
        if(!inputIsDisable) 
        {
            //输入被禁用
            PlayerInput();
        }
        SwitchAnimation();
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

        if(Input.GetKey(KeyCode.LeftShift))
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);

        isMoving = movementInput != Vector2.zero;
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Movement()
    {
        rigidbody2d.MovePosition(rigidbody2d.position + movementInput * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 切换动画
    /// </summary>
    private void SwitchAnimation()
    {
        foreach (var anim in animatorArray)
        {
            anim.SetBool("IsMoving", isMoving);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputIsDisable = true;
    }
    private void OnAfterSceneLoadEvent()
    {
        inputIsDisable = false;
    }
    private void OnMoveToPositionEvent(Vector3 pos)
    {
        transform.position = pos;
    }


}
