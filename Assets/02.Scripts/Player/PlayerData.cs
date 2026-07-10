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
}
