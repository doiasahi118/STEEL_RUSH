using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [Header("ターゲット")]
    [SerializeField] Transform target;          // 追いかける相手（プレイヤー）

    [Header("移動設定")]
    [SerializeField] float stoppingDistance = 2f;  // どこまで近づいたら止まるか
    [SerializeField] float rotateSpeed = 10f;      // 向きを合わせるスピード

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        if (!agent || !agent.enabled) return;

        // 目的地を常にターゲット位置に設定
        agent.SetDestination(target.position);

        // ターゲットの方向を向く（移動だけでよければ無くてもOK）
        Vector3 dir = agent.steeringTarget - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion t = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, t, Time.deltaTime * rotateSpeed);
        }
    }

    /// <summary>外部からターゲットを設定したいとき用</summary>
    public void SetTarget(Transform t)
    {
        target = t;
    }

    /// <summary>強制停止させたいとき</summary>
    public void Stop()
    {
        if (!agent) return;
        agent.isStopped = true;
        agent.ResetPath();
    }

    /// <summary>停止解除して再び移動可能に</summary>
    public void Resume()
    {
        if (!agent) return;
        agent.isStopped = false;
    }
}

