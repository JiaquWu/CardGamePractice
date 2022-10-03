using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Image))]
public class CardButton : MonoBehaviour,IPointerClickHandler {
    
    CardSO card;
    public void OnPointerClick(PointerEventData eventData) {
        if(card == null || card.ChampionPrefab.GetComponent<Champion>() == null) return;
        if(Player.Instance.Money < card.Cost)return; 
        if((Player.Instance.TotalAvailabeSpace > AllyChampionManager.SpaceTakenByChampions && GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY)
        || !QuadsManager.Instance.IsPreparationQuadsFull 
        || AllyChampionManager.Instance.CanThisChampionUpgrade(card.ChampionPrefab.GetComponent<Champion>())) {
            GameEventsManager.TriggerEvent(GameEventTypeChampion.BUY_A_CHAMPION,card.ChampionPrefab.GetComponent<Champion>());
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
