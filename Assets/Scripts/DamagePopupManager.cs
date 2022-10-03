using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopupManager : SingletonManager<DamagePopupManager> {
    [SerializeField]
    private GameObject damagePopupPrefab;
    [SerializeField]
    private Vector3 offset;
    public void CreateAPopup(Transform target,float damage,DamageType damageType) {
        //should use object pool
        GameObject popup = Instantiate(damagePopupPrefab,Camera.main.WorldToScreenPoint(target.position) + offset,Quaternion.identity,transform);
        if(popup.TryGetComponent<DamagePopup>(out DamagePopup up)) {
            up.UpdateDamage(damage,damageType);
        }
    }
}
