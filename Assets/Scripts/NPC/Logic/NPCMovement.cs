using DG.Tweening;
using MFarm.AStarAlgorithm;
using MFarm.GameTime;
using MFarm.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// NPC移动
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour, ISaveable
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentScheduleDetails;

    [SerializeField]
    private string currentScene;    //初始所在场景
    private string targetScene;     //要移动的场景
    private Vector3Int currentGridPosition; //当前所处位置网格坐标
    private Vector3Int targetGridPosition;      //最终目标所处位置网格坐标
    private Vector3Int nextGridPosition;    //下一步要到达的位置网格坐标
    private Vector3 nextWorldPosition;  //下一步要到达的位置世界坐标

    public string StartScene { set => currentScene = value; }   //属于哪个场景
    [Header("移动属性")]
    public float normalSpeed = 2f;
    private float minSpeed = 1;
    private float maxSpeed = 3;
    private Vector2 dir;
    public bool isMoving;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Animator animator;

    private Stack<MoveMentStep> movementStepStack;  //移动路径
    private Grid currentGrid;

    private bool isInitialised; //是否已经初始化过
    private bool npcMove;   //npc是否移动
    private bool sceneLoaded;   //场景已经是否加载
    public bool interactable;   //是否可以互动
    public bool isFirstLoad;    //是否第一次加载NPC
    private E_Season currentSeason; //当前季节

    private float animationBreakTimer;  //动画间隔计时器
    private bool canPlayStopAnimation;  //能否播放停止动画
    private AnimationClip stopAnimationClip; //是否停止动画
    public AnimationClip blankAnimationClip;    //空动画切片
    private AnimatorOverrideController animatorOverride;

    private Coroutine npcMoveRoutine;   //移动协程

    private TimeSpan GameTime => TimeMgr.Instance.GameTime;

    public string GUID => GetComponent<DataGUID>().guid;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        movementStepStack = new Stack<MoveMentStep>();

        animatorOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverride;

        scheduleSet = new SortedSet<ScheduleDetails>();
        foreach (var schedule in scheduleData.scheduleDetailList)
        {
            scheduleSet.Add(schedule);
        }

    }
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;

    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }


    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void Update()
    {
        if (sceneLoaded)
        {
            SwitchAnimation();
        }
        animationBreakTimer -= Time.deltaTime;
        canPlayStopAnimation = animationBreakTimer <= 0;
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            Movement();
        }
    }


    /// <summary>
    /// 检查场景并显示对应NPC
    /// </summary>
    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
        {
            SetActionInScene();
        }
        else
        {
            SetInactiveInScenc();
        }
    }
    /// <summary>
    /// 初始化NPC
    /// </summary>
    private void InitNPC()
    {
        targetScene = currentScene;

        //保持在当前坐标的网格中心点
        currentGridPosition = currentGrid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        targetGridPosition = currentGridPosition;

    }
    #region 移动相关
    /// <summary>
    /// NPC移动
    /// </summary>
    private void Movement()
    {

        if (!npcMove)
        {
            //要移动到指定点的栈
            if (movementStepStack.Count > 0)
            {
                MoveMentStep step = movementStepStack.Pop();

                currentScene = step.sceneName;

                CheckVisiable();

                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(nextGridPosition, stepTime);
            }
            else if (!isMoving && canPlayStopAnimation)
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }
    /// <summary>
    /// 移动到下一个格子
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="stepTime"></param>
    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMoveRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPos, stepTime));
    }
    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPosition(gridPos);
        //计算下一步是否有时间走
        if (stepTime > GameTime)
        {
            //用来移动的时间差
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //实际移动距离
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            //实际移动速度
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));
            if (speed <= maxSpeed)
            {
                //当前位置与下一步位置的距离大于设置像素 循环
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;  //移动方向
                    //位置偏移
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);   //移动的位置为自身位置+位置偏移
                    yield return new WaitForFixedUpdate();  //等待下一次物理更新
                }
            }
        }
        //如果时间已经到了就瞬移
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;

        npcMove = false;
    }

    /// <summary>
    /// 构建行动路径
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(ScheduleDetails schedule)
    {
        movementStepStack.Clear();
        currentScheduleDetails = schedule;
        targetGridPosition = (Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;
        targetScene = schedule.targetScene;

        this.interactable = schedule.interactable;
        

        if (schedule.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementStepStack);
        }
        //跨场景移动
        else if(schedule.targetScene!=currentScene)
        {
            SceneRoute sceneRoute = NPCMgr.Instance.GetSceneRoute(currentScene, schedule.targetScene);

            if(sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, toPos;
                    ScenePath scenePath = sceneRoute.scenePathList[i];
                    

                    if(scenePath.fromGridCell.x >= Settings.maxGridSize|| scenePath.fromGridCell.y >= Settings.maxGridSize)
                    {
                        //来的地方是当前场景中当前坐标
                        fromPos = (Vector2Int)currentGridPosition;
                    }
                    else
                    {
                        //另一个场景中，角色生成的对应位置
                        fromPos = scenePath.fromGridCell;
                    }
                    if (scenePath.toGridCell.x >= Settings.maxGridSize|| scenePath.toGridCell.y >= Settings.maxGridSize)
                    {
                        //当前场景的目标点 使用scheduledataList_So中配置的目标
                        toPos = schedule.targetGridPosition;
                    }
                    else
                    {
                        //跨场景的目标点  使用sceneroutedatalist_so中配置的目标点
                        toPos = scenePath.toGridCell;
                    }

                    AStar.Instance.BuildPath(scenePath.sceneName, fromPos, toPos, movementStepStack);
                }
            }
        }

        if (movementStepStack.Count > 0)
        {
            //更新每一步对应的时间戳
            UpdateTimeOnPath();
        }
    }
    /// <summary>
    /// 更新时间戳
    /// </summary>
    private void UpdateTimeOnPath()
    {
        MoveMentStep previousStep = null;

        TimeSpan curremtGameTime = GameTime;

        foreach (var step in movementStepStack)
        {
            if (previousStep == null)
            {
                previousStep = step;
            }

            step.hour = curremtGameTime.Hours;
            step.minute = curremtGameTime.Minutes;
            step.second = curremtGameTime.Seconds;

            TimeSpan gridMovementStepTime;  //走过每一个格子所需要的时间长度
            if (MoveInDiagonal(previousStep, step))
            {
                //斜向格子时间
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            }
            else
            {
                //直线格子时间
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));
            }
            curremtGameTime = curremtGameTime.Add(gridMovementStepTime);

            //循环下一步
            previousStep = step;
        }
    }
    /// <summary>
    /// 是否走斜线方向
    /// </summary>
    /// <param name="previousStep">上一步</param>
    /// <param name="currentStep">当前步</param>
    /// <returns>
    /// true 斜走<br/>
    /// false 直走
    /// </returns>
    private bool MoveInDiagonal(MoveMentStep previousStep, MoveMentStep currentStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    #endregion
    /// <summary>
    /// 获取世界坐标中心点
    /// 将网格坐标转换为世界坐标中心点
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = currentGrid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2, 0);
    }
    /// <summary>
    /// 切换动画
    /// </summary>
    private void SwitchAnimation()
    {
        isMoving = transform.position != GetWorldPosition(targetGridPosition);
        animator.SetBool("IsMoving", isMoving);
        if (isMoving)
        {
            animator.SetBool("Exit", true);
            animator.SetFloat("DirX", dir.x);
            animator.SetFloat("DirY", dir.y);
        }
        else
        {
            animator.SetBool("Exit", false);
        }
    }
    private IEnumerator SetStopAnimation()
    {
        //强制面向镜头
        animator.SetFloat("DirX", 0);
        animator.SetFloat("DirY", -1);

        animationBreakTimer = Settings.animationBreakTime;
        if (stopAnimationClip != null)
        {
            animatorOverride[blankAnimationClip] = stopAnimationClip;
            animator.SetBool("EventAnimation", true);
            yield return null;
            animator.SetBool("EventAnimation", false);
        }
        else
        {
            animatorOverride[stopAnimationClip] = blankAnimationClip;
            animator.SetBool("EventAnimation", false);
        }

    }

    private void SetActionInScene()
    {
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;

        transform.GetChild(0).gameObject.SetActive(true);
    }
    private void SetInactiveInScenc()
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);  //影子关闭
    }

    #region 事件相关
    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        CheckVisiable();
        currentGrid = FindObjectOfType<Grid>();
        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }
        sceneLoaded = true;

        if(!isFirstLoad)
        {
            currentGridPosition = currentGrid.WorldToCell(transform.position);
            var schedule = new ScheduleDetails(0, 0, 0, 0, currentSeason, targetScene, (Vector2Int)targetGridPosition, stopAnimationClip, interactable);
            BuildPath(schedule);
            isFirstLoad = true;
        }
    }
    private void OnGameMinuteEvent(int minute, int hour,int day , E_Season season)
    {
        int time = (hour * 100) + minute;
        currentSeason = season;
        
        ScheduleDetails matchSchedule = null;
        foreach (var schedule in scheduleSet)
        {
            if (schedule.Time == time)
            {
                if (schedule.season != season)
                {
                    continue;
                }
                if (schedule.day != 0 && schedule.day != day)
                {
                    continue;
                }
                matchSchedule = schedule;
            }
            else if (schedule.Time > time)
            {
                break;
            }
        }
        if(matchSchedule != null)
        {
            BuildPath(matchSchedule);
        }
    }

    private void OnEndGameEvent()
    {
        sceneLoaded = false;
        npcMove = false;
        if (npcMoveRoutine != null)
        {
            StopCoroutine(npcMoveRoutine);
        }
    }
    private void OnStartNewGameEvent(int obj)
    {
        isInitialised = false;
        isFirstLoad = true;
    }

    #endregion

    public GameSaveData GenerateSaveData()
    {
        GameSaveData data = new GameSaveData();
        data.characterPosDic = new Dictionary<string, SerializableVector3>();
        data.characterPosDic.Add("targetGridPosition", new SerializableVector3(targetGridPosition));
        data.characterPosDic.Add("currentPosition", new SerializableVector3(transform.position));
        data.dataSceneName = currentScene;
        data.targetScene = this.targetScene;
        if (stopAnimationClip != null)
        {
            data.animationInstanceId = stopAnimationClip.GetInstanceID();
        }
        data.interactable = this.interactable;

        data.timeDic = new Dictionary<string, int>();
        data.timeDic.Add("currentSeason", (int)currentSeason);
        return data;
    }

    public void RestoreData(GameSaveData data)
    {
        this.isInitialised = true;
        this.isFirstLoad = false;

        currentScene = data.dataSceneName;
        targetScene = data.targetScene;

        Vector3 pos = data.characterPosDic["currentPosition"].ToVector3();
        Vector3Int gridPos = (Vector3Int)data.characterPosDic["targetGridPosition"].ToVector2Int();

        transform.position = pos;
        targetGridPosition = gridPos;



        if (data.animationInstanceId != 0)
        {
            this.stopAnimationClip = Resources.InstanceIDToObject(data.animationInstanceId) as AnimationClip;
        }
        this.interactable = data.interactable;
        this.currentSeason = (E_Season)data.timeDic["currentSeason"];
    }
}
