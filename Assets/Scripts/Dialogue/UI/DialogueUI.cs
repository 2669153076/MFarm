using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    /// <summary>
    /// 对话UI
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        public GameObject dialoguePanel;  //对话框
        public Text dialogueText; //对话文字
        public Image faceLeftImage, faceRightImage; //头像
        public Text nameLeftText, nameRightText;    //名字
        public GameObject continueBox;  //继续对话提示框

        private void Awake()
        {

        }

        private void OnEnable()
        {
            EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
        }

        private void OnDisable()
        {
            EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
        }

        // Start is called before the first frame update
        void Start()
        {
            dialoguePanel.SetActive(false);
            continueBox.SetActive(false);
        }



        private void OnShowDialogueEvent(DialoguePiece piece)
        {
            StartCoroutine(ShowDialogue(piece));
        }

        private IEnumerator ShowDialogue(DialoguePiece dialoguePiece)
        {
            if (dialoguePiece != null)
            {
                dialoguePiece.isDone = false;
                dialoguePanel.SetActive(true);
                continueBox.SetActive(false);

                dialogueText.text = string.Empty;

                if (dialoguePiece.name != string.Empty)
                {
                    if (dialoguePiece.onLeft)
                    {
                        faceLeftImage.gameObject.SetActive(true);
                        faceRightImage.gameObject.SetActive(false);
                        faceLeftImage.sprite = dialoguePiece.faceImage;
                        nameLeftText.text = dialoguePiece.name;
                    }
                    else
                    {
                        faceLeftImage.gameObject.SetActive(false);
                        faceRightImage.gameObject.SetActive(true);
                        faceRightImage.sprite = dialoguePiece.faceImage;
                        nameRightText.text = dialoguePiece.name;
                    }
                }
                else
                {
                    faceLeftImage.gameObject.SetActive(false);
                    faceRightImage.gameObject.SetActive(false);
                }

                yield return dialogueText.DOText(dialoguePiece.dialogueText,1f).WaitForCompletion();

                dialoguePiece.isDone = true;

                if (dialoguePiece.hasToPause && dialoguePiece.isDone)
                {
                    continueBox.SetActive(true);
                }
                else
                {
                    continueBox.SetActive(false);
                }
            }
            else
            {
                dialoguePanel.SetActive(false);
                yield break;
            }
        }
    }
}