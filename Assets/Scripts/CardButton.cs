using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Image))]
public class CardButton : MonoBehaviour,IPointerClickHandler {//和抽卡有关的事情都在这里,这里的card应该是指可以点击的框框,里面的内容应该被封装起来
    
    CardSO card;
    public void OnPointerClick(PointerEventData eventData) {
        if(card == null || card.ChampionPrefab.GetComponent<Champion>() == null) return;
        if(Player.Instance.Money < card.Cost)return; 
        if((Player.Instance.TotalAvailabeSpace > AllyChampionManager.SpaceTakenByChampions && GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY)
        || !QuadsManager.Instance.IsPreparationQuadsFull 
        || AllyChampionManager.Instance.CanThisChampionUpgrade(card.ChampionPrefab.GetComponent<Champion>())) {
            //如果场上有位置并且在deploy阶段,或者场下还有位置
            //这里还有条件,或者当前要买的这个英雄能不能与友方场上的合成?
            GameEventsManager.TriggerEvent(GameEventTypeChampion.BUY_A_CHAMPION,card.ChampionPrefab.GetComponent<Champion>());
            //买了英雄之后,我这里就应该disable
            card = null;
            GetComponent<Image>().enabled = false;
        }
    }
    public void OnRefresh(CardSO card) {
        if(card == null) {
            Debug.LogWarning("no card available!");
            return;
        }
        this.card = card;
        GetComponent<Image>().enabled = true;
        GetComponent<Image>().sprite = card.CardSprite;

    }
}
