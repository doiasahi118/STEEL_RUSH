using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShotScript : MonoBehaviour
{
    [Header("必須参照")]
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject bulletPrefab;

    [Header("射撃設定")]
    [SerializeField,Tooltip("1秒間の発射数")] float fireRate = 8f;
    [SerializeField,Tooltip("弾速")] float bulletSpeed = 60f;
    [SerializeField,Tooltip("照準例キャスト距離")] float aimCastDistance = 2000f;
    [SerializeField, Tooltip("弾のばらつき角度")] float spreadAngle = 0f;

    [Header("弾倉/リロード")]
    [SerializeField,Tooltip("弾倉容量")] int magazineSize = 30;
    [SerializeField,Tooltip("リロード時間")] float reloadTime = 2f;

    [Header("エフェクト設定")]
    [SerializeField] GameObject muzzleFlashEffect; //マズルフラッシュ
    [SerializeField] AudioClip fireSound; //発射音(予定)
    [SerializeField] float effectLifeTime =0.2f; //エフェクトの生存時間
    //AudioSource audioSource; 

    int currentAmmo;
    bool isReloading;

    //内部状態
    bool isFiring;
    float fireInterval; //1秒あたりの発射数の感覚
    float fireTimer; //次弾発射までのタイマー

    private void Awake()
    {
        if (!muzzle) muzzle = transform;//念のため
        fireInterval = 1f / Mathf.Max(0.01f, fireRate);
        currentAmmo = magazineSize;

    }

    void Update()
    {
        if (isReloading||!isFiring) return;
        //押しっぱなしで連射
        if (!isFiring) return;

        fireTimer += Time.deltaTime;
        while(fireTimer>=fireInterval)
        {
            fireTimer -= fireInterval;
            if(!TryFireOnce())
            {
                //弾切れ->自動リロード(好みで無効にしてOK)
                if(currentAmmo<= 0)StartCoroutine(ReloadRoutine());
                break;
            }
        }
    }

    bool TryFireOnce()
    {
        if (currentAmmo <= 0) return false;
        //照準方向
        var cam = Camera.main;
        Vector3 dir = cam ? GetAimDirectionFromCamera(cam) : muzzle.forward;

        //バラつき
        if (spreadAngle > 0f)
        {
            dir = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle), 0f) * dir;
        }
        //重工の向きも併せたい場合
        muzzle.rotation = Quaternion.LookRotation(dir, Vector3.up);
        //弾の生成
        var go = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(dir));
        var rb = go.GetComponent<Rigidbody>();
        if(rb)
        {
            rb.velocity = dir * bulletSpeed;
        }
        currentAmmo--;
        //エフェクト
        if(muzzleFlashEffect!=null)
        {
            var flash = Instantiate(muzzleFlashEffect, muzzle.position, muzzle.rotation, muzzle);
            Destroy(flash, effectLifeTime);
        }
        return true;
    }
    Vector3 GetAimDirectionFromCamera(Camera cam)
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if(Physics.Raycast(ray,out RaycastHit hit, aimCastDistance))
        {
            return (hit.point - muzzle.position).normalized;
        }
        else
        {
            Vector3 far = ray.origin + ray.direction * aimCastDistance;
            return (far - muzzle.position).normalized;
        }
    }

    public void StartFire()
    {
        Debug.Log("StartFire called");
        if (isReloading) return;
        isFiring = true;
    }
    public void StopFire()
    {
        isFiring = false;
    }

    public void Reload()
    {
        if(isReloading)
        {
            return;
        }

        if(currentAmmo==magazineSize)
        {
            return;
        }

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        isFiring = false;
        //リロードエフェクト
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
    }

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloadingNow => isReloading;
}
