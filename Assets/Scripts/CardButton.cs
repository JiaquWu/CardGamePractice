using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardButton : MonoBehaviour,IPointerClickHandler {//和抽卡有关的事情都在这里,这里的card应该是指可以点击的框框,里面的内容应该被封装起来
    
    CardSO card;
    public void OnPointerClick(PointerEventData eventData) {
        if(card == null || card.ChampionPrefab.GetComponent<Champion>() == null) return;
        if(Player.Instance.Money >= card.Cost && 
        (Player.Instance.TotalAvailabeSpace > AllyChampionManager.SpaceTakenByChampions
        || !QuadsManager.Instance.IsPreparationQuadsFull)) {//这里还要知道场上或者场下还有没有位置
            GameEventsManager.TriggerEvent(GameEventTypeChampion.BUY_A_CHAMPION,card.ChampionPrefab.GetComponent<Champion>());
            //应该告诉一个英雄生成器之类的生成英雄
        }
    }
    public void OnRefresh(CardSO card) {
        if(card == null) {
            Debug.LogWarning("no card available!");
            return;
        }
        this.card = card;
        GetComponent<Image>().sprite = card.CardSprite;

    }
}
