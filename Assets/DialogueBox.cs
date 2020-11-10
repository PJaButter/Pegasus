using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Text))]
public class DialogueBox : MonoBehaviour
{
    private Text m_pTextComponent;

    private string[] m_ary_strDialogueStrings;

    public float SecondsBetweenCharacters;
    public float CharacterRateMultiplier;

    public string DialogueInput;

    private bool m_bIsStringBeingRevealed;
    private bool m_bIsDialoguePlaying;
    private bool m_bIsEndOfDialogue;

    public GameObject ContinueIcon;
    public GameObject StopIcon;

    // Use this for initialization
    void Start()
    {
        m_pTextComponent = GetComponent<Text>();
        m_pTextComponent.text = "";

        HideIcons();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator PresentDialogue(string[] ary_strDialogueStrings, Action callback)
    {
        Debug.Assert(!m_bIsDialoguePlaying, "Can't call PresentText if the DialogueBox is already presenting text!");
        m_bIsDialoguePlaying = true;

        int dialogueLength = ary_strDialogueStrings.Length;
        int currentDialogueIndex = 0;

        while (currentDialogueIndex < dialogueLength)
        {
            if (!m_bIsStringBeingRevealed)
            {
                m_bIsStringBeingRevealed = true;
                if (currentDialogueIndex + 1 >= dialogueLength)
                {
                    m_bIsEndOfDialogue = true;
                }
                yield return StartCoroutine(DisplayString(ary_strDialogueStrings[currentDialogueIndex]));

                currentDialogueIndex++;
            }

            yield return null;
        }

        HideIcons();
        m_bIsEndOfDialogue = false;
        m_bIsDialoguePlaying = false;

        callback();
    }

    private IEnumerator DisplayString(string stringToDisplay)
    {
        int stringLength = stringToDisplay.Length;
        int currentCharacterIndex = 0;

        HideIcons();

        m_pTextComponent.text = "";

        while (currentCharacterIndex < stringLength)
        {
            m_pTextComponent.text += stringToDisplay[currentCharacterIndex];
            currentCharacterIndex++;

            if (currentCharacterIndex < stringLength)
            {
                if (Input.GetButton(DialogueInput))
                {
                    yield return new WaitForSeconds(SecondsBetweenCharacters * CharacterRateMultiplier);
                }
                else
                {
                    yield return new WaitForSeconds(SecondsBetweenCharacters);
                }
            }
            else
            {
                break;
            }
        }

        ShowIcon();

        while (true)
        {
            if (Input.GetButtonDown(DialogueInput))
            {
                break;
            }

            yield return null;
        }

        HideIcons();

        m_bIsStringBeingRevealed = false;
        m_pTextComponent.text = "";
    }

    private void HideIcons()
    {
        ContinueIcon.SetActive(false);
        StopIcon.SetActive(false);
    }

    private void ShowIcon()
    {
        if (m_bIsEndOfDialogue)
        {
            StopIcon.SetActive(true);
            return;
        }

        ContinueIcon.SetActive(true);
    }
}