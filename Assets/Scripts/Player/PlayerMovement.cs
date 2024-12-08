using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameObject player;
    InputAction moveInputAction;
    private void Start()
    {
        moveInputAction = GameManager.Instance.GetInputActions().Player.Move;
    }
    private void Update()
    {
        Vector2 moveDirection = moveInputAction.ReadValue<Vector2>();
        player.transform.Translate(moveDirection * GameManager.Instance.playerData.currentMovementSpeed * Time.deltaTime);
    }
}
