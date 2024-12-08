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

    private float mouseX;   //鼠标X轴方向
    private float mouseY;   //鼠标Y轴方向
    private bool isUseTool; //是否使用工具

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
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPositionEvent -= OnMoveToPositionEvent;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }


    private void Update()
    {
        if (!inputIsDisable)
        {
            //输入没有被关闭
            PlayerInput();
        }
        else
        {
            //输入被关闭
            isMoving = false;   //停止移动
        }
        SwitchAnimation();
    }
    private void FixedUpdate()
    {
        if (!inputIsDisable)
        {
            //输入没有被关闭
            Movement();//移动
        }
    }

    /// <summary>
    /// 获取玩家输入 创建移动方向
    /// </summary>
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        //限制斜向移动距离，将每次斜向移动距离减小
        if (inputX != 0 && inputY != 0)
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }

        if (Input.GetKey(KeyCode.LeftShift))
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
            anim.SetFloat("MouseX", mouseX);
            anim.SetFloat("MouseY", mouseY);

            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        isUseTool = true;   //正在使用工具
        inputIsDisable = true;  //鼠标禁用
        yield return null;
        foreach (var anim in animatorArray)
        {
            anim.SetTrigger("UseTool");
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);

        }
        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExecuteActionAfterAnimationEvent(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);

        isUseTool = false;
        inputIsDisable = false;
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

    private void OnMouseClickedEvent(Vector3 mousePos, ItemDetails itemDetails)
    {
        //如果正在使用工具
        if(isUseTool) 
        {
            return;
        }

        //TODO:执行动画
        //现在没有使用工具，判断是否要使用工具
        if (itemDetails.itemType != E_ItemType.Seed || itemDetails.itemType != E_ItemType.Commodity || itemDetails.itemType != E_ItemType.Furniture | itemDetails.itemType != E_ItemType.None)
        {
            //计算角色面向哪个方向使用工具
            mouseX = mousePos.x - transform.position.x;
            mouseY = mousePos.y - (transform.position.y+1f);//鼠标y减去(人物坐标.y+身高)(因为是以人物脚下坐标计算的transform.position)

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))  //鼠标在x轴方向大于y轴方向距离
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            StartCoroutine(UseToolRoutine(mousePos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimationEvent(mousePos, itemDetails);    //播种什么的 不使用工具的事件
        }
    }

}
