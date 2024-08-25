using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform _playerBody;
    [SerializeField] private float _maxYAngle = 80f;
    [SerializeField] private float _mouseSensitivity = 2f;

    private float _yRotation = 0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _yRotation -= mouseY;
        _yRotation = Mathf.Clamp(_yRotation, -_maxYAngle, _maxYAngle);

        transform.localRotation = Quaternion.Euler(_yRotation, 0f, 0f);
        _playerBody.Rotate(_mouseSensitivity * mouseX * Time.deltaTime * Vector3.up);
    }
}
