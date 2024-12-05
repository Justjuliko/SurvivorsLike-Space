using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private GameObject aimingPivot;
    private InputAction aimInputAction;
    [SerializeField] private float rotationSpeed = 5f;

    private void Start()
    {
        aimInputAction = GameManager.Instance.GetInputActions().Player.Aim;
    }

    private void Update()
    {
        Vector2 aimDirection = aimInputAction.ReadValue<Vector2>();

        if (aimDirection.magnitude > 0.1f)
        {
            RotatePlayer(aimDirection);
        }
    }

    private void RotatePlayer(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        targetAngle -= 90;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));

        aimingPivot.transform.rotation = Quaternion.RotateTowards(aimingPivot.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
