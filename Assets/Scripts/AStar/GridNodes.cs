using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStarAlgorithm
{
    /// <summary>
    /// 整个地图的数据类型
    /// </summary>
    public class GridNodes 
    {
        private int width;  //地图整体宽
        private int height; //地图整体高
        private Node[,] gridNode;   //整个地图中所有的格子坐标

        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;
            gridNode = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridNode[x,y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        /// <summary>
        /// 获取单个格子
        /// </summary>
        /// <param name="xPos">x坐标</param>
        /// <param name="yPos">y坐标</param>
        /// <returns>单个格子</returns>
        public Node GetGridNode(int xPos, int yPos)
        {
            if (xPos < width && yPos < height)
            {
                return gridNode[xPos,yPos];
            }
            Debug.Log("超出网格范围");
            return null;
        }

    }

}
