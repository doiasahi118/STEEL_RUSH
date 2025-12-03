using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    //HitDataは前に作成された構造体(ダメージ情報をまとめるデータ)
    void ApplyDamage(HitData hit);
}
