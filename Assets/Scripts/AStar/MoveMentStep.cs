using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.AStarAlgorithm{
    /// <summary>
    /// NPC指定移动的点的信息
    /// </summary>
    public class MoveMentStep 
    {
        public string sceneName;    //场景名
        public int hour;
        public int minute;
        public int second;
        public Vector2Int gridCoordinate;   //网格坐标
    }
}