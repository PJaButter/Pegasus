using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private MonsterBattleHUD hud;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] float offScreenPosX;

    private Image monsterImage;
    private Vector3 originalPos;
    private Color originalColor;

    public MonsterBattleHUD HUD { get { return hud; } }
    public bool IsPlayerUnit { get { return isPlayerUnit; } }
    public Monster Monster { get; set; }

    private void Awake()
    {
        monsterImage = GetComponent<Image>();
        originalPos = monsterImage.rectTransform.anchoredPosition;
        originalColor = monsterImage.color;
    }

    public void Setup(Monster monster)
    {
        Monster = monster;
        if (isPlayerUnit)
        {
            monsterImage.sprite = Monster.MonsterBase.BackSprite;
        }
        else
        {
            monsterImage.sprite = Monster.MonsterBase.FrontSprite;
        }

        hud.SetupHUD(monster);
    }

    public IEnumerator PlayEnterAnimation()
    {
        monsterImage.rectTransform.anchoredPosition = new Vector3(offScreenPosX, originalPos.y);
        monsterImage.color = originalColor;
        yield return LerpPosition(originalPos, 1.0f);
    }

    public IEnumerator PlayAttackAnimation()
    {
        if (isPlayerUnit)
            yield return LerpPosition(new Vector3(originalPos.x + 50, originalPos.y), 0.25f);
        else
            yield return LerpPosition(new Vector3(originalPos.x - 50, originalPos.y), 0.25f);

        yield return LerpPosition(originalPos, 0.25f);
    }

    public IEnumerator PlayHitAnimation()
    {
        yield return LerpColor(Color.gray, 0.1f);
        yield return LerpColor(originalColor, 0.1f);
    }

    public IEnumerator PlayDeathAnimation()
    {
        Coroutine cor_LerpPosition = StartCoroutine(LerpPosition(new Vector3(originalPos.x, originalPos.y - 150), 0.5f));
        yield return LerpColor(Color.clear, 0.5f);
        yield return cor_LerpPosition;
    }

    private IEnumerator LerpPosition(Vector3 endPos, float time)
    {
        Vector3 startPos = monsterImage.rectTransform.anchoredPosition;
        float currentTime = 0;
        while (currentTime < time)
        {
            monsterImage.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, currentTime / time);
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, time);
            yield return null;
        }
        monsterImage.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, currentTime / time);
    }

    private IEnumerator LerpColor(Color endColor, float time)
    {
        Color startColor = monsterImage.color;
        float currentTime = 0;
        while (currentTime < time)
        {
            monsterImage.color = Color.Lerp(startColor, endColor, currentTime / time);
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, time);
            yield return null;
        }
        monsterImage.color = Color.Lerp(startColor, endColor, currentTime / time);
    }
}
