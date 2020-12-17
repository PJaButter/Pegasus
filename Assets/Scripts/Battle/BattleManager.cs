using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleManager : MonoBehaviour
{
    private static BattleManager instance = null;
    public static BattleManager Get { get { return instance; } }

    private bool inBattle;
    private bool canFlee;

    private BattleState state;

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
    [SerializeField] private MonsterBattleHUD playerMonsterHUD;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private MonsterBattleHUD enemyMonsterHUD;

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

    public IEnumerator EnterBattle()
    {
        GameManager.Get.WorldCamera.gameObject.SetActive(false);
        GameManager.Get.BattleCamera.gameObject.SetActive(true);
        inBattle = true;

        // Set info from battle data
        canFlee = true;

        playerUnit.Setup();
        enemyUnit.Setup();

        playerMonsterHUD.SetupHUD(playerUnit.Monster);
        enemyMonsterHUD.SetupHUD(enemyUnit.Monster);

        SetupAttackButtons(playerUnit.Monster.BasicMove, playerUnit.Monster.Moves);

        yield return battleDialogueBox.TypeDialogue($"A wild { enemyUnit.Monster.MonsterBase.Name } appeared.");

        PlayerAction();

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
        PlayerMove();
        mainBattleDialogueBox.gameObject.SetActive(false);
        actionsPanel.SetActive(false);
        moveDetailsPanel.SetActive(true);
        attackPanel.SetActive(true);
    }

    public void OnClick_AttackBackButton()
    {
        PlayerAction();
        mainBattleDialogueBox.gameObject.SetActive(true);
        actionsPanel.SetActive(true);
        moveDetailsPanel.SetActive(false);
        attackPanel.SetActive(false);
    }

    public void OnClick_AttackButton(Move move)
    {
        StartCoroutine(PerformPlayerMove(move));
        mainBattleDialogueBox.gameObject.SetActive(true);
        battleDialogueBox.EnableDialogueText(true);
        actionsPanel.SetActive(false);
        moveDetailsPanel.SetActive(false);
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

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(battleDialogueBox.TypeDialogue("Choose an action"));
        battleDialogueBox.EnableActionsPanel(true);
    }

    private void PlayerMove()
    {
        state = BattleState.PlayerMove;
        battleDialogueBox.EnableActionsPanel(false);
        battleDialogueBox.EnableDialogueText(false);
        battleDialogueBox.EnableAttackPanel(false);
    }

    private void SetupAttackButtons(Move basicMove, List<Move> moves)
    {
        basicAttackButton.SetupButton(basicMove);
        for (int i = 0; i < attackButtons.Count; ++i)
        {
            attackButtons[i].SetupButton(i < moves.Count ? moves[i] : null);
        }
    }

    private IEnumerator PerformPlayerMove(Move move)
    {
        state = BattleState.Busy;

        yield return battleDialogueBox.TypeDialogue($"{playerUnit.Monster.MonsterBase.Name} used {move.MoveBase.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1.0f);

        enemyUnit.PlayHitAnimation();
        DamageDetails damageDetails = enemyUnit.Monster.TakeDamage(move, playerUnit.Monster);
        yield return enemyMonsterHUD.UpdateHealth();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return battleDialogueBox.TypeDialogue($"{enemyUnit.Monster.MonsterBase.Name} Died");
            enemyUnit.PlayDeathAnimation();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    private IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        Move move = enemyUnit.Monster.GetRandomMove();

        yield return battleDialogueBox.TypeDialogue($"{enemyUnit.Monster.MonsterBase.Name} used {move.MoveBase.Name}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1.0f);

        playerUnit.PlayHitAnimation();
        DamageDetails damageDetails = playerUnit.Monster.TakeDamage(move, enemyUnit.Monster);
        yield return playerMonsterHUD.UpdateHealth();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return battleDialogueBox.TypeDialogue($"{playerUnit.Monster.MonsterBase.Name} Died");
            playerUnit.PlayDeathAnimation();
        }
        else
        {
            PlayerAction();
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
}
