using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public class BulletScript : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] float speed = 40.0f;
    [SerializeField] float lifeTime = 3.0f;
    [SerializeField] int damage = 30;
    [SerializeField] LayerMask hittableLayers = ~0;//何に当たるか
    [SerializeField] GameObject hitEffect;         //命中エフェクト

    Rigidbody rb;
    Collider col;
    Vector3 prevPos;
    bool hitPos;
    bool hitOnce; //二重ヒット防止
    GameObject owner; //発射元(Playerなど)

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        //物理設定
        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        //弾側はTriggerにしておくと使いやすい(相手側は非Trigger推奨)
        col.isTrigger = true;
    }


    void OnEnable()
    {
        hitOnce = false;
        prevPos = transform.position;
        //前方へ一定速度
        rb.velocity = transform.forward * speed;
        //寿命
        Invoke(nameof(Despawn), lifeTime);
    }

    void Update()
    {
        prevPos = transform.position;//次フレームの前位置用
    }

    void OnDisable()
    {
        CancelInvoke(nameof(Despawn));
    }

    public void Init(GameObject obj)
    {
        owner = obj;
        //発射元と自弾の衝突を無効化(Playerや銃のコライダー)
        if (!owner)
        {
            return;
        }
        var ownerCols = owner.GetComponentsInChildren<Collider>(true);
        foreach (var c in ownerCols)
        {
            if (c && col)
            {
                Physics.IgnoreCollision(col, c, true);
            }
        }
    }
    void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hitOnce)
        {
            return;
        }

        if (!IsHittable(other.gameObject))
        {
            return;
        }

        if(owner&&other.transform.root == owner.transform.root)
        {
            return;
        }

        //命中情報作成
        Vector3 hitPoint = other.ClosestPoint(prevPos);
        Vector3 hitNormal = (-rb.velocity).normalized;

        var hit = new HitData(damage, hitPoint, hitNormal,owner? owner: gameObject);

        //受け手に伝える(IDamageable)
        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage(hit);
        }

        //視覚効果
        if (hitEffect)
        {
            Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(hitNormal));
        }

        hitOnce = true;
        Despawn();
    }

    bool IsHittable(GameObject go)
    {
        //レイヤーフィルター
        int mask = 1 <<go.layer;
        return (hittableLayers.value & mask) != 0;
    }

    public void SrtDamage(int value)=>damage = value;
    public void SetSpeed(float value)
    {
        speed = value;
        if (rb)
        {
            rb.velocity = transform.forward * speed;
        }
    }
}   
