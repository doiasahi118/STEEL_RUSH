using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.InputSystem.DefaultInputActions;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed ; //移動速度
    [SerializeField] private float jumpHeight ;//ジャンプの高さ
    [HideInInspector] public float SpeedMultiplier = 1f;
    private CharacterController characterController;//キャラクターコントローラーのキャッシュ
    private Transform _transform; //transformのキャッシュ
    private Animator animator;
    private Vector3 _moveVelocity; //キャラの移動情報
    private InputAction _move; //移動アクション
    private InputAction _jump; //ジャンプアクション
    public Vector3 LastMoveDir { get; private set; }=Vector3.forward;
   PlayerStateScript state;

    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<PlayerStateScript>();
        characterController = GetComponent<CharacterController>();
     //毎フレームアクセスするので、負荷を下げるためにキャッシュしておく
     _transform = transform;//Transformもキャッシュすると少しだけ負荷が下がる
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false; //RootMotionを無効にする

        var input = GetComponent<PlayerInput>();
    //Playerインプットのアクションマップから、移動とジャンプのアクションを取得する
    input.currentActionMap.Enable();
    _move = input.currentActionMap.FindAction("Move");
    _jump = input.currentActionMap.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mv = _move.ReadValue<Vector2>();
        //Camera基準の移動方向
        Transform cam = Camera.main.transform;
        Vector3 camForward = Vector3.Scale(cam.forward,new Vector3(1,0,1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right,new Vector3(1,0,1)).normalized;
        Vector3 dir = camForward*mv.y+camRight*mv.x;
        if(dir.sqrMagnitude > 1f) dir.Normalize();

        //移動
        float baseSpeed = state ? state.MoveSpeed : moveSpeed;
        Vector3 horizontal = dir*(baseSpeed* SpeedMultiplier);
        _moveVelocity.x = horizontal.x;
        _moveVelocity.z = horizontal.z;

        //移動方向に向く
        if(dir.sqrMagnitude > 0.0001f)
        {
            Quaternion target =Quaternion.LookRotation(dir,Vector3.up);
            _transform.rotation = Quaternion.Slerp(_transform.rotation,target,Time.deltaTime*10f);
        }
       
        ////移動方向に向く
        //_transform.LookAt(_transform.position + new Vector3(_moveVelocity.x, 0, _moveVelocity.z));
        if (characterController.isGrounded)
        {
            if (_jump.WasPressedThisFrame())
            {
                float h = state ? state.jumpHeight : jumpHeight;
                _moveVelocity.y = Mathf.Sqrt(2f * -Physics.gravity.y * h);
            }
        }
        else
        {
            //重力の計算
            _moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        //オブジェクトを動かす
        characterController.Move(_moveVelocity * Time.deltaTime);

        //animatorへ連動
        float playerSpeed = new Vector2(_moveVelocity.x,_moveVelocity.z).magnitude;
        animator.SetFloat("MoveSpeed",playerSpeed,0.15f,Time.deltaTime);

        if(mv.sqrMagnitude>0.0001f)
        {
            LastMoveDir = dir.normalized;
        }

    }
}
