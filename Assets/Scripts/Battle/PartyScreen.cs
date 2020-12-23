using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private Button backButton;

    private PartyMemberUI[] memberSlots;
    private List<Monster> monsters;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Monster> monsters, bool mustChooseMonster)
    {
        this.monsters = monsters;

        for (int i = 0; i < memberSlots.Length; ++i)
        {
            if (i < monsters.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetupHUD(monsters[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Monster";

        backButton.gameObject.SetActive(!mustChooseMonster);
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
