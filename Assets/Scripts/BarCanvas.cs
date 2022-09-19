using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BarCanvas : MonoBehaviour {
    [SerializeField]
    private Champion champion;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image manaBar;

    private void OnEnable() {
        champion.UpdateHealthBar += UpdateHealthBarUI;
        champion.UpdateManaBar += UpdateManaBarUI;
        UpdateHealthBarUI(champion.CurrentChampionStats.healthPoints);
        UpdateManaBarUI(champion.CurrentChampionStats.manaPoints);
        GameEventsManager.StartListening(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_1,UpdateUIScale);
        GameEventsManager.StartListening(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_2,UpdateUIScale);
        transform.localScale = new Vector3((float)1 / champion.transform.localScale.x, (float)1 / champion.transform.localScale.y, (float)1 / champion.transform.localScale.z); 
    }
    private void LateUpdate() {
        transform.LookAt(transform.position + Camera.main.transform.forward * 10);
        //transform.Rotate(0,180,0);
    }
    private void UpdateUIScale(GameEventTypeChampion ev,Champion target) {
        if(target == champion) {
            transform.localScale = new Vector3((float)1 / target.transform.localScale.x, (float)1 / target.transform.localScale.y, (float)1 / target.transform.localScale.z); 
        }
    }
    private void UpdateHealthBarUI(float hpAmount) {
        if(healthBar != null) {
            healthBar.fillAmount =  (float)hpAmount / champion.CurrentChampionStats.MaxHealthPoints;
        }
    }
    private void UpdateManaBarUI(float mpAmount) {
        if(manaBar != null) {
            manaBar.fillAmount = (float)mpAmount / champion.CurrentChampionStats.MaxManaPoints;
        }
    }
    private void OnDisable() {
        champion.UpdateHealthBar -= UpdateHealthBarUI;
        champion.UpdateManaBar -= UpdateManaBarUI;
        GameEventsManager.StopListening(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_1,UpdateUIScale);
        GameEventsManager.StopListening(GameEventTypeChampion.CHAMPION_UPGRADE_LEVEL_2,UpdateUIScale);
    }
}
