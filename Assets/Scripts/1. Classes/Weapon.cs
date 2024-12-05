using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public int level;
    public float currentDamage => baseDamage * damageModifier;
    public float baseDamage;
    public float damageModifier;
    public float currentCooldown => baseCooldown * cooldownModifier;
    public float baseCooldown;
    public float cooldownModifier;
    public float currentMaxHits => baseMaxHits * maxHitsModifier;
    public float baseMaxHits;
    public float maxHitsModifier;
}
