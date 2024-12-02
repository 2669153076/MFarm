using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTrans;  //物体坐标
        private BoxCollider2D coll;

        public float gravity = 3.5f;   //重力
        private bool isGround;  //是否在地面
        private float distance; //距离
        private Vector2 direction;   //方向
        private Vector3 targetPos;  //目标坐标

        private void Awake()
        {
            spriteTrans = transform.GetChild(0);
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;
        }

        private void Update()
        {
            Bounce();
        }

        /// <summary>
        /// 初始化自由落体属性
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <param name="dir">方向</param>
        public void InitBounceItem(Vector3 target,Vector2 dir)
        {
            coll.enabled = false;
            direction = dir;
            targetPos = target;
            distance = Vector3.Distance(target, transform.position);

            spriteTrans.position += Vector3.up * 1.5f;   //1.5f即物体在人物头顶据人物坐标的y轴距离
        }

        /// <summary>
        /// 自由落体
        /// </summary>
        private void Bounce()
        {
            isGround = spriteTrans.position.y <= transform.position.y;

            if ((Vector3.Distance(transform.position,targetPos)>0.1f))  //数值太小物体可能飞出去
            {
                //横轴移动
                transform.position += (Vector3)direction * distance * gravity*Time.deltaTime;
            }
            if(!isGround)
            {
                //纵轴移动
                spriteTrans.position += Vector3.up * -gravity * Time.deltaTime;
            }
            else
            {
                spriteTrans.position = transform.position;
                coll.enabled = true;
            }
        }
    }
}