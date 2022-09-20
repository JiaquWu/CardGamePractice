using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitPanel : PanelBase {
    [SerializeField]
    private GameObject traitTextPrefab;
    private Dictionary<TraitBase,Text> traitTextDict;
    private void OnEnable() {
        traitTextDict = new Dictionary<TraitBase, Text>();
        GameEventsManager.StartListening(GameEventTypeVoid.UPDATE_CURRENT_TRAITS,UpdateCurrentTraitsUI);
    }

    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeVoid.UPDATE_CURRENT_TRAITS,UpdateCurrentTraitsUI);        
    }
    private void UpdateCurrentTraitsUI(GameEventTypeVoid ev) {
        foreach (TraitBase trait in AllyChampionManager.Instance.traitsDict.Keys) {
            if(!traitTextDict.ContainsKey(trait)) {
                //如果字典里面没有,说明要新加上一个羁绊,要新生成一个
                traitTextDict.Add(trait,InstantiateText());
            }
            Debug.Log(trait.TriatName + "??????????");
            int count = AllyChampionManager.Instance.traitsDict[trait].GroupBy(x=>x.ChampionName).Select(x=>x.FirstOrDefault()).ToList().Count;
            if(count == 0) {
                GameObject go = traitTextDict[trait].gameObject;
                Destroy(go);
                traitTextDict.Remove(trait);
            }else {
                traitTextDict[trait].text = trait.TriatName + ":\r\n" + count;
            }
        }
    }
    private Text InstantiateText() {
        return Instantiate(traitTextPrefab,transform).GetComponent<Text>();
    }
}
