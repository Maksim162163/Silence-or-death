using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneController : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioClip _audioClip;
    [SerializeField] private bool _useMicrophone;
    private string _selectedDevice;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                _selectedDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
            }
            else
            {
                _useMicrophone = false;
            }
        }
        if (!_useMicrophone)
        {
            _audioSource.clip = _audioClip;
        }

        _audioSource.Play();
    }

    public void TurnOnMicrophone()
    {
        _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
        _audioSource.Play();
    }

    public void TurnOffMicrophone()
    {
        Microphone.End(_selectedDevice);
    }
}
