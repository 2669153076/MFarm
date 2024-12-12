using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MFarm.Save{
    public class SaveLoadMgr : Singleton<SaveLoadMgr>
    {
        public List<ISaveable> saveableList = new List<ISaveable>();

        public List<DataSlot> dataSlotList = new List<DataSlot>(new DataSlot[3]);

        private string jsonFolder;
        private int currentDataIndex;

        protected override void Awake()
        {
            base.Awake();
            jsonFolder = Application.persistentDataPath + "/SAVEDATA/";
            ReadSaveData();
        }
        private void OnEnable()
        {
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                Save(currentDataIndex);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                Load(currentDataIndex);
            }
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if(!saveableList.Contains(saveable))
            {
                saveableList.Add(saveable);
            }
        }

        private void ReadSaveData()
        {
            if(Directory.Exists(jsonFolder))
            {
                for(int i = 0; i < dataSlotList.Count; i++)
                {
                    var resultPath = jsonFolder + "data" + i + ".sav";
                    if(File.Exists(resultPath))
                    {
                        var stringData = File.ReadAllText(resultPath);
                        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                        dataSlotList[i] = jsonData;
                    }

                }
            }
        }

        public void Save(int index)
        {
            DataSlot data = new DataSlot();

            foreach(ISaveable s in saveableList)
            {
                data.gameDataDic.Add(s.GUID,s.GenerateSaveData());
            }
            dataSlotList[index] = data;

            var resultPath = jsonFolder + "data" + index + ".sav";
            var jsonData = JsonConvert.SerializeObject(dataSlotList[index], Formatting.Indented);

            if(!File.Exists(resultPath))
            {
                Directory.CreateDirectory(jsonFolder);
            }
            File.WriteAllText(resultPath, jsonData);
        }

        public void Load(int index)
        {
            this.currentDataIndex = index;

            var resultPath = jsonFolder + "data" + index + ".sav";
            var stringData = File.ReadAllText(resultPath);
            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

            foreach(ISaveable s in saveableList)
            {
                s.RestoreData(jsonData.gameDataDic[s.GUID]);
            }
        }



        private void OnStartNewGameEvent(int index)
        {
            this.currentDataIndex = index;
        }

        private void OnEndGameEvent()
        {
            Save(currentDataIndex);
        }
    }
}