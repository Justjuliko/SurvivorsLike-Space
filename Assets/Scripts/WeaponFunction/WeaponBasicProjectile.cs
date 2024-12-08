using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProjectileShooter : MonoBehaviour
{
    [Header("References")]
    public string weaponName = "Gun";
    public Transform firePoint; // The starting point of the projectile
    public GameObject projectilePrefab; // The prefab of the projectile
    public Transform target; // The target the projectile will move towards
    public Transform poolParent; // Parent object for pooled projectiles

    [Header("Settings")]
    public int poolSize = 10; // Number of projectiles in the pool
    public float projectileSpeed = 1f; // Speed at which the projectile travels
    public float projectileLifeTime = 1f; // Maximum lifetime of the projectile
    public float shootingCooldown = 1f; // Time between each shot
    public float maxHits = 1f; // Maximum number of hits a projectile can register before being returned to the pool
    public float projectileArea = 1f; // The size of the projectile

    private Queue<GameObject> projectilePool; // A pool of inactive projectiles

    private void Start()
    {
        GetWeaponAttributes();

        // Initialize the projectile pool
        projectilePool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, poolParent);
            projectile.SetActive(false); // Start inactive
            projectilePool.Enqueue(projectile);
        }

        // Start the shooting coroutine
        ShootProjectileWithCooldown();
    }

    // Retrieve weapon attributes and apply them
    public void GetWeaponAttributes()
    {
        Weapon weapon = GetWeaponByName();
        if (weapon != null)
        {
            shootingCooldown = weapon.currentCooldown;
            projectileSpeed = weapon.currentSpeed;
            maxHits = weapon.currentMaxHits;
            projectileArea = weapon.currentArea;
        }
    }

    // Find the weapon by its name in the player's inventory
    public Weapon GetWeaponByName()
    {
        Weapon foundWeapon = GameManager.Instance.playerData.weaponInventory
            .FirstOrDefault(weapon => weapon.weaponName == weaponName);

        if (foundWeapon != null)
        {
            Debug.Log("Weapon found: " + foundWeapon.weaponName);
            return foundWeapon;
        }
        else
        {
            Debug.LogWarning("Weapon not found.");
            return null;
        }
    }

    // Start the shooting coroutine
    public void ShootProjectileWithCooldown()
    {
        StartCoroutine(ShootProjectileCoroutine());
    }

    // Coroutine for handling shooting and cooldowns
    private IEnumerator ShootProjectileCoroutine()
    {
        while (true)
        {
            if (projectilePool.Count > 0)
            {
                GameObject projectile = projectilePool.Dequeue();
                projectile.SetActive(true);

                // Set the size of the projectile
                projectile.transform.localScale = new Vector3(projectileArea, projectileArea, 1);

                // Set the position and direction of the projectile
                projectile.transform.position = firePoint.position;
                Vector3 direction = (target.position - firePoint.position).normalized;

                // Assign velocity to the projectile
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed;
                }

                // Start handling lifetime and hits
                StartCoroutine(HandleProjectileLifetime(projectile, maxHits));

                yield return new WaitForSeconds(shootingCooldown);
            }
            else
            {
                Debug.LogWarning("No available projectiles in the pool!");
                yield return null;
            }
        }
    }

    // Coroutine to handle projectile lifetime and maximum hits
    private IEnumerator HandleProjectileLifetime(GameObject projectile, float initialHits)
    {
        int remainingHits = Mathf.CeilToInt(initialHits);
        float timer = projectileLifeTime;

        // Loop until the projectile expires or uses all its hits
        while (timer > 0 && remainingHits > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // Return the projectile to the pool when its lifetime ends or it runs out of hits
        ReturnProjectileToPool(projectile);
    }

    // Return the projectile to the pool and reset its state
    public void ReturnProjectileToPool(GameObject projectile)
    {
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset the velocity
        }

        projectile.SetActive(false); // Deactivate the projectile
        projectilePool.Enqueue(projectile); // Add it back to the pool
    }
}
