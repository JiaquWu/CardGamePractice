using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(RectTransform),typeof(Text))]
public class DamagePopup : MonoBehaviour {
    [SerializeField]
    private AnimationCurve moveCurve;
    [SerializeField]
    private AnimationCurve colorCurve;
    private RectTransform rect;
    private Text text;
    [SerializeField]
    private float moveMutiplier;
    public void UpdateDamage(float damage,DamageType damageType) {
        rect = GetComponent<RectTransform>();
        text = GetComponent<Text>();
        text.text = damage.ToString();
        text.color = damageType == DamageType.PHYSICS? Color.red : Color.cyan;   
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut() {
        float x = 0f;
        Color originColor = text.color;
        while(x <= moveCurve.keys[moveCurve.length-1].time) {
            rect.anchoredPosition += new Vector2(x,moveCurve.Evaluate(x)) * moveMutiplier;
            text.color = new Color(originColor.r,originColor.g,originColor.b,colorCurve.Evaluate(x));
            x += Time.deltaTime;
            yield return null; 
        }
        Destroy(gameObject);
    }
}
