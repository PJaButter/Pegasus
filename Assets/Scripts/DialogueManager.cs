using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance = null;
    public static DialogueManager Get { get { return instance; } }

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Text dialogueText;
    [SerializeField] private int lettersPerSecond;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    private Dialogue dialogue;
    private int currentLine = 0;
    private bool isTyping;

    public bool IsShowing { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void HandleUpdate()
    {
        if (Input.anyKeyDown && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialogue.Lines.Count)
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogueBox.SetActive(false);

                OnCloseDialogue?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialogue?.Invoke();

        IsShowing = true;
        this.dialogue = dialogue;
        dialogueBox.SetActive(true);
        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }

    public IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1.0f / lettersPerSecond);
        }
        isTyping = false;
    }
}
