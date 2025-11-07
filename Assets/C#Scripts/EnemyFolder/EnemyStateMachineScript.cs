using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エネミーのステート
public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Dead
}

public class EnemyStateMachineScript : MonoBehaviour
{
    public EnemyState currentState;
    public Transform player;
    public float chaseDistance = 0;
    public float attackDistance = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
