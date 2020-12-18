using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Get { get { return instance; } }

    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera battleCamera;
    [SerializeField] private string currentAreaID;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject battleSystem;

    private GameState gameState;

    public Camera WorldCamera { get { return worldCamera; } set { worldCamera = value; } }
    public Camera BattleCamera { get { return battleCamera; } set { battleCamera = value; } }
    public string CurrentAreaID { get { return currentAreaID; } set { currentAreaID = value; } }

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
        playerController.OnEncountered += StartBattle;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle()
    {
        gameState = GameState.Battle;

        playerController.Pause();
        battleSystem.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);
        StartCoroutine(BattleManager.Get.EnterBattle());
    }

    public void EndBattle(bool won)
    {
        gameState = GameState.FreeRoam;

        playerController.Resume();
        BattleManager.Get.ExitBattle();
        worldCamera.gameObject.SetActive(true);
        battleCamera.gameObject.SetActive(false);
        battleSystem.SetActive(false);
    }
}
