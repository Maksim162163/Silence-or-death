using UnityEngine;

public class KeyboardForMovement : MonoBehaviour
{
    [SerializeField] private MovementController _movementController;

    private Vector3 _inputVector;

    private void Update()
    {
        _inputVector.x = Input.GetAxisRaw(Axis.Horizontal);
        _inputVector.z = Input.GetAxisRaw(Axis.Vertical);

        _movementController.MoveCharacter(_inputVector.normalized);
    }
}
