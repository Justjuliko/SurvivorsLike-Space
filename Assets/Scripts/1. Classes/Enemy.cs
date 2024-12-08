using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float currentHealth => baseHealth * eventHealthModifier;
    public float baseHealth;
    public float eventHealthModifier;
    public float speed;
    public float maxDistance = 20f; // Maximum distance before deactivating
    private Transform playerTransform; // The player's transform (to be found dynamically)

    private Coroutine distanceCheckCoroutine; // To keep track of the coroutines

    // Delegate and event to notify when the enemy is deactivated
    public delegate void EnemyDeactivateDelegate(GameObject enemy);
    public event EnemyDeactivateDelegate OnEnemyDeactivate;

    private void OnEnable()
    {
        // When the enemy is enabled, find the player and start the coroutine
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        // Start the distance check coroutine when the enemy is enabled
        if (distanceCheckCoroutine == null)
        {
            distanceCheckCoroutine = StartCoroutine(CheckDistanceCoroutine());
        }
    }

    private void OnDisable()
    {
        // Stop the coroutine when the enemy is disabled
        if (distanceCheckCoroutine != null)
        {
            StopCoroutine(distanceCheckCoroutine);
            distanceCheckCoroutine = null; // Ensure we don't restart the coroutine accidentally
        }

        // Trigger the OnEnemyDeactivate event when the enemy is deactivated
        OnEnemyDeactivate?.Invoke(gameObject);
    }

    // Coroutine that checks the distance periodically
    private IEnumerator CheckDistanceCoroutine()
    {
        while (true)
        {
            // Check the distance between the enemy and the player
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            // If the enemy is too far from the player, deactivate it
            if (distance > maxDistance && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }

            // Wait before checking again (this could be adjusted as needed)
            yield return new WaitForSeconds(1f); // Adjust time as needed
        }
    }
}
