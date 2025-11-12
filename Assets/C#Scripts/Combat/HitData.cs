using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HitData
{
    public int damage;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public GameObject attacker;

    public HitData(int dmg ,Vector3 point, Vector3 normal, GameObject source)
    {
        damage = dmg;
        hitPoint = point;
        hitNormal = normal;
        attacker = source;
    }
}
