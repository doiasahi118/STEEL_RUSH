using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{ 
    [SerializeField] float speed = 40.0f;
    [SerializeField] float lifeTime = 3.0f;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }


    void OnEnable()
    {
        //ëOï˚Ç÷àÍíËë¨ìx
        rb.velocity = transform.forward * speed;
        //éıñΩ
        Invoke(nameof(Despawn), lifeTime);
    }

    void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //è’ìÀéûÇ…è¡Ç¶ÇÈ
        if (!other.isTrigger)        {
            Destroy(gameObject);
        }
    }

}   
