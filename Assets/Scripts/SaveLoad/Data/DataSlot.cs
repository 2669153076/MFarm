using MFarm.GameTime;
using MFarm.Transition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save{
    public class DataSlot
    {
        public Dictionary<string, GameSaveData> gameDataDic = new Dictionary<string, GameSaveData>(); //GUID、游戏数据


        public string DataTime
        {
            get
            {
                var key = TimeMgr.Instance.GUID;
                if (gameDataDic.ContainsKey(key))
                {
                    var timeData = gameDataDic[key];
                    return timeData.timeDic["gameYear"] + "年/" + timeData.timeDic["gameSeason"] + "/" + timeData.timeDic["gameMonth"] + "月/" + timeData.timeDic["gameDay"] + "日/" + timeData.timeDic["gameHour"] + "时/" + timeData.timeDic["gameMinute"] + "分/" + timeData.timeDic["gameSecond"] + "秒";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionMgr.Instance.GUID;
                if (gameDataDic.ContainsKey(key))
                {
                    var transitionData = gameDataDic[key];

                    return transitionData.dataSceneName switch
                    {
                        "00.Start" => "海边",
                        "01.Field" => "农场",
                        "02.Home" => "小木屋",
                        "03.Stall" => "市场",
                        _ => string.Empty
                    };
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}