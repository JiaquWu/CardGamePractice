using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Cards/Card",fileName = "Card")]
public class CardSO : ScriptableObject {
    public string ChampionName {
        get {
            Champion champion = null;
            if(championPrefab.TryGetComponent<Champion>(out champion)) {
                return champion.ChampionName;
            }
            return null;
        }
    }
    [SerializeField]
    private int cost;//购买所需费用
    public int Cost => cost;
    [SerializeField]
    private Sprite cardSprite;
    public Sprite CardSprite => cardSprite;
    [SerializeField]
    private GameObject championPrefab;//这张卡代表的英雄,应该还是要手动去配好
    public GameObject ChampionPrefab => championPrefab;
}
