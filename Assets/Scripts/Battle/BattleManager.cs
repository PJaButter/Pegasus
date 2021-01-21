using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, AboutToSummon, BattleOver }
public enum BattleAction { Move, SwitchMonster, UseItem, Run }

public class BattleManager : MonoBehaviour
{
    private static BattleManager instance = null;
    public static BattleManager Get { get { return instance; } }

    private bool canFlee;

    private BattleState state;
    private BattleState? previousState;

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

    [SerializeField] private Image playerImage;
    [SerializeField] private Image enemyTamerImage;

    [SerializeField] private PartyScreen partyScreen;

    private MonsterParty playerParty;
    private MonsterParty enemyTamerParty;

    private PlayerController playerController;
    private TamerController enemyTamerController;

    private WildMonster wildMonster;

    private bool isTamerBattle;
    private bool mustSummonEnemyMonster;

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

    public void StartWildBattle(MonsterParty playerParty, WildMonster wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
        isTamerBattle = false;

        StartCoroutine(SetupBattle());
    }

    public void StartTamerBattle(MonsterParty playerParty, MonsterParty enemyParty)
    {
        this.playerParty = playerParty;
        this.enemyTamerParty = enemyParty;

        isTamerBattle = true;
        playerController = playerParty.GetComponent<PlayerController>();
        enemyTamerController = enemyParty.GetComponent<TamerController>();

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        battleDialogueBox.SetDialogue("");

        if (!isTamerBattle)
        {
            // Wild Monster Battle
            canFlee = true;

            playerUnit.gameObject.SetActive(true);
            enemyUnit.gameObject.SetActive(true);

            playerImage.gameObject.SetActive(false);
            enemyTamerImage.gameObject.SetActive(false);

            playerUnit.Setup(playerParty.GetHealthyMonster());
            enemyUnit.Setup(wildMonster.Monster);

            Coroutine playerEnterAnimationCor = StartCoroutine(playerUnit.PlayEnterAnimation());
            yield return enemyUnit.PlayEnterAnimation();
            yield return playerEnterAnimationCor;

            SetupAttackButtons(playerUnit.Monster.BasicMove, playerUnit.Monster.Moves);
            yield return battleDialogueBox.TypeDialogue($"A wild { enemyUnit.Monster.MonsterBase.Name } appeared.");
        }
        else
        {
            // Tamer Battle
            canFlee = false;

            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);
            playerUnit.HUD.gameObject.SetActive(false);
            enemyUnit.HUD.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            enemyTamerImage.gameObject.SetActive(true);
            playerImage.sprite = playerController.BattleSprite;
            enemyTamerImage.sprite = enemyTamerController.BattleSprite;

            yield return battleDialogueBox.TypeDialogue($"{ enemyTamerController.Name } wants to fight.");

            // Send out enemy's first monster
            enemyTamerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            Monster enemyMonster = enemyTamerParty.GetHealthyMonster();
            enemyUnit.Setup(enemyMonster);
            Coroutine enemyEnterAnimationCor = StartCoroutine(enemyUnit.PlayEnterAnimation());
            yield return battleDialogueBox.TypeDialogue($"{ enemyTamerController.Name } sent out { enemyMonster.MonsterBase.Name }.");
            yield return enemyEnterAnimationCor;

            // Send out player's first monster
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            Monster playerMonster = playerParty.GetHealthyMonster();
            playerUnit.Setup(playerMonster);
            Coroutine playerEnterAnimationCor = StartCoroutine(playerUnit.PlayEnterAnimation());
            yield return battleDialogueBox.TypeDialogue($"Go { playerMonster.MonsterBase.Name }.");
            yield return playerEnterAnimationCor;
            SetupAttackButtons(playerMonster.BasicMove, playerMonster.Moves);
        }

        partyScreen.Init();

        ActionSelection();
        SetActionsButtonsInteractable(true);
    }

    public void ExitBattle()
    {
        SetActionsButtonsInteractable(false);
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
        if (playerUnit.Monster.CurrentEnergy < move.MoveBase.EnergyCost)
            return;

        playerUnit.Monster.CurrentMove = move;
        StartCoroutine(RunTurns(BattleAction.Move));

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

        if (previousState == BattleState.ActionSelection)
        {
            StartCoroutine(RunTurns(BattleAction.SwitchMonster, selectedMonster));
        }
        else
        {
            ChangeState(BattleState.Busy);
            battleDialogueBox.SetDialogue("");
            StartCoroutine(SwitchMonster(selectedMonster));
        }
    }

    public void OnClick_PartyScreenBack()
    {
        partyScreen.gameObject.SetActive(false);
        if (!mustSummonEnemyMonster)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(SendNextTamerMonster());
        }
    }

    public void OnClick_AboutToSummon_Yes()
    {
        battleDialogueBox.EnableChoiceBox(false);
        OpenPartyScreen(false);
    }

    public void OnClick_AboutToSummon_No()
    {
        battleDialogueBox.EnableChoiceBox(false);
        StartCoroutine(SendNextTamerMonster());
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
            if (playerUnit.Monster.CurrentEnergy < move.MoveBase.EnergyCost)
                moveEnergyCostText.color = Color.red;
            else
                moveEnergyCostText.color = Color.black;
        }
    }

    private void ActionSelection()
    {
        ChangeState(BattleState.ActionSelection);
        battleDialogueBox.SetDialogue("Choose an action");
        battleDialogueBox.EnableActionsPanel(true);
    }

    private void MoveSelection()
    {
        ChangeState(BattleState.MoveSelection);
        battleDialogueBox.EnableActionsPanel(false);
        battleDialogueBox.EnableDialogueText(false);
        battleDialogueBox.EnableAttackPanel(false);
    }

    private IEnumerator AboutToSummon(Monster newMonster)
    {
        mustSummonEnemyMonster = true;
        ChangeState(BattleState.Busy);
        yield return battleDialogueBox.TypeDialogue($"{ enemyTamerController.Name } is about to summon { newMonster.MonsterBase.Name }. Do you want to change monsters?");
        ChangeState(BattleState.AboutToSummon);
        battleDialogueBox.EnableChoiceBox(true);
    }

    private void BattleOver(bool won)
    {
        ChangeState(BattleState.BattleOver);
        playerParty.Monsters.ForEach(monster => monster.OnBattleOver());
        GameManager.Get.EndBattle(won);
    }

    private void OpenPartyScreen(bool mustChooseMonster)
    {
        ChangeState(BattleState.PartyScreen);
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

    private IEnumerator RunTurns(BattleAction playerAction, Monster selectedMonster = null)
    {
        ChangeState(BattleState.RunningTurn);

        if (playerAction == BattleAction.Move)
        {
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            // Check who goes first
            int playerMovePriority = playerUnit.Monster.CurrentMove.MoveBase.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.MoveBase.Priority;
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;
            }

            BattleUnit firstBattleUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            BattleUnit secondBattleUnit = (playerGoesFirst) ? enemyUnit : playerUnit;
            Monster secondMonster = secondBattleUnit.Monster;

            // First Turn
            yield return RunMove(firstBattleUnit, secondBattleUnit, firstBattleUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstBattleUnit);
            if (state == BattleState.BattleOver)
                yield break;

            if (secondMonster.CurrentHealth > 0)
            {
                // Second Turn
                yield return RunMove(secondBattleUnit, firstBattleUnit, secondBattleUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondBattleUnit);
                if (state == BattleState.BattleOver)
                    yield break;
            }
        }
        else if (playerAction == BattleAction.SwitchMonster)
        {
            ChangeState(BattleState.Busy);
            StartCoroutine(SwitchMonster(selectedMonster));

            // Enemy Turn
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyUnit.Monster.CurrentMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver)
                yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Monster.OnBeforeMove();
        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.HUD.UpdateHealth();
        if (!canRunMove)
        {
            yield break;
        }

        sourceUnit.Monster.UseEnergy(move.MoveBase.EnergyCost);
        yield return sourceUnit.HUD.UpdateEnergy();

        yield return battleDialogueBox.TypeDialogue($"{sourceUnit.Monster.MonsterBase.Name} used {move.MoveBase.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Monster, targetUnit.Monster))
        {
            yield return sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1.0f);

            yield return targetUnit.PlayHitAnimation();

            if (move.MoveBase.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.MoveBase.Effects, sourceUnit.Monster, targetUnit.Monster, move.MoveBase.Target);
            }
            else
            {
                DamageDetails damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                yield return targetUnit.HUD.UpdateHealth();
                yield return ShowDamageDetails(damageDetails);
            }

            bool targetFainted = targetUnit.Monster.CurrentHealth <= 0;
            if (move.MoveBase.SecondaryEffects != null && move.MoveBase.SecondaryEffects.Count > 0 && !targetFainted)
            {
                foreach (SecondaryEffects secondaryEffect in move.MoveBase.SecondaryEffects)
                {
                    int randomNum = UnityEngine.Random.Range(1, 101);
                    if (randomNum <= secondaryEffect.Chance)
                        yield return RunMoveEffects(secondaryEffect, sourceUnit.Monster, targetUnit.Monster, secondaryEffect.Target);
                }
            }

            if (targetFainted)
            {
                yield return battleDialogueBox.TypeDialogue($"{targetUnit.Monster.MonsterBase.Name} Died");
                yield return targetUnit.PlayDeathAnimation();

                yield return new WaitForSeconds(2.0f);
                HandleMonsterFainted(targetUnit);
            }
        }
        else
        {
            yield return battleDialogueBox.TypeDialogue($"{sourceUnit.Monster.MonsterBase.Name}'s attack missed!");
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects moveEffects, Monster source, Monster target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (moveEffects.StatBoosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyStatBoosts(moveEffects.StatBoosts);
            else
                target.ApplyStatBoosts(moveEffects.StatBoosts);
        }

        // Status Condition
        if (moveEffects.Status != ConditionID.None)
        {
            target.SetStatus(moveEffects.Status);
        }

        // Volatile Status Condition
        if (moveEffects.VolatileStatus != ConditionID.None)
        {
            target.SetVolatileStatus(moveEffects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver)
            yield break;

        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Statuses like burn or poison hurt the monster after the turn
        sourceUnit.Monster.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.HUD.UpdateHealth();
        if (sourceUnit.Monster.CurrentHealth <= 0)
        {
            yield return battleDialogueBox.TypeDialogue($"{sourceUnit.Monster.MonsterBase.Name} Died");
            yield return sourceUnit.PlayDeathAnimation();

            yield return new WaitForSeconds(2.0f);
            HandleMonsterFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    private bool CheckIfMoveHits(Move move, Monster source, Monster target)
    {
        if (move.MoveBase.AlwaysHits)
            return true;

        float accuracy = move.MoveBase.Accuracy;

        int accuracyBoost = source.StatBoosts[Stat.Accuracy];
        int evasionBoost = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1.0f, 4.0f/3.0f, 5.0f/3.0f, 2.0f, 7.0f/3.0f, 8.0f/3.0f, 3.0f };

        if (accuracyBoost > 0)
            accuracy *= boostValues[accuracyBoost];
        else
            accuracy /= boostValues[-accuracyBoost];

        if (evasionBoost > 0)
            accuracy /= boostValues[evasionBoost];
        else
            accuracy *= boostValues[-evasionBoost];

        return UnityEngine.Random.Range(1, 101) <= accuracy;
    }

    private IEnumerator ShowStatusChanges(Monster monster)
    {
        while (monster.StatusChanges.Count > 0)
        {
            string message = monster.StatusChanges.Dequeue();
            yield return battleDialogueBox.TypeDialogue(message);
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
            if (!isTamerBattle)
            {
                wildMonster.Defeated();
                BattleOver(true);
            }
            else
            {
                Monster nextMonster = enemyTamerParty.GetHealthyMonster();
                if (nextMonster != null)
                {
                    // Send out next monster
                    StartCoroutine(AboutToSummon(nextMonster));
                }
                else
                {
                    BattleOver(true);
                }
            }
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

        if (!mustSummonEnemyMonster)
        {
            ChangeState(BattleState.RunningTurn);
        }
        else
        {
            StartCoroutine(SendNextTamerMonster());
        }
    }

    private IEnumerator SendNextTamerMonster()
    {
        ChangeState(BattleState.Busy);

        Monster nextMonster = enemyTamerParty.GetHealthyMonster();
        enemyUnit.Setup(nextMonster);

        Coroutine enemyEnterAnimationCor = StartCoroutine(enemyUnit.PlayEnterAnimation());
        yield return battleDialogueBox.TypeDialogue($"{ enemyTamerController.Name } sent out { nextMonster.MonsterBase.Name }.");
        yield return enemyEnterAnimationCor;

        mustSummonEnemyMonster = false;
        ChangeState(BattleState.RunningTurn);
    }

    private void ChangeState(BattleState newState)
    {
        previousState = state;
        state = newState;
    }
}
