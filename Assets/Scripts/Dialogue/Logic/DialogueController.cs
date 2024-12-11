using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MFarm.Dialogue
{
    /// <summary>
    /// 对话控制器
    /// 挂载在NPC身上
    /// </summary>
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        public UnityEvent OnFinishEvent;    //对话完成事件

        private NPCMovement NPC=>GetComponent<NPCMovement>();
        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        public Stack<DialoguePiece> dialogueStack;

        private bool canTalk;   //是否可以对话
        private GameObject uiSign; //对话提示UI
        private bool isTalking; //是否正在对话

        private void Awake()
        {
            uiSign = transform.GetChild(1).gameObject;
            FillDialogueStack();
        }

        private void Update()
        {
            uiSign.SetActive(canTalk);
            if(canTalk&&Input.GetKeyDown(KeyCode.Space)&&!isTalking) {
                StartCoroutine(DialogueRoutine());
            }
        }

        /// <summary>
        /// 填充栈
        /// </summary>
        private void FillDialogueStack()
        {
            dialogueStack = new Stack<DialoguePiece>();
            for(int i = dialogueList.Count - 1; i >= 0; i--)
            {
                dialogueList[i].isDone = false;
                dialogueStack.Push(dialogueList[i]);
            }
        }
        /// <summary>
        /// 开启对话
        /// </summary>
        /// <returns></returns>
        private IEnumerator DialogueRoutine()
        {
            isTalking = true;
            if(dialogueStack.TryPop(out DialoguePiece result))
            {
                //传给UI显示对话
                EventHandler.CallShowDialogueEvent(result);
                EventHandler.CallUpdateGameStateEvent(E_GameState.Pause);
                yield return new WaitUntil(() => result.isDone);
                isTalking = false;
            }
            else
            {
                EventHandler.CallUpdateGameStateEvent(E_GameState.Playing);
                EventHandler.CallShowDialogueEvent(null);
                FillDialogueStack();
                isTalking = false;

                OnFinishEvent?.Invoke();
                canTalk = false;
            }

        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = !NPC.isMoving && NPC.interactable;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = false;
            }
        }
    }
}