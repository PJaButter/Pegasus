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

    [SerializeField] private Image poisonImage;
    [SerializeField] private Image sleepImage;
    [SerializeField] private Image paralyzeImage;
    [SerializeField] private Image freezeImage;
    [SerializeField] private Image confusionImage;
    [SerializeField] private Image burnImage;
    [SerializeField] private Image drownImage;
    [SerializeField] private Image stunImage;
    [SerializeField] private Image knockbackImage;
    [SerializeField] private Image blindImage;
    [SerializeField] private Image unconsciousImage;

    private Monster m_pMonster;
    private Dictionary<ConditionID, Image> statusImages;

    public void SetupHUD(Monster monster)
    {
        m_pMonster = monster;

        nameText.text = "Name: " + monster.MonsterBase.Name;
        levelText.text = "Lvl: " + monster.Level;
        healthSliderBar.SetValue((float)monster.CurrentHealth / monster.MaxHealth);
        energySliderBar.SetValue((float)monster.CurrentEnergy / monster.MaxEnergy);
        healthText.text = monster.CurrentHealth + "/" + monster.MaxHealth;
        energyText.text = monster.CurrentEnergy + "/" + monster.MaxEnergy;

        statusImages = new Dictionary<ConditionID, Image>()
        {
            { ConditionID.Poison, poisonImage },
            { ConditionID.Sleep, sleepImage },
            { ConditionID.Paralyze, paralyzeImage },
            { ConditionID.Freeze, freezeImage },
            { ConditionID.Confusion, confusionImage },
            { ConditionID.Burn, burnImage },
            { ConditionID.Drown, drownImage },
            { ConditionID.Stun, stunImage },
            { ConditionID.KnockBack, knockbackImage },
            { ConditionID.Blind, blindImage },
            { ConditionID.Unconscious, unconsciousImage }
        };

        SetStatusIcons();
        m_pMonster.OnStatusChanged += SetStatusIcons;
    }

    private void SetStatusIcons()
    {
        if (m_pMonster.Status == null)
        {
            foreach (Image statusImage in statusImages.Values)
            {
                statusImage.gameObject.SetActive(false);
            }
        }
        else
        {
            statusImages[m_pMonster.Status.ID].gameObject.SetActive(true);
        }
    }

    public IEnumerator UpdateHealth()
    {
        if (m_pMonster.HealthChanged)
        {
            yield return healthSliderBar.SetValueSmooth((float)m_pMonster.CurrentHealth / m_pMonster.MaxHealth,
                (currentValue) => { healthText.text = Mathf.FloorToInt(m_pMonster.MaxHealth * currentValue) + "/" + m_pMonster.MaxHealth; });
            m_pMonster.HealthChanged = false;
        }
    }

    public IEnumerator UpdateEnergy()
    {
        if (m_pMonster.EnergyChanged)
        {
            yield return energySliderBar.SetValueSmooth((float)m_pMonster.CurrentEnergy / m_pMonster.MaxEnergy,
                (currentValue) => { energyText.text = Mathf.FloorToInt(m_pMonster.MaxEnergy * currentValue) + "/" + m_pMonster.MaxEnergy; });
            m_pMonster.EnergyChanged = false;
        }
    }
}
