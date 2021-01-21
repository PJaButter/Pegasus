using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialogue, Cutscene }

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Get { get { return instance; } }

    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera battleCamera;
    [SerializeField] private string currentAreaID;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject battleSystem;
    [SerializeField] private LayerMask fovLayer;

    private GameState gameState;
    private TamerController enemyTamerController;

    public Camera WorldCamera { get { return worldCamera; } set { worldCamera = value; } }
    public Camera BattleCamera { get { return battleCamera; } set { battleCamera = value; } }
    public string CurrentAreaID { get { return currentAreaID; } set { currentAreaID = value; } }
    public LayerMask FOVLayer { get { return fovLayer; } }

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

        ConditionsDB.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController.OnEncountered += StartWildBattle;

        playerController.OnEnteredTamersView += () =>
        {
            gameState = GameState.Cutscene;
            playerController.Pause();
        };

        DialogueManager.Get.OnShowDialogue += () =>
        {
            gameState = GameState.Dialogue;
            playerController.Pause();
        };

        DialogueManager.Get.OnCloseDialogue += () =>
        {
            if (gameState == GameState.Dialogue)
            {
                gameState = GameState.FreeRoam;
                playerController.Resume();
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Dialogue)
        {
            DialogueManager.Get.HandleUpdate();
        }
    }

    public void StartWildBattle(WildMonster wildMonster)
    {
        gameState = GameState.Battle;

        playerController.Pause();
        battleSystem.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);

        MonsterParty playerParty = playerController.GetComponent<MonsterParty>();

        BattleManager.Get.StartWildBattle(playerParty, wildMonster);
    }

    public void StartTamerBattle(TamerController tamerController)
    {
        gameState = GameState.Battle;

        enemyTamerController = tamerController;

        playerController.Pause();
        battleSystem.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);

        MonsterParty playerParty = playerController.GetComponent<MonsterParty>();
        MonsterParty tamerParty = tamerController.GetComponent<MonsterParty>();

        BattleManager.Get.StartTamerBattle(playerParty, tamerParty);
    }

    public void EndBattle(bool won)
    {
        gameState = GameState.FreeRoam;

        if (enemyTamerController != null && won)
        {
            enemyTamerController.BattleLost();
            enemyTamerController = null;
        }

        playerController.Resume();
        BattleManager.Get.ExitBattle();
        worldCamera.gameObject.SetActive(true);
        battleCamera.gameObject.SetActive(false);
        battleSystem.SetActive(false);
    }
}
