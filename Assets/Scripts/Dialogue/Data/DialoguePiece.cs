using UnityEngine;
using UnityEngine.Events;

namespace Dialogue
{
    /// <summary>
    /// 对话数据
    /// </summary>
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("对话详情")]
        public Sprite faceImage;    //人物头像
        public bool onLeft; //是否是左边的对话
        public string name; //人物名字

        [TextArea]
        public string dialogueText; //对话
        public bool hasToPause; //是否继续
        [HideInInspector]public bool isDone; //对话是否结束
        //public UnityEvent afterTalkEvent;   //对话结束事件
    }
}