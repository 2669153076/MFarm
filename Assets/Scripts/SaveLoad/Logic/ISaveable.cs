using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save{
    public interface ISaveable
    {
        string GUID {  get; }   

        void RegisterSaveable()
        {
            SaveLoadMgr.Instance.RegisterSaveable(this);
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        /// <returns></returns>
        GameSaveData GenerateSaveData();

        /// <summary>
        /// 恢复数据
        /// </summary>
        /// <param name="data"></param>
        void RestoreData(GameSaveData data);
    }
}