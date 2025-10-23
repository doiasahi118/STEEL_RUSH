using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class LookOnActionScript : MonoBehaviour
{
    [Header("特定注視")]

    [SerializeField] float lockOnRange = 80.0f;
   [SerializeField] Transform lockOnTarget; // 注視対象

    List<Transform> enemies = new List<Transform>();
    int currentIndex = -1;

    public bool IsLookedOn { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        //scene内のEnemyタグを持つオブジェクトを全て取得し、enemiesリストに追加
        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
        enemies.Add(e.transform) ;

    }

    public void SwitchControl()
    {
        if(IsLookedOn)
        {
            IsLookedOn = false;
            lockOnTarget = null;
            Debug.Log("[LookOn]OFF");
        }

        else
        {
            FindNearesTarget();
            IsLookedOn = true;
            Debug.Log("[LookOn]ON"); 
        }
    }
    public void CycleNext()
    {
        if(!IsLookedOn||enemies.Count==0)return;
        {
            currentIndex = (currentIndex + 1) % enemies.Count;
            lockOnTarget = enemies[currentIndex];
            Debug.Log("[LookOn]Next:");
        }
    }

    void FindNearesTarget()
    {
        float minDist = Mathf.Infinity;
        Transform best = null;
        
        foreach(var e in enemies)
        {
            float dist = Vector3.Distance(transform.position,e.position);
            if (dist < minDist && dist <= lockOnRange)
            {
                minDist = dist;
                best = e;
            }
        }
        lockOnTarget = best;
        currentIndex = enemies.IndexOf(best);
    }

    public Transform GetTarget()=> lockOnTarget;
}
