using System;
using UnityEngine;

[System.Serializable]
public class PlayerData : GameDataBase
{
    public string Name;
    public int MaxHp;
    public int MaxHunger;
    public int MaxThirst;
    public int AttackPower;
    public float MoveSpeed;
    public int HungerInterval;
    public int ThirstInterval;
    public int HungerDecrease;
    public int ThirstDecrease;
    public int HungerDamage;
    public int ThirstDamage;
}
