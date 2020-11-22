using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private static BattleManager instance = null;
    public static BattleManager Get { get { return instance; } }

    private bool inBattle;
    private bool canFlee;

    public DialogueBox mainBattleDialogueBox;

    public GameObject actionsPanel;
    public Button attackButton;
    public Button evolutionsButton;
    public Button bagButton;
    public Button monstersButton;
    public Button fleeButton;
    public string[] FleeingText;
    public string[] FailedFleeText;

    public GameObject attackPanel;
    public Button attackBackButton;
    public Button attackButton1;
    public Button attackButton2;
    public Button attackButton3;
    public Button attackButton4;
    public Button attackButton5;

    public bool InBattle { get { return inBattle; } set { inBattle = value; } }

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnterBattle()
    {
        GameManager.Get.WorldCamera.gameObject.SetActive(false);
        GameManager.Get.BattleCamera.gameObject.SetActive(true);
        inBattle = true;

        // Set info from battle data
        canFlee = true;
        
        SetActionsButtonsInteractable(true);
    }

    public void ExitBattle()
    {
        SetActionsButtonsInteractable(false);
        GameManager.Get.WorldCamera.gameObject.SetActive(true);
        GameManager.Get.BattleCamera.gameObject.SetActive(false);
        inBattle = false;
    }

    public void OnClick_AttackAction()
    {
        actionsPanel.SetActive(false);
        attackPanel.SetActive(true);
    }

    public void OnClick_AttackBackButton()
    {
        actionsPanel.SetActive(true);
        attackPanel.SetActive(false);
    }

    public void TryFleeBattle()
    {
        if (canFlee)
        {
            SetActionsButtonsInteractable(false);
            StartCoroutine(mainBattleDialogueBox.PresentDialogue(FleeingText, this.ExitBattle));
        }
        else
        {
            SetActionsButtonsInteractable(false);
            StartCoroutine(mainBattleDialogueBox.PresentDialogue(FailedFleeText, this.FailedToFlee));
        }
    }

    private void FailedToFlee()
    {
        SetActionsButtonsInteractable(true);
    }

    public void SetActionsButtonsInteractable(bool interactable)
    {
        attackButton.interactable = interactable;
        evolutionsButton.interactable = interactable;
        bagButton.interactable = interactable;
        monstersButton.interactable = interactable;
        fleeButton.interactable = interactable;
    }

    public void SetAttackButtonsInteractable(bool interactable)
    {
        attackBackButton.interactable = interactable;
        attackButton1.interactable = interactable;
        attackButton2.interactable = interactable;
        attackButton3.interactable = interactable;
        attackButton4.interactable = interactable;
        attackButton5.interactable = interactable;
    }
}
