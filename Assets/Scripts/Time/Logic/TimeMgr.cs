using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameTime
{
    /// <summary>
    /// 时间管理类
    /// </summary>
    public class TimeMgr : Singleton<TimeMgr>
    {
        //秒、分、时、天、月、年
        private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
        private E_Season gameSeason = E_Season.Spring;

        private int monthInSeason = 3;  //一个季节几个月
        public bool gameClockPause; //是否暂停
        private float tikTimer; //计时器

        public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

        protected override void Awake()
        {
            base.Awake();
            InitGameTime();
        }
        private void OnEnable()
        {
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }
        private void OnDisable()
        {
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }
        private void Start()
        {
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
        }

        private void Update()
        {
            if (!gameClockPause)
            {
                tikTimer += Time.deltaTime;
                if (tikTimer >= Settings.secondThreshold)  //当累积到这个点时为1秒
                {
                    tikTimer -= Settings.secondThreshold;
                    UpdateGameTime();
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                for (int i = 0; i < 60; i++)
                {
                    UpdateGameTime();
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                gameDay++;
                EventHandler.CallGameDayEvent(gameDay, gameSeason);
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
        }

        /// <summary>
        /// 初始化时间
        /// </summary>
        private void InitGameTime()
        {
            gameSecond = 0;
            gameMinute = 0;
            gameHour = 8;
            gameDay = 28;
            gameMonth = 11;
            gameYear = 2024;
            gameSeason = E_Season.Spring;
            UpdateGameTime();
        }

        /// <summary>
        /// 更新游戏时间
        /// </summary>
        private void UpdateGameTime()
        {
            gameSecond++;
            if (gameSecond > Settings.secondHold)
            {
                gameMinute++;
                gameSecond = 0;
                if (gameMinute > Settings.minuteHold)
                {
                    gameHour++;
                    gameMinute = 0;
                    if (gameHour > Settings.hourHold)
                    {
                        gameDay++;
                        gameHour = 0;
                        if (gameDay > Settings.dayHold)
                        {
                            gameMonth++;
                            gameDay = 1;
                            if (gameMonth > Settings.monthHold)
                            {
                                gameMonth = 1;
                            }
                            monthInSeason--;
                            if (monthInSeason == 0)
                            {
                                monthInSeason = 3;
                                int seasonNumber = (int)gameSeason;
                                seasonNumber++;
                                if (seasonNumber > Settings.seasonHold)
                                {
                                    seasonNumber = 1;
                                    gameYear++;
                                }
                                gameSeason = (E_Season)seasonNumber;

                                if (gameYear > 9999)
                                {
                                    gameYear = 2024;
                                }
                            }
                        }
                            //刷新地图和农作物生长
                            EventHandler.CallGameDayEvent(gameDay, gameSeason); //每一天的变化
                    }
                    EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason); //小时变化带动日期变化
                }
                EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason); //分钟变化带动小时变化
            }
        }



        private void OnBeforeSceneUnloadEvent()
        {
            gameClockPause = true;
        }

        private void OnAfterSceneLoadEvent()
        {
            gameClockPause = false;
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        }


    }
}