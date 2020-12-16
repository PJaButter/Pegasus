using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;

    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject actionsPanel;
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private GameObject moveDetailsPanel;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> attackTexts;

    [SerializeField] private Text moveDescriptionText;
    [SerializeField] private Text moveUsesText;
    [SerializeField] private Text typeText;

    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public IEnumerator TypeDialogue(string dialog)
    {
        dialogueText.text = "";
        foreach (char letter in dialog.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1.0f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1.0f);
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionsPanel(bool enabled)
    {
        actionsPanel.SetActive(enabled);
    }

    public void EnableAttackPanel(bool enabled)
    {
        attackPanel.SetActive(enabled);
        moveDetailsPanel.SetActive(enabled);
    }
}
