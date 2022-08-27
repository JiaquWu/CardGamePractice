using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card :MonoBehaviour,IPointerClickHandler {//和抽卡有关的事情都在这里
    [SerializeField]
    private int cost;//购买所需费用
    [SerializeField]
    private GameObject championPrefab;//这张卡代表的英雄,应该还是要手动去配好
    public void OnPointerClick(PointerEventData eventData) {
        if(Player.Instance.Money >= cost) {
            //应该告诉一个英雄生成器之类的生成英雄
            ChampionGenerator.Instance.GenerateChampion(championPrefab);
        }
    }
}
