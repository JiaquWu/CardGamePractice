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
    private int cost;
    public int Cost => cost;
    [SerializeField]
    private Sprite cardSprite;
    public Sprite CardSprite => cardSprite;
    [SerializeField]
    private GameObject championPrefab;//need to be specified manually
    public GameObject ChampionPrefab => championPrefab;
}
