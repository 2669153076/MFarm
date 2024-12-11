using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace MFarm.AStarAlgorithm{
    /// <summary>
    /// A星寻路测试
    /// </summary>
    public class AStarTest : MonoBehaviour
    {
        private AStar aStar;
        public Vector2Int startPos;
        public Vector2Int endPos;

        public Tilemap displayMap;
        public TileBase displayTile;

        public bool displayStartAndFinish;
        public bool displayPath;

        private Stack<MoveMentStep> npcMoveMentStepStack = new Stack<MoveMentStep>();


        [Space]
        [Header("测试移动NPC")]
        public NPCMovement npcMovement;
        public bool moveNPC;
        [SceneName] public string targetSceneName;
        public Vector2Int targetPos;
        public AnimationClip stopClip;


        private void Awake()
        {
            aStar = GetComponent<AStar>();
        }
        private void Update()
        {
            ShowPathOnGripMap();
            if(moveNPC)
            {
                moveNPC = false;
                var schedule = new ScheduleDetails(0, 0, 0, 0, E_Season.Spring, targetSceneName, targetPos, stopClip, true);
                npcMovement.BuildPath(schedule);
            }

        }
        private void ShowPathOnGripMap()
        {
            if(displayMap != null&&displayTile!=null)
            {
                if(displayStartAndFinish)
                {
                    displayMap.SetTile((Vector3Int)startPos, displayTile);
                    displayMap.SetTile((Vector3Int)endPos, displayTile);
                }
                else
                {
                    displayMap.SetTile((Vector3Int)startPos, null);
                    displayMap.SetTile((Vector3Int)endPos, null);
                }

                if(displayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    aStar.BuildPath(sceneName, startPos, endPos,npcMoveMentStepStack);

                    foreach (var step in npcMoveMentStepStack)
                    {
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                    npcMoveMentStepStack.Clear();
                }
            }
        }
    }
}