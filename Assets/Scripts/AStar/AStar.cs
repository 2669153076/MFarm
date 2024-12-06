using GridMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStarAlgorithm{
    /// <summary>
    /// A星寻路
    /// </summary>
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;    //地图
        private Node startNode; //起点
        private Node endNode;   //终点(目标点)
        private int gridWidth;  //地图宽
        private int gridHeight; //地图高
        private int originX;    //原点X
        private int originY;    //原点Y

        private List<Node> openNodeList;    //当前选中Node周围的点
        private HashSet<Node> closedNodeList;   //  所有被选中的点
        private bool pathFound; //是否找到了最短路径


        /// <summary>
        /// 生成网格节点
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="startPos">起始位置</param>
        /// <param name="endPos">终点位置</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName,Vector2Int startPos,Vector2Int endPos)
        {
            if(GridMapMgr.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin))
            {
                gridNodes = new GridNodes(gridDimensions.x,gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
            {
                return false;
            }

            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            endNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    var key = tilePos.x + "x" + tilePos.y + "y" + sceneName;
                    TileDetails tileDetails = GridMapMgr.Instance.GetTileDetails(key);

                    if(tileDetails != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tileDetails.isNPCObstacle)
                        {
                            node.isObstacle = true;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 构建最短路径
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos,Stack<MoveMentStep> npcMoveMentStepStack)
        {
            pathFound = false;

            if(GenerateGridNodes(sceneName, startPos, endPos))
            {
                //查找最短路径
                if (FindShortestPath())
                {
                    //构建NPC移动路径
                    UpdatePathOnMovementStepStack(sceneName, npcMoveMentStepStack);
                }
            }
        }

        /// <summary>
        /// 寻找最短路径
        /// </summary>
        private bool FindShortestPath()
        {
            openNodeList.Add(startNode);


            while (openNodeList.Count > 0)
            {
                openNodeList.Sort();

                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == endNode)
                {
                    pathFound = true;
                    break;
                }

                //计算周围8个Node
                EvaluateNeighbourNodes(closeNode);

            }

            return pathFound;
        }
        /// <summary>
        /// 计算周围的格子
        /// </summary>
        /// <param name="currentNode">当前被计算的点</param>
        private void  EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    validNeighbourNode  = GetValidNeighbourNode(currentNodePos.x + x,currentNodePos.y+ y);

                    if(validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            //不为空、不在开启列表、不是障碍、不在关闭列表
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, endNode);

                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);

                        }
                    }
                }
            }
        }
        /// <summary>
        /// 找到邻近的有效的Node    <br/>
        /// 非障碍、非已选择(非关闭列表)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>邻近的有效的Node</returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
            {
                return null;
            }

            Node neighbourNode = gridNodes.GetGridNode(x, y);
            if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
            {
                //如果邻近节点是障碍或在关闭列表中
                return null ;
            }
            else
            {
                return neighbourNode;
            }
        }
        /// <summary>
        /// 返回两点的距离值
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>14的倍数+10的倍数</returns>
        private int GetDistance(Node left, Node right)
        {
            int xDistance = Mathf.Abs(left.gridPosition.x - right.gridPosition.x);
            int yDistance = Mathf.Abs(left.gridPosition.y - right.gridPosition.y);

            if(xDistance > yDistance)
            {
                return 14*yDistance+10*(xDistance-yDistance);
            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
        /// <summary>
        /// 更新路径每一步的坐标和场景名字
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovementStep"></param>
        private void UpdatePathOnMovementStepStack(string sceneName,Stack<MoveMentStep> npcMovementStep)
        {
            Node nextNode = endNode;

            while (nextNode!=null)
            {
                MoveMentStep newStep = new MoveMentStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x+originX,nextNode.gridPosition.y+originY);
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}