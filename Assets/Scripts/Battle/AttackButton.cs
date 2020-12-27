using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Text text;
    private Move move;

    void Start()
    {

    }

    public void SetupButton(Move move)
    {
        if (move != null)
        {
            this.GetComponent<Button>().interactable = true;
            gameObject.SetActive(true);
            this.move = move;
            text.text = move.MoveBase.Name;
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
            gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BattleManager.Get.SetAttackDetails(move);
    }

    public void OnClick()
    {
        BattleManager.Get.OnClick_AttackButton(move);
    }
}
