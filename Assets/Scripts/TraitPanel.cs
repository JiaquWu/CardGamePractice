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
                traitTextDict.Add(trait,InstantiateText());
            }
            int count = AllyChampionManager.Instance.GetTraitChampionCount(trait);
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
