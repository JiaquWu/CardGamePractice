using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public enum ChampionState {
    PREPARE,
    IDLE,
    WALK,
    ATTACK,
    CAST_ABILITY,
    DEAD,
}

[RequireComponent(typeof(Animator))]
public class Champion : MonoBehaviour {

    [SerializeField]
    private string championName;
    public string ChampionName => championName;
    [SerializeField]
    private int tier;
    public int Tier => tier;
    [SerializeField]
    private int defaultCost;
    private int currentCost;
    public int Cost {
        get {
            return currentCost == 0? defaultCost : currentCost;
        }
    }
    private int currentLevel;
    public int Level => currentLevel;
    [SerializeField]
    private int space;
    public int Space {
        get {
            if(space == 0) {
                space = 1;
            }
            return space;
        }
    }
    private bool isMouseHoveringOnThisChampion;
    [SerializeField]
    private bool isAllyChampion;
    public bool IsAllyChampion => isAllyChampion;
    public bool IsActive;
    private StateMachine<ChampionState> championStateMachine = new StateMachine<ChampionState>();
    ChampionPrepare championPrepareState;
    ChampionIdle championIdleState;
    ChampionWalk championWalkState;
    ChampionAttack championAttackState;
    ChampionCastAbility championCastAbilityState;
    ChampionDead championDeadState;
    protected Animator animator;
    protected Animator Animator {
        get{
            if(animator == null) {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }
    [SerializeField]
    private ChampionStats defaultChampionStats;
    private ChampionStats currentChampionStats;
    [HideInInspector]
    public ChampionStats CurrentChampionStats {
        get {
            if(currentChampionStats != null) return currentChampionStats;
            if(defaultChampionStats != null) {
                currentChampionStats = (ChampionStats)ScriptableObject.CreateInstance("ChampionStats");
                currentChampionStats.CopyChampionStats(defaultChampionStats,currentLevel);
                return currentChampionStats;
            }
            return null;
        }
    }
    [SerializeField]
    public List<TraitBase> traits = new List<TraitBase>();
    private Dictionary<TraitBase,int> traitActivateStateDict;
    public ChampionAbility championAbility;

    protected ChampionState currentChampionState;
    Quad lastQuadThisChampionStand = null;//need to change when a champion's position is changing
    public Quad LastQuadThisChampionStand => lastQuadThisChampionStand;
    Quad lastMouseHoveringQuad = null;
    Quad currentMouseHoveringQuad = null;

    public event Action<float> UpdateHealthBar;
    public event Action<float> UpdateManaBar;
    public Action<Champion,Champion> additionalAttackEffect;
    private void Update() {
        if(championStateMachine != null) {
            championStateMachine.OnLogic();
        } 
    }
    #region Quad related
    public void OnDeploy(Quad quadToStay,bool isAlly,int level = 0) {
        isAllyChampion = isAlly;
        if(isAlly) {          
            currentLevel = 0;
            currentCost = defaultCost;
        }else {
            currentLevel = level;
            if(currentLevel == 0) {
                currentChampionStats = (ChampionStats)ScriptableObject.CreateInstance("ChampionStats");
                currentChampionStats.CopyChampionStats(defaultChampionStats,currentLevel);
            }else {
                OnChampionUpgrade(currentLevel);
            }
        }        
        traitActivateStateDict = new Dictionary<TraitBase, int>();
        foreach (TraitBase trait in traits) {
            traitActivateStateDict.Add(trait,-1);
        }
        InitFSM();
        OnEnterQuad(quadToStay);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StartListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
        GameEventsManager.StartListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSell);
        GameEventsManager.StartListening(GameEventTypeVoid.ON_SELL_BUTTON_DOWN,OnSellButtonDown);
        RegisterThisChampion();
    }
    public void OnDisappear() {//will be invoked when a champion is being sold or upgrading
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_DEPLOY_STATE,OnEnterDeployState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENTER_COMBAT_STATE,OnEnterCombatState);
        GameEventsManager.StopListening(GameEventTypeVoid.ENETR_BONUS_STATE,OnEnterBonusState);
        GameEventsManager.StopListening(GameEventTypeChampion.SELL_A_CHAMPION,OnSell);
        GameEventsManager.StopListening(GameEventTypeVoid.ON_SELL_BUTTON_DOWN,OnSellButtonDown);
    }
    public void OnChampionSwap(Champion championOnTheTargetQuad) {
        championOnTheTargetQuad.OnEnterQuad(lastQuadThisChampionStand,true);
    }
    public void OnEnterQuad(Quad quad,bool isSwaping = false) {
        if(quad is DeployQuad) {
            championStateMachine.Trigger("OnDeployQuad");
            if(lastQuadThisChampionStand == null || lastQuadThisChampionStand is PreparationQuad) {
                if(isAllyChampion) {
                    AllyChampionManager.Instance.OnSpaceChange(Space);
                    AllyChampionManager.Instance.UpdateCurrentTraits(this,true);
                } 
            }
        }else if(quad is PreparationQuad){     
            championStateMachine.Trigger("OnPreparationQuad");
            if(lastQuadThisChampionStand != null && lastQuadThisChampionStand is DeployQuad) {
                if(isAllyChampion) {
                    AllyChampionManager.Instance.OnSpaceChange(Space * -1);
                    AllyChampionManager.Instance.UpdateCurrentTraits(this,false);
                    ResetAllTraitsLevel();
                }
            }
        }else if(quad is EnemyQuad) {//only works for enemy champions 
            EnemyChampionManager.Instance.UpdateCurrentTraits(this,true);//need to update when enemy's build is changing
            championStateMachine.Trigger("EnterEnemyQuad");
        }
        transform.position = quad.node.worldPosition;
        if(lastQuadThisChampionStand != null && quad.ChampionOnThisQuad == null && lastQuadThisChampionStand != quad) {
            //which means it's moving from a quad to an empty quad, and not moving to the same quad
            lastQuadThisChampionStand.OnChampionLeave(this);
        }
        quad.OnChampionStay(this,isSwaping);
        lastMouseHoveringQuad = quad;
        currentMouseHoveringQuad = quad;
    }
    public void MoveToNewQuad(Vector3 target,float speed,Champion targetChampion,ref bool isWalking,Action findNewPathFunc,Action triggerAttackFunc) {       
        if(Vector3.Distance(transform.position,target) <= float.Epsilon) {
            //a champion is standing on a new quad, need to check attack range first
            if(Vector3.Distance(targetChampion.transform.position,transform.position) 
            <= currentChampionStats.attackRange.Value * MapManager.Instance.CurrentMapConfiguration.ScaleRatio) {
                transform.LookAt(target);
                triggerAttackFunc?.Invoke();
                isWalking = false;
            }else {
                findNewPathFunc?.Invoke();
            }
        }else {
            transform.position = Vector3.MoveTowards(transform.position,target,speed * Time.deltaTime);
            transform.LookAt(target); 
        }
    }
    public void QuadStateChange(Vector3 target) {
        Debug.Log("quad的状态改变了一次");
        Quad quad = QuadsManager.Instance.GetCombatQuadByPostion(target);
        if(quad != null) {
            lastQuadThisChampionStand.OnChampionLeave(this);
            quad.OnChampionStay(this);
            lastQuadThisChampionStand = quad;
        }
    }
    #endregion
    #region Mouse Interaction      
    private void OnMouseDrag() {
        if(!(GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY
        || (GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT && lastQuadThisChampionStand is PreparationQuad))) {
            return;
        }
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,screenPos.z));
        temp.y = QuadsManager.Instance.CurrentMap.OriginPoint.y;
        transform.position = temp;
        currentMouseHoveringQuad = QuadsManager.Instance.GetAllyQuadByPosition(transform.position);
        if(currentMouseHoveringQuad == null && lastMouseHoveringQuad != null) {
            transform.position = lastMouseHoveringQuad.node.worldPosition;
            return;
        }
        if(lastMouseHoveringQuad != null && lastMouseHoveringQuad != currentMouseHoveringQuad) {
            lastMouseHoveringQuad.OnChampionExitOnMouse(this);
        }
        currentMouseHoveringQuad.OnChampionEnterOnMouse(this);
        lastMouseHoveringQuad = currentMouseHoveringQuad;
    }
    private void OnMouseUp() {//need to check if a champion can entering a new quad
        if(lastMouseHoveringQuad != null) {
            if(isAllyChampion) {
                if(lastQuadThisChampionStand is PreparationQuad && lastMouseHoveringQuad is DeployQuad) {
                    //if it's moving from preparation quad
                    if(GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY
                    && ((lastMouseHoveringQuad.ChampionOnThisQuad == null && Player.Instance.TotalAvailabeSpace 
                        >= AllyChampionManager.SpaceTakenByChampions + Space)
                    || (lastMouseHoveringQuad.ChampionOnThisQuad != null && Player.Instance.TotalAvailabeSpace 
                        >= AllyChampionManager.SpaceTakenByChampions + lastMouseHoveringQuad.ChampionOnThisQuad.Space - Space))) {
                        OnEnterQuad(lastMouseHoveringQuad);
                    }else {
                        OnEnterQuad(lastQuadThisChampionStand);
                    }
                }else if(lastQuadThisChampionStand is DeployQuad && lastMouseHoveringQuad is PreparationQuad) {
                    if(lastMouseHoveringQuad.ChampionOnThisQuad == null 
                    ||(lastMouseHoveringQuad.ChampionOnThisQuad != null && Player.Instance.TotalAvailabeSpace 
                        >= AllyChampionManager.SpaceTakenByChampions + lastMouseHoveringQuad.ChampionOnThisQuad.Space - Space)) {
                            OnEnterQuad(lastMouseHoveringQuad);
                    }else {
                        OnEnterQuad(lastQuadThisChampionStand);
                    }             
                }else {
                    OnEnterQuad(lastMouseHoveringQuad);
                }
            } 
        }
        
    }
    private void OnMouseOver() {
        isMouseHoveringOnThisChampion = true;
    }
    private void OnMouseExit() {
        isMouseHoveringOnThisChampion = false;
    }
    #endregion
    public void InitFSM() {
        championPrepareState = new ChampionPrepare(Animator,false);
        championIdleState = new ChampionIdle(this,Animator,false);
        championWalkState = new ChampionWalk(this,Animator,(x)=>{CombatEnd(x);}, 
        (target)=>{championAttackState.UpdateTargetChampion(target);
        championStateMachine.Trigger("Attack");}, false);
        championAttackState = new ChampionAttack(this,Animator,false);
        championCastAbilityState = new ChampionCastAbility(this,Animator,false);
        championDeadState = new ChampionDead(this,Animator,false);

        championStateMachine.AddState(ChampionState.PREPARE,championPrepareState);
        championStateMachine.AddState(ChampionState.IDLE,championIdleState);
        championStateMachine.AddState(ChampionState.WALK,championWalkState);
        championStateMachine.AddState(ChampionState.ATTACK,championAttackState);
        championStateMachine.AddState(ChampionState.CAST_ABILITY,championCastAbilityState);
        championStateMachine.AddState(ChampionState.DEAD,championDeadState);

        championStateMachine.AddTriggerTransition("BattleStart",ChampionState.IDLE,ChampionState.WALK);
        championStateMachine.AddTriggerTransition("Attack",ChampionState.WALK,ChampionState.ATTACK);
        championStateMachine.AddTriggerTransition("TargetDead",ChampionState.ATTACK,ChampionState.WALK);
        championStateMachine.AddTriggerTransition("ForceToWalk",ChampionState.ATTACK,ChampionState.WALK);
        championStateMachine.AddTriggerTransition("FinishCastAbility",ChampionState.CAST_ABILITY,ChampionState.WALK);
        championStateMachine.AddTriggerTransition("OnPreparationQuad",ChampionState.IDLE,ChampionState.PREPARE);
        championStateMachine.AddTriggerTransition("OnDeployQuad",ChampionState.PREPARE,ChampionState.IDLE);

        championStateMachine.AddTriggerTransitionFromAny("CastAbility",ChampionState.CAST_ABILITY);//
        championStateMachine.AddTriggerTransitionFromAny("Dead",ChampionState.DEAD);//would die at any time
        championStateMachine.AddTriggerTransitionFromAny("EnterDeployStateIdle",ChampionState.IDLE);
        championStateMachine.AddTriggerTransitionFromAny("EnterEnemyQuad",ChampionState.IDLE);
        championStateMachine.Init();
    }
    #region Trait related        
    public void UpdateTraitLevelToChampion(TraitBase trait,int traitLevel) {
        if(traitActivateStateDict.ContainsKey(trait)) {
            traitActivateStateDict[trait] = traitLevel;
        }
        if(trait.buffFactory != null && trait.buffFactory.buffType == BuffTypeEnum.INSTANT) {
            if(traitLevel != -1) {
                trait.Apply(this,traitLevel);
            }else {
                trait.Remove(this);
            }
        }
    }
    private void ResetAllTraitsLevel() {
        foreach (TraitBase trait in traits) {
            UpdateTraitLevelToChampion(trait,-1);
        }
    }
    #endregion
    #region Battle related
    public void ForceAttackTarget(Champion targetChampion) {
        StopAllCoroutines();
        if(championStateMachine.ActiveState.name == ChampionState.WALK) {
            championWalkState.MoveToTarget(targetChampion.transform.position,targetChampion,true,true);        
        }else if(championStateMachine.ActiveState.name == ChampionState.ATTACK) {
            if(IsTargetInAttackRange(targetChampion.transform.position)) {
                championAttackState.UpdateTargetChampion(targetChampion);
            }else {
                championWalkState.MoveToTarget(targetChampion.transform.position,targetChampion,true,true);
                championStateMachine.Trigger("ForceToWalk");
            }
        }
    }
    public bool IsTargetInAttackRange(Vector3 targetPos) {
        return Vector3.Distance(targetPos,transform.position) <= CurrentChampionStats.attackRange.Value * MapManager.Instance.CurrentMapConfiguration.ScaleRatio;
    }
    public void CheckIfTargetIsInRange() {
        if(championStateMachine.ActiveState.name == ChampionState.ATTACK) {
            if(!IsTargetInAttackRange(championAttackState.targetChampion.transform.position)) {
                ForceAttackTarget(championAttackState.targetChampion);//need to chase it if it's running away
            }
        }
    }
    public void HitTarget() {
        championAttackState.HitTarget();
    }
    public void OnHit(Champion champion) {
        TakeDamage(champion,DamageType.PHYSICS,champion.currentChampionStats.attackDamage.Value);
    }
    public void GainMana(float amount) {
        currentChampionStats.manaPoints.Value += amount;
        if(currentChampionStats.manaPoints.Value >= currentChampionStats.maxManaPoints.Value) {
            if(championAbility != null) {
                championAbility.Execute(this);
                //need to make all champions have ability anim to change to cast ability state
            }
            currentChampionStats.manaPoints.Value = 0;
        }
        UpdateManaBar?.Invoke(currentChampionStats.manaPoints.Value);
    }
    public void TakeDamage(Champion damageSource, DamageType damageType,float damage) {
        float result = this.CalculateDamage(damageType,damage);
        //UI display
        DamagePopupManager.Instance.CreateAPopup(transform,result,damageType);
        currentChampionStats.healthPoints.Value -= result;
        if(currentChampionStats.healthPoints.Value <= 0) {
            OnDead();
            damageSource.OnTargetDead();
        }else {
            GainMana(Mathf.CeilToInt(result / currentChampionStats.manaGainedByTakingDamage.Value));
            UpdateHealthBar?.Invoke(currentChampionStats.healthPoints.Value);
        }
    }
    
    public void OnDead() {
        championStateMachine.Trigger("Dead");
    }
    public void OnTargetDead() {
        if(championStateMachine.ActiveState.name == ChampionState.ATTACK) {
            championStateMachine.Trigger("TargetDead");
        }
    }
    public void CombatEnd(bool isAllyWin) {

    }
    #endregion
    public void OnEnterDeployState(GameEventTypeVoid ev) {
        if(lastQuadThisChampionStand is DeployQuad) {
            championStateMachine.Trigger("EnterDeployStateIdle");
        }
    }
    public void OnEnterCombatState(GameEventTypeVoid ev) {
        championStateMachine.Trigger("BattleStart");
    }
    public void OnEnterBonusState(GameEventTypeVoid ev) {
        
    }
    public void RegisterThisChampion() {
        if(isAllyChampion) {
            AllyChampionManager.Instance.RegisterChampion(this);
        }else {
            EnemyChampionManager.Instance.RegisterChampion(this);
        }
    }
    public void UnRegisterThisChampion() {
        if(isAllyChampion) {
            AllyChampionManager.Instance.UnregisterChampion(this);
        }else {
            EnemyChampionManager.Instance.UnregisterChampion(this);
        }
    }
    public Vector3 GetNearestOpponentChampionPos(out Champion targetChampion,out bool isAChampionAvailable) {
        targetChampion = null;
        Vector3 result = Vector3.zero;
        if(isAllyChampion) {
           result = EnemyChampionManager.Instance.GetNearestOpponentChampion(this,out targetChampion,out isAChampionAvailable);
        }else {
           result = AllyChampionManager.Instance.GetNearestOpponentChampion(this,out targetChampion, out isAChampionAvailable);
        }
        return result;
    }
    public void ModifyStats(ChampionStatsType statsType,float amount) {
        if(CurrentChampionStats.statsDict.ContainsKey(statsType)) {
            CurrentChampionStats.statsDict[statsType].Value += amount;
            if(statsType == ChampionStatsType.MAX_HEALTH_POINT) {
                ModifyStats(ChampionStatsType.HEALTH_POINT,amount);
            }else if(statsType == ChampionStatsType.MAX_MANA_POINT) {
                ModifyStats(ChampionStatsType.MANA_POINT,amount);
            }
        }
    }
    public void OnChampionUpgrade(int level) {
        //level = 0 : remove, level = 1 : upgrade to level 1, level = 2 : upgrade to level 2
        if(level == 0) {
            lastQuadThisChampionStand.OnChampionLeave(this);
            if(lastQuadThisChampionStand is DeployQuad) {
                AllyChampionManager.Instance.OnSpaceChange(Space * -1);
                AllyChampionManager.Instance.UpdateCurrentTraits(this,false);
            }
            OnDisappear();
            UnRegisterThisChampion();
            Destroy(gameObject);
        }else if(level == 1) {
            currentLevel = 1;
            currentCost = tier == 1? 3 : tier * 3 - 1;
            transform.localScale *= 1.2f;
            currentChampionStats.UpdateStats(defaultChampionStats,1);
        }else if(level == 2) {
            currentLevel = 2;
            currentCost = tier == 1? 9 : tier * 9 - 5;
            transform.localScale *= 1.2f;
            currentChampionStats.UpdateStats(defaultChampionStats,2);
        }
    }
    public void OnSell(GameEventTypeChampion ev,Champion _champion) {
        if(_champion != this) return;
        _champion.lastQuadThisChampionStand.OnChampionLeave(_champion);
        if(_champion.lastQuadThisChampionStand is DeployQuad) {
            if(_champion.IsAllyChampion) {
                AllyChampionManager.Instance.OnSpaceChange(_champion.Space * -1);
                AllyChampionManager.Instance.UpdateCurrentTraits(this,false);
            }
        }
        _champion.OnDisappear();
        _champion.UnRegisterThisChampion();
        Destroy(_champion.gameObject);
    }
    
    public void OnSellButtonDown(GameEventTypeVoid ev) {
        if(isMouseHoveringOnThisChampion && GameManager.Instance.GameManagerStateMachine.ActiveState.name == GameState.PLAY && isAllyChampion) {
            if((GameManager.Instance.PlayState.ActiveState.name == OnPlayState.DEPLOY ) 
            || (GameManager.Instance.PlayState.ActiveState.name == OnPlayState.COMBAT && lastQuadThisChampionStand is PreparationQuad)) { 
                GameEventsManager.TriggerEvent(GameEventTypeChampion.SELL_A_CHAMPION,this);
            }
            
        }
    }
}

public class ChampionIdle : StateBase<ChampionState> {//idle = on deploy quad
    Animator animator;
    Champion champion;
    public ChampionIdle(Champion champion, Animator animator, bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
        this.champion = champion;
    }
    public override void OnEnter() {
        animator.SetTrigger("Idle");
        champion.IsActive = true;
    }
    public override void OnLogic() {
    }
    public override void OnExit() {
    }
}

public class ChampionPrepare: StateBase<ChampionState> {
    Animator animator;
    public ChampionPrepare(Animator animator, bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
    }
    public override void OnEnter() {
        animator.SetTrigger("Idle");
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
    }
}
public class ChampionWalk: StateBase<ChampionState> {
    private Animator animator;
    private Champion champion;
    Vector3[] path;
    int targetIndex;
    float speed = 1;
    MonoBehaviour mono;
    Champion targetChampion;
    Action<Champion> attackTrigger;
    Action<bool> combatEndTrigger;
    bool isAChampionAvailable;
    public ChampionWalk(Champion champion, Animator animator,Action<bool> combatEndTrigger, Action<Champion> attackTrigger, bool needsExitTime) : base(needsExitTime) {
        mono = champion;
        this.champion = champion;
        this.animator = animator;
        this.attackTrigger = attackTrigger;
        this.combatEndTrigger = combatEndTrigger;
    }
    public override void OnEnter() {
        if(targetChampion == null) {
            CheckNearestOpponent();
        }
    }
    public override void OnLogic() {

    }
    public override void OnExit() {
        animator.ResetTrigger("Walk");
        targetChampion = null;
    }
    public void OnDynamicPathFound(Vector3[] newPath, bool pathSuccessful) {
        if(pathSuccessful) {
            path = newPath;
            mono.StopAllCoroutines();
            mono.StartCoroutine(FollowDynamicPath());
        }
    }
    public void OnFixedPathFound(Vector3[] newPath, bool pathSuccessful) {
        if(pathSuccessful) {
            path = newPath;
            mono.StopAllCoroutines();
            mono.StartCoroutine(FollowPathToFixedTarget(targetChampion));
        }
    }
    public void CheckNearestOpponent() { 
        Vector3 target = champion.GetNearestOpponentChampionPos(out targetChampion,out isAChampionAvailable);
        MoveToTarget(target,targetChampion,isAChampionAvailable);
    }
    public void MoveToTarget(Vector3 targetPos,Champion targetChampion,bool isAChampionAvailable,bool alreadyHasTarget = false) {
        if(alreadyHasTarget) {
            if(targetChampion != null && targetChampion.IsActive) {
                this.targetChampion = targetChampion;
                if(champion.IsTargetInAttackRange(targetPos)) {
                    TriggerAttackBehavior();
                }else {
                    FindPathTowardsTarget(targetPos);
                }
            }else {
                CheckNearestOpponent();
            } 
        }else {
            if(isAChampionAvailable) {
                if(champion.IsTargetInAttackRange(targetPos)) {
                    TriggerAttackBehavior();
                }else {
                    FindPathTowardsNearestOpponent(targetPos);
                }
            }else {
                combatEndTrigger?.Invoke(champion.IsAllyChampion);
            }
        }
    }
    public void FindPathTowardsTarget(Vector3 target) {
        PathRequestManager.RequestPath(champion.transform.position,target,OnFixedPathFound);
    }
    public void FindPathTowardsNearestOpponent(Vector3 newTarget) {
        Vector3 target = champion.GetNearestOpponentChampionPos(out targetChampion, out isAChampionAvailable);
        PathRequestManager.RequestPath(champion.transform.position,target,OnDynamicPathFound);
    }
    public void TriggerAttackBehavior() {
        attackTrigger?.Invoke(targetChampion);
    }
    IEnumerator FollowPathToFixedTarget(Champion targetChampion) {
        if(path.Length > 0) {
            animator.SetTrigger("Walk");
            Vector3 currentWayPoint = path[0];
            champion.QuadStateChange(currentWayPoint);
            while(true) {
                if(champion.transform.position == currentWayPoint) {
                    champion.transform.LookAt(currentWayPoint);
                    MoveToTarget(targetChampion.transform.position,targetChampion,true,true);
                    yield break;
                }else {
                    champion.transform.position = Vector3.MoveTowards(champion.transform.position,currentWayPoint,speed * Time.deltaTime);
                    champion.transform.LookAt(currentWayPoint);
                    yield return null;
                }
            }
        }else {
            CheckNearestOpponent();
        }
    }
    IEnumerator FollowDynamicPath() {
        if(path.Length > 0) {
            animator.SetTrigger("Walk");
            Vector3 currentWayPoint = path[0];
            champion.QuadStateChange(currentWayPoint);
            while(true) {
                if(champion.transform.position == currentWayPoint) {
                    champion.transform.LookAt(currentWayPoint);
                    CheckNearestOpponent();
                    yield break;
                }else {
                    champion.transform.position = Vector3.MoveTowards(champion.transform.position,currentWayPoint,speed * Time.deltaTime);
                    champion.transform.LookAt(currentWayPoint);
                    yield return null;
                }
            }
        }else {
            CheckNearestOpponent();
        }
    }
}
public class ChampionAttack: StateBase<ChampionState> {
    private Animator animator;
    public Champion targetChampion;
    private Champion champion;
    public ChampionAttack(Champion champion, Animator animator, bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
        this.champion = champion;
    }
    public void UpdateTargetChampion(Champion target) {
        targetChampion = target;
        champion.transform.LookAt(targetChampion.transform);
    }
    public void HitTarget() {
        champion.additionalAttackEffect?.Invoke(champion,targetChampion);
        targetChampion.OnHit(champion);
        champion.GainMana(champion.CurrentChampionStats.manaGainedPerAttack.Value);
    }
    public override void OnEnter() {
        animator.SetTrigger("Attack");
        animator.speed = champion.CurrentChampionStats.attackSpeed.Value;
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}
public class ChampionCastAbility : StateBase<ChampionState>
{
    private Animator animator;
    public Champion targetChampion;
    private Champion champion;
    public ChampionCastAbility(Champion champion, Animator animator, bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
        this.champion = champion;
    }
    

    public override void OnEnter() {
        base.OnEnter();
    }
    public override void OnExit() {
        base.OnExit();
    }
    public override void OnLogic() {
        base.OnLogic();
    }
}
public class ChampionDead: StateBase<ChampionState> {
    private Animator animator;
    private Champion champion;
    public ChampionDead(Champion champion, Animator animator,bool needsExitTime) : base(needsExitTime) {
        this.animator = animator;
        this.champion = champion;
    }
    public override void OnEnter() {
        champion.LastQuadThisChampionStand.OnChampionLeave(champion);
        champion.StopAllCoroutines();
        champion.gameObject.SetActive(false);
        champion.IsActive = false;
    }
    public override void OnLogic() {
        
    }
    public override void OnExit() {
        
    }
}