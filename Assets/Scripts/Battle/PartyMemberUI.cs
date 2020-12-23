using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Slider healthSliderBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Slider energySliderBar;
    [SerializeField] private Text energyText;

    private Monster m_pMonster;

    public void SetupHUD(Monster monster)
    {
        m_pMonster = monster;

        nameText.text = "Name: " + monster.MonsterBase.Name;
        levelText.text = "Lvl: " + monster.Level;
        healthSliderBar.value = ((float)monster.CurrentHealth / monster.MaxHealth);
        energySliderBar.value = ((float)monster.CurrentEnergy / monster.MaxEnergy);
        healthText.text = monster.CurrentHealth + "/" + monster.MaxHealth;
        energyText.text = monster.CurrentEnergy + "/" + monster.MaxEnergy;
    }
}
