using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBattleHUD : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private SliderBar healthSliderBar;
    [SerializeField] private Text healthText;
    [SerializeField] private SliderBar energySliderBar;
    [SerializeField] private Text energyText;

    private Monster m_pMonster;

    public void SetupHUD(Monster monster)
    {
        m_pMonster = monster;

        nameText.text = "Name: " + monster.MonsterBase.Name;
        levelText.text = "Lvl: " + monster.Level;
        healthSliderBar.SetValue((float)monster.CurrentHealth / monster.MaxHealth);
        energySliderBar.SetValue((float)monster.CurrentEnergy / monster.MaxEnergy);
        healthText.text = monster.CurrentHealth + "/" + monster.MaxHealth;
        energyText.text = monster.CurrentEnergy + "/" + monster.MaxEnergy;
    }

    public IEnumerator UpdateHealth()
    {
        yield return healthSliderBar.SetValueSmooth((float)m_pMonster.CurrentHealth / m_pMonster.MaxHealth,
            (currentValue)=> { healthText.text = Mathf.FloorToInt(m_pMonster.MaxHealth * currentValue) + "/" + m_pMonster.MaxHealth; });
    }
}
