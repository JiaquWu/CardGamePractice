﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CapsuleCollider))]
public class AbilityCustomTrigger : MonoBehaviour,IPoolObject {
    private event Action<Champion> triggerEvent;
    private CapsuleTriggerData data;
    private CapsuleCollider capsuleCollider;
    public void UpdateTrigger(Action<Champion> action,CapsuleTriggerData _data) {
        capsuleCollider = GetComponent<CapsuleCollider>();
        data = _data;
        triggerEvent = action;
        capsuleCollider.center = data.center;
        capsuleCollider.radius = data.radius;
        capsuleCollider.height = data.height;
        capsuleCollider.direction = data.direction;
        if(data.needMove) {
            StartCoroutine(TriggerMove());
        }
        StartCoroutine(DestoryCountDown());
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<Champion>(out Champion champion)) {
            triggerEvent?.Invoke(champion);
        }
    }
    IEnumerator TriggerMove() {
        for (int i = 0; i < data.moveData.moveCount; i++) {
            float timer = Time.time;
            while(Time.time - timer <= data.moveData.moveTimePerCount) {
                transform.position = Vector3.MoveTowards(transform.position,transform.position + data.moveData.moveDirection,data.moveData.moveSpeed);
            }
            yield return data.moveData.moveIntervalPercount;
        }
    }
    IEnumerator DestoryCountDown() {
        yield return new WaitForSeconds(data.duration);
        gameObject.SetActive(false);
    }
    public void OnObjectReuse() {
        triggerEvent = null;
        data = null;
        StopAllCoroutines();
    }

    public void Destroy() {
        
    }
}
[Serializable]
public class CapsuleTriggerData {
    public Vector3 initiatePositionOffset;
    public Vector3 initiateRotationOffset;
    public Vector3 center;
    public float radius;
    public float height;
    public int direction;//The value can be 0, 1 or 2 corresponding to the X, Y and Z axes, respectively.
    public bool needMove;
    public TriggerMoveData moveData;
    public float duration;
}
[Serializable]
public class TriggerMoveData {
    public int moveCount;//1 = move once
    public Vector3 moveDirection;
    public float moveSpeed;
    public float moveTimePerCount;
    public float moveIntervalPercount;
    
}