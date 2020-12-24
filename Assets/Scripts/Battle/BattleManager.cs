using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleManager : MonoBehaviour
{
    private static BattleManager instance = null;
    public static BattleManager Get { get { return instance; } }

    private bool inBattle;
    private bool canFlee;

    private BattleState state;

    [SerializeField] private Camera battleCamera;
    [SerializeField] private DialogueBox mainBattleDialogueBox;
    [SerializeField] private BattleDialogueBox battleDialogueBox;

    [SerializeField] private GameObject actionsPanel;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button evolutionsButton;
    [SerializeField] private Button bagButton;
    [SerializeField] private Button monstersButton;
    [SerializeField] private Button fleeButton;
    [SerializeField] private string[] FleeingText;
    [SerializeField] private string[] FailedFleeText;

    [SerializeField] private GameObject moveDetailsPanel;
    [SerializeField] private Text moveDescriptionText;
    [SerializeField] private Text moveEnergyCostText;
    [SerializeField] private Text moveTypeText;

    [SerializeField] private GameObject attackPanel;
    [SerializeField] private Button attackBackButton;
    [SerializeField] private AttackButton basicAttackButton;
    [SerializeField] private List<AttackButton> attackButtons;

    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private PartyScreen partyScreen;

    private MonsterParty playerParty;
    private WildMonster wildMonster;

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

    public IEnumerator EnterBattle(MonsterParty playerParty, WildMonster wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;

        inBattle = true;

        // Set info from battle data
        canFlee = true;

        playerUnit.Setup(playerParty.GetHealthyMonster());
        enemyUnit.Setup(wildMonster.Monster);

        partyScreen.Init();

        battleDialogueBox.SetDialogue("");

        Coroutine playerEnterAnimationCor = StartCoroutine(playerUnit.PlayEnterAnimation());
        yield return enemyUnit.PlayEnterAnimation();
        yield return playerEnterAnimationCor;

        SetupAttackButtons(playerUnit.Monster.BasicMove, playerUnit.Monster.Moves);

        yield return battleDialogueBox.TypeDialogue($"A wild { enemyUnit.Monster.MonsterBase.Name } appeared.");

        ActionSelection();

        SetActionsButtonsInteractable(true);
    }

    public void ExitBattle()
    {
        SetActionsButtonsInteractable(false);
        inBattle = false;
    }

    public void OnClick_AttackAction()
    {
        MoveSelection();
        mainBattleDialogueBox.gameObject.SetActive(false);
        actionsPanel.SetActive(false);
        moveDetailsPanel.SetActive(true);
        attackPanel.SetActive(true);
    }

    public void OnClick_AttackBackButton()
    {
        ActionSelection();
        mainBattleDialogueBox.gameObject.SetActive(true);
        actionsPanel.SetActive(true);
        moveDetailsPanel.SetActive(false);
        attackPanel.SetActive(false);
    }

    public void OnClick_AttackButton(Move move)
    {
        StartCoroutine(PlayerMove(move));
        mainBattleDialogueBox.gameObject.SetActive(true);
        battleDialogueBox.EnableDialogueText(true);
        actionsPanel.SetActive(false);
        moveDetailsPanel.SetActive(false);
        attackPanel.SetActive(false);
    }

    public void OnClick_MonstersButton()
    {
        OpenPartyScreen(false);
    }

    public void OnClick_PartyMember(int partyMemberIndex)
    {
        Monster selectedMonster = playerParty.Monsters[partyMemberIndex];
        if (selectedMonster.CurrentHealth <= 0)
        {
            partyScreen.SetMessageText("This monster is currently unable to battle.");
            return;
        }
        else if (selectedMonster == playerUnit.Monster)
        {
            partyScreen.SetMessageText("This monster is already in battle.");
            return;
        }

        partyScreen.gameObject.SetActive(false);
        state = BattleState.Busy;
        battleDialogueBox.SetDialogue("");
        StartCoroutine(SwitchMonster(selectedMonster));
    }

    public void OnClick_PartyScreenBack()
    {
        partyScreen.gameObject.SetActive(false);
        ActionSelection();
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
        basicAttackButton.GetComponent<Button>().interactable = interactable;
        foreach (AttackButton attackButton in attackButtons)
        {
            attackButton.GetComponent<Button>().interactable = interactable;
        }
    }

    public void SetAttackDetails(Move move)
    {
        moveDescriptionText.text = move.MoveBase.Description;
        moveTypeText.text = $"{move.MoveBase.Attribute.ToString()}";

        if (move.MoveBase.EnergyCost == 0)
        {
            moveEnergyCostText.text = $"Basic Move";
        }
        else
        {
            moveEnergyCostText.text = $"Energy Cost: {move.MoveBase.EnergyCost}";
        }
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        battleDialogueBox.SetDialogue("Choose an action");
        battleDialogueBox.EnableActionsPanel(true);
    }

    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        battleDialogueBox.EnableActionsPanel(false);
        battleDialogueBox.EnableDialogueText(false);
        battleDialogueBox.EnableAttackPanel(false);
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        GameManager.Get.EndBattle(won);
    }

    private void OpenPartyScreen(bool mustChooseMonster)
    {
        partyScreen.SetPartyData(playerParty.Monsters, mustChooseMonster);
        partyScreen.gameObject.SetActive(true);
    }

    private void SetupAttackButtons(Move basicMove, List<Move> moves)
    {
        basicAttackButton.SetupButton(basicMove);
        for (int i = 0; i < attackButtons.Count; ++i)
        {
            attackButtons[i].SetupButton(i < moves.Count ? moves[i] : null);
        }
    }

    private IEnumerator PlayerMove(Move move)
    {
        state = BattleState.PerformMove;

        yield return RunMove(playerUnit, enemyUnit, move);

        // If the battle state was not changed by RunMove, proceed to the next step
        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    private IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        Move move = enemyUnit.Monster.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        // If the battle state was not changed by RunMove, proceed to the next step
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        sourceUnit.Monster.UseEnergy(move.MoveBase.EnergyCost);
        yield return sourceUnit.HUD.UpdateEnergy();

        yield return battleDialogueBox.TypeDialogue($"{sourceUnit.Monster.MonsterBase.Name} used {move.MoveBase.Name}");

        yield return sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1.0f);

        yield return targetUnit.PlayHitAnimation();
        DamageDetails damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
        yield return targetUnit.HUD.UpdateHealth();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return battleDialogueBox.TypeDialogue($"{targetUnit.Monster.MonsterBase.Name} Died");
            yield return targetUnit.PlayDeathAnimation();

            yield return new WaitForSeconds(2.0f);
            HandleMonsterFainted(targetUnit);
        }
    }

    private void HandleMonsterFainted(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            Monster nextMonster = playerParty.GetHealthyMonster();
            if (nextMonster != null)
            {
                OpenPartyScreen(true);
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            wildMonster.Defeated();
            BattleOver(true);
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.CriticalModifier > 1.0f)
        {
            yield return battleDialogueBox.TypeDialogue("A critical hit!");
        }

        if (damageDetails.AttributeModifier > 1.0f)
        {
            yield return battleDialogueBox.TypeDialogue("It's super effective!");
        }
        else if (damageDetails.AttributeModifier < 1.0f)
        {
            yield return battleDialogueBox.TypeDialogue("It's not very effective!");
        }
    }

    private IEnumerator SwitchMonster(Monster newMonster)
    {
        if (playerUnit.Monster.CurrentHealth > 0)
        {
            yield return battleDialogueBox.TypeDialogue($"Take a break { playerUnit.Monster.MonsterBase.Name }");
            yield return playerUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(1.0f);
        }

        playerUnit.Setup(newMonster);

        yield return playerUnit.PlayEnterAnimation();

        SetupAttackButtons(newMonster.BasicMove, newMonster.Moves);

        yield return battleDialogueBox.TypeDialogue($"Go { newMonster.MonsterBase.Name }!");

        StartCoroutine(EnemyMove());
    }
}
