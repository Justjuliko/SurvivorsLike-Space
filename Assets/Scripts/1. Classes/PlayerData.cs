using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currentLevel;

    public float currentHealth => baseHealth * eventHealthModifier;
    public float baseHealth;
    public float eventHealthModifier;

    public float currentMovementSpeed => baseMovementSpeed * eventMovementSpeedModifier;
    public float baseMovementSpeed;
    public float eventMovementSpeedModifier;

    public float currentDamage => baseDamage * eventDamageModifier;
    public float baseDamage;
    public float eventDamageModifier;

    public float currentEscapeTimer => baseEscapeTimer * eventEscapeTimerModifier;
    public float baseEscapeTimer;
    public float eventEscapeTimerModifier;

    public List<Resource> resourceInventory;
    public List<Weapon> weaponInventory;
    public List<Equipment> equipmentInventory;

    public Weapon equippedWeapon;
    public Equipment equippedEquipment;

}
