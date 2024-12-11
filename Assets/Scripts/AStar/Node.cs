using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.AStarAlgorithm
{
    /// <summary>
    /// 单个格子属性
    /// </summary>
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; //网格坐标
        public int gCost = 0;   //离起点距离
        public int hCost = 0;   //离终点距离
        public int FCost => gCost + hCost;  //当前格子寻路总消耗
        public bool isObstacle = false; //当前格子是否是障碍物
        public Node parentNode; //该节点父对象

        public Node(Vector2Int pos)
        {
            this.gridPosition = pos;
            parentNode = null;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// -1 小于<br/>
        /// 0 等于<br/>
        /// 1 大于
        /// </returns>
        public int CompareTo(Node other)
        {
            //比较选出最低的F 返回-1 0 1
            int result = FCost.CompareTo(other.FCost);
            if(result == 0)
            {
                //如果总消耗相等，则比较离终点的距离
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}