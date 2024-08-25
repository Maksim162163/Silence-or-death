using UnityEngine;

[RequireComponent(typeof(MicrophoneController))]
public class KeyboardForCommunication : MonoBehaviour
{
    private MicrophoneController _usingMicrophone;

    private void Awake()
    {
        _usingMicrophone = GetComponent<MicrophoneController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _usingMicrophone.TurnOnMicrophone();
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            _usingMicrophone.TurnOffMicrophone();
        }
    }
}
