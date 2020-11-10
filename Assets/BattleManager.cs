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

    public Button attackButton;
    public Button evolutionsButton;
    public Button bagButton;
    public Button monstersButton;
    public Button fleeButton;
    public string[] FleeText;

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
        canFlee = false;
        
        SetButtonsInteractable(true);
    }

    public void ExitBattle()
    {
        SetButtonsInteractable(false);
        GameManager.Get.WorldCamera.gameObject.SetActive(true);
        GameManager.Get.BattleCamera.gameObject.SetActive(false);
        inBattle = false;
    }

    public void TryFleeBattle()
    {
        if (canFlee)
        {
            ExitBattle();
        }
        else
        {
            SetButtonsInteractable(false);
            StartCoroutine(mainBattleDialogueBox.PresentDialogue(FleeText, this.FailedToFlee));
        }
    }

    private void FailedToFlee()
    {
        SetButtonsInteractable(true);
    }

    public void SetButtonsInteractable(bool interactable)
    {
        attackButton.interactable = interactable;
        evolutionsButton.interactable = interactable;
        bagButton.interactable = interactable;
        monstersButton.interactable = interactable;
        fleeButton.interactable = interactable;
    }
}
