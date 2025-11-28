using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

//定数クラス
public static class CommonConst
{

}

//エネミーのステート
public enum EnemyState
{
    Patrol,
    Chase,
    Shot,
    Dead
}


[RequireComponent(typeof(EnemyBaseScript))]
public class EnemyStateMachineScript : MonoBehaviour
{
    [Header("ターゲット")]
    [SerializeField] Transform player;

    [Header("距離しきい値")]
    [SerializeField,Tooltip("この距離内で追跡開始")]
    public float chaseDistance = 15.0f;
    [SerializeField,Tooltip("この距離内で攻撃開始")]
    float attackDistance = 30.5f;
    [SerializeField, Tooltip("攻撃のクールダウン")]
    float attackInterval = 1.0f;

    [Header("移動")]
    [SerializeField,Tooltip("NavMashAgentを使って追跡ができる(任意)")]
    bool useNavMesh = true;
    [SerializeField] float stopDistance = 1.0f;

    [Header("参照(任意)")]
    [SerializeField] EnemyMove move;
    [SerializeField] EnemyShot shot;
    [SerializeField] NavMeshAgent agent;

    EnemyBaseScript enemyBase;
    public EnemyState currentState { get; private set; } =EnemyState.Patrol;
   
    // Start is called before the first frame update
    void Awake()
    {
       enemyBase = GetComponent<EnemyBaseScript>();
       if(!move) move = GetComponent<EnemyMove>();
       if(!shot) shot = GetComponent<EnemyShot>();
       if(!agent) agent = GetComponent<NavMeshAgent>();
       if(shot&&player)shot.SetTarget(player);

       if(shot&&player)
        {
            ChangeState(EnemyState.Chase);
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        if (!player || !enemyBase.IsAlive)
        {
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);

        switch(currentState)
            {
            case EnemyState.Patrol:
                UpdatePatrol(dist);
                break;
            case EnemyState.Chase:
                UpdateChase(dist);
                break;
            case EnemyState.Shot:
                UpdateShot(dist);
                break;
            case EnemyState.Dead:
                //何もしない
                break;
        }
    }

    void UpdatePatrol(float dist)
    {
        //ここではまだうろつくだけ(今は何もしない)
        if(dist<=chaseDistance)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    void UpdateChase(float dist)
    {
        if(useNavMesh&&agent)
        {
            agent.isStopped = false;
            agent.stoppingDistance = stopDistance;
            agent.SetDestination(player.position);
        }

        if(dist<=attackDistance)
        {
            ChangeState(EnemyState.Shot);
        }
    }

    void UpdateShot(float dist)
    {
        //攻撃中はその場でPlayerのほうを向く
        if(useNavMesh && agent)
        {
            agent.isStopped = true;
        }
        //水平だけ向く
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPos);

        //距離が離れたら追いかけなおす
        if(dist>attackDistance*1.2f)//少し余裕を持たせる
        {
            ChangeState(EnemyState.Chase);
        }
    }

    public void ChangeState(EnemyState nextState)
    {
        if(currentState == nextState) 
        {
            return;
        }

        //Exit
        OnExit(currentState);

        //状態遷移
        currentState = nextState;

        //Enter
        OnEnter(currentState);
    }

    void OnEnter(EnemyState state)
    {
        switch(state)
        {
            case EnemyState.Chase:
                if(useNavMesh&&agent)agent.isStopped = false;
                break;
            case EnemyState.Shot:
                Debug.Log("[EnemyState] Enter Shot",this);
                shot?.StartAttack();
                break;
            case EnemyState.Dead:
                if(useNavMesh&&agent)agent.isStopped = true;
                break;
        }
    }

    void OnExit(EnemyState state)
    {
        switch(state)
        {
            case EnemyState.Shot:
                shot?.StopAttack();
                break;
        }
    }

    bool HasPlayer() => player != null;

    public void SetDistances(float chase, float attack)
    {
        chaseDistance = Mathf.Max(0, chaseDistance);
        attackDistance = Mathf.Clamp(attack,0.1f, attackDistance);
    }
    //便利メソッド
    public void SetPlayer(Transform t)
    {
        player = t;
        if(shot&&player)
        {
            shot.SetTarget(player);
        }
    }
}
