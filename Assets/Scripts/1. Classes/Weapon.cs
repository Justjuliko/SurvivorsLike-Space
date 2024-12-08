using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Properties")]
    public string weaponName;          // Name of the weapon
    public int level;                  // Current level of the weapon
    public int maxLevel = 5;           // Maximum level the weapon can reach

    public float baseDamage;           // Base damage value
    public float damageModifier = 1f;  // Damage multiplier
    public float currentDamage => baseDamage * damageModifier;

    public float baseCooldown;         // Base cooldown value
    public float cooldownModifier = 1f;// Cooldown multiplier
    public float currentCooldown => baseCooldown * cooldownModifier;

    public float baseMaxHits;          // Base maximum hits
    public float maxHitsModifier = 1f; // Hits multiplier
    public float currentMaxHits => baseMaxHits * maxHitsModifier;

    public float baseSpeed;          // Base speed
    public float speedModifier = 1f; // Speed multiplier

    public float currentSpeed => baseMaxHits * maxHitsModifier;

    public float baseArea;          // Base area
    public float AreaModifier = 1f; // Area multiplier

    public float currentArea => baseMaxHits * maxHitsModifier;

    // Dictionary to store level-specific upgrade actions
    private Dictionary<int, Action<Weapon>> levelUpgrades;

    private void Start()
    {
        // Initialize the level upgrades based on the weapon type
        InitializeWeaponUpgrades();
    }

    /// <summary>
    /// Defines the level-specific upgrades based on the weapon type.
    /// </summary>
    private void InitializeWeaponUpgrades()
    {
        levelUpgrades = new Dictionary<int, Action<Weapon>>();

        // Customize upgrades based on weapon name
        switch (weaponName.ToLower())
        {
            case "pistol":
                levelUpgrades[1] = weapon => weapon.baseDamage += 5;  // Level 1: Increase base damage
                levelUpgrades[2] = weapon => weapon.cooldownModifier -= 0.1f; // Level 2: Reduce cooldown
                levelUpgrades[3] = weapon => weapon.maxHitsModifier += 0.2f; // Level 3: Increase max hits
                break;

            case "shotgun":
                levelUpgrades[1] = weapon => weapon.cooldownModifier -= 0.15f; // Level 1: Reduce cooldown
                levelUpgrades[2] = weapon => weapon.baseDamage += 10;         // Level 2: Increase damage
                levelUpgrades[3] = weapon => weapon.maxHitsModifier += 0.3f; // Level 3: Increase max hits
                break;

            // Add more cases for other weapon types
            default:
                Debug.LogWarning($"No specific upgrades defined for weapon '{weaponName}'. Using default upgrades.");
                levelUpgrades[1] = weapon => weapon.baseDamage += 3;  // Default upgrade for level 1
                levelUpgrades[2] = weapon => weapon.cooldownModifier -= 0.05f; // Default upgrade for level 2
                break;
        }
    }

    /// <summary>
    /// Attempts to level up the weapon.
    /// </summary>
    public void LevelUp()
    {
        if (level < maxLevel)
        {
            level++;

            if (levelUpgrades.TryGetValue(level, out var upgradeAction))
            {
                // Apply the upgrade for this level
                upgradeAction.Invoke(this);
                Debug.Log($"Weapon '{weaponName}' leveled up to {level}. Upgrade applied.");
            }
            else
            {
                Debug.LogWarning($"No upgrade defined for level {level} on weapon '{weaponName}'.");
            }
        }
        else
        {
            Debug.Log($"Weapon '{weaponName}' is already at max level!");
        }
    }
}
