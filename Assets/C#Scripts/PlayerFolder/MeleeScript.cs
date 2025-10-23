using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeScript : MonoBehaviour
{
    [Header("UŒ‚İ’è")]
    [SerializeField] public float attackRange = 2.0f; // UŒ‚”ÍˆÍ
    [SerializeField] float capsuleRadius = 0.7f;
    [SerializeField]private float nextAttackTime = 0.5f; // Ÿ‚ÉUŒ‚‚Å‚«‚éŠÔ
    [SerializeField] LayerMask targetMask;

    PlayerStateScript stats;
    private bool canAttack = false;
    private Transform cam;


    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<PlayerStateScript>();
        cam = Camera.main.transform;
    }

    public void OnMeleeAttack(InputValue value)
    {
        if (!value.isPressed || !canAttack) return;
        StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        canAttack = false;
        //UŒ‚ˆ—
        Vector3 start = transform.position + Vector3.up*1.0f;//ƒLƒƒƒ‰ƒNƒ^[‚Ì‹¹“–‚½‚è
        Vector3 dir = cam.forward;

        if(Physics.SphereCast(start,1.0f,dir,out RaycastHit hit, attackRange,targetMask))
        {
            Debug.Log($"Melee {hit.collider.name}");
            var target = hit.collider.GetComponent<EnemyHealthScript>();
            if (target != null)
            {
                target.TakeDamage(stats.AttackPower);
            }
        }
        // Ÿ‚ÌUŒ‚‚Ü‚Å‚ÌƒN[ƒ‹ƒ_ƒEƒ“
        yield return new WaitForSeconds(nextAttackTime);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
     Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up*1.2f, capsuleRadius);
    }

}
