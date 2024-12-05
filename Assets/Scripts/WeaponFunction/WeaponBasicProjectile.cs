using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProjectileShooter : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;  // The pivot point where the projectile will be shot from (assigned in the Inspector)
    public GameObject projectilePrefab;  // The projectile prefab to be pooled (assigned in the Inspector)
    public Transform target;  // The target the projectile will move towards (assigned in the Inspector)
    public Transform poolParent;  // The parent GameObject for the object pool (assigned in the Inspector)

    [Header("Settings")]
    public float projectileSpeed = 10f;  // Speed at which the projectile will travel
    public int poolSize = 10;  // Number of projectiles to keep in the pool
    public float projectileLifeTime;  // Time before the projectile disappears and is returned to the pool
    public float shootingCooldown;  // The cooldown time between shots (in seconds)

    private Queue<GameObject> projectilePool;  // A pool of inactive projectiles

    private void Start()
    {
        shootingCooldown = GetWeaponByName().currentCooldown;

        // Initialize the pool of projectiles
        projectilePool = new Queue<GameObject>();

        // Create the pool of projectiles and assign them as children of poolParent
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, poolParent);
            projectile.SetActive(false);  // Deactivate projectiles at start
            projectilePool.Enqueue(projectile);  // Add to the pool
        }
        ShootProjectileWithCooldown();
    }
    public Weapon GetWeaponByName()
    {
        Weapon foundWeapon = GameManager.Instance.playerData.weaponInventory
            .FirstOrDefault(weapon => weapon.weaponName == "Gun");

        if (foundWeapon != null)
        {
            Debug.Log("Weapon found: " + foundWeapon.weaponName);
            return foundWeapon;
        }
        else
        {
            Debug.Log("Weapon not found.");
            return null;
        }
    }

    // Call this function to start the shooting process from another script
    public void ShootProjectileWithCooldown()
    {
        StartCoroutine(ShootProjectileCoroutine());
    }

    // Coroutine to handle the cooldown and shooting
    private IEnumerator ShootProjectileCoroutine()
    {
        while (true)
        {
            // Check if there is an available projectile in the pool
            if (projectilePool.Count > 0)
            {
                // Get a projectile from the pool
                GameObject projectile = projectilePool.Dequeue();
                projectile.SetActive(true);  // Activate the projectile

                // Position the projectile at the fire point
                projectile.transform.position = firePoint.position;

                // Calculate the direction towards the target
                Vector3 direction = (target.position - firePoint.position).normalized;

                // Set the velocity of the projectile
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();  // Assuming the projectile has a Rigidbody2D
                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed;
                }

                // Start the life timer for this projectile
                StartCoroutine(ProjectileLifeTimer(projectile));

                // Wait for the cooldown time before allowing another shot
                yield return new WaitForSeconds(shootingCooldown);
            }
            else
            {
                Debug.LogWarning("No available projectiles in the pool!");
            }
        }
    }

    // Coroutine to handle the lifetime of the projectile
    private IEnumerator<WaitForSeconds> ProjectileLifeTimer(GameObject projectile)
    {
        // Wait for the specified lifetime
        yield return new WaitForSeconds(projectileLifeTime);

        // Deactivate the projectile and return it to the pool
        ReturnProjectileToPool(projectile);
    }

    // Function to return the projectile to the pool when it's no longer needed
    public void ReturnProjectileToPool(GameObject projectile)
    {
        projectile.SetActive(false);  // Deactivate the projectile
        projectilePool.Enqueue(projectile);  // Add it back to the pool
    }
}
