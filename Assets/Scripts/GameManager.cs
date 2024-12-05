using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData playerData;
    public PlayerInputActions playerInput;

    [Header("Initial Prefabs")]
    public List<GameObject> resourcePrefabs;   // Prefabs for initial resources
    public List<GameObject> weaponPrefabs;    // Prefabs for initial weapons
    public List<GameObject> equipmentPrefabs; // Prefabs for initial equipment

    private string saveFilePath;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Define the save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");

        // Initialize player data, either loading from save or creating new data
        InitializePlayerData();

        // Initialize Input System
        InitializeInputSystem();
    }

    /// <summary>
    /// Initializes player data by either loading saved data or creating new default data.
    /// </summary>
    private void InitializePlayerData()
    {
        if (File.Exists(saveFilePath))
        {
            LoadGame();
        }
        else
        {
            CreateNewPlayerData();
        }
    }

    /// <summary>
    /// Creates a new instance of PlayerData with default values and initializes starting inventories.
    /// </summary>
    private void CreateNewPlayerData()
    {
        playerData = new PlayerData
        {
            currentLevel = 1,
            baseHealth = 100f,
            eventHealthModifier = 1f,
            baseMovementSpeed = 5f,
            eventMovementSpeedModifier = 1f,
            baseDamage = 10f,
            eventDamageModifier = 1f,
            baseEscapeTimer = 60f,
            eventEscapeTimerModifier = 1f,
            resourceInventory = new List<Resource>(),
            weaponInventory = new List<Weapon>(),
            equipmentInventory = new List<Equipment>()
        };

        // Initialize resource inventory with initial prefabs
        foreach (var prefab in resourcePrefabs)
        {
            Resource resource = Instantiate(prefab).GetComponent<Resource>();
            if (resource != null)
            {
                playerData.resourceInventory.Add(resource);
            }
        }

        // Initialize weapon inventory with initial prefabs
        foreach (var prefab in weaponPrefabs)
        {
            Weapon weapon = Instantiate(prefab).GetComponent<Weapon>();
            if (weapon != null)
            {
                playerData.weaponInventory.Add(weapon);
            }
        }

        // Initialize equipment inventory with initial prefabs
        foreach (var prefab in equipmentPrefabs)
        {
            Equipment equipment = Instantiate(prefab).GetComponent<Equipment>();
            if (equipment != null)
            {
                playerData.equipmentInventory.Add(equipment);
            }
        }

        // Assign the first weapon and equipment as the default equipped items
        if (playerData.weaponInventory.Count > 0)
        {
            playerData.equippedWeapon = playerData.weaponInventory[0];
        }

        if (playerData.equipmentInventory.Count > 0)
        {
            playerData.equippedEquipment = playerData.equipmentInventory[0];
        }

        Debug.Log("New PlayerData created with default equipped weapon and equipment.");
        Debug.Log(playerData);
    }

    /// <summary>
    /// Saves the player's data to a JSON file.
    /// </summary>
    public void SaveGame()
    {
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Game saved to {saveFilePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save game: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the player's data from a JSON file.
    /// </summary>
    public void LoadGame()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                playerData = JsonUtility.FromJson<PlayerData>(json);

                Debug.Log("Game Loaded.");
            }
            else
            {
                Debug.LogWarning("Save file not found. Creating new player data.");
                CreateNewPlayerData();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load game: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes the saved game file, resetting the game to a new state.
    /// </summary>
    public void DeleteSave()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Save Deleted.");
            }
            else
            {
                Debug.LogWarning("No save file to delete.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to delete save: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes the Input System and enables input actions.
    /// </summary>
    private void InitializeInputSystem()
    {
        playerInput = new PlayerInputActions();
        playerInput.Enable();
    }

    /// <summary>
    /// Provides access to specific input actions.
    /// </summary>
    public PlayerInputActions GetInputActions()
    {
        return playerInput;
    }
}
