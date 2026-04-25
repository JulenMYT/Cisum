using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private List<CinemachineVirtualCamera> virtualCameras;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SwitchToCamera(int cameraIndex)
    {
        if (cameraIndex < 0 || cameraIndex >= virtualCameras.Count)
        {
            Debug.LogError($"Camera index {cameraIndex} is out of range.");
            return;
        }

        for (int i = 0; i < virtualCameras.Count; i++)
        {
            virtualCameras[i].Priority = (i == cameraIndex) ? 10 : 0;
        }
    }
    public void SwitchToCamera(string cameraName)
    {
        foreach (var cam in virtualCameras)
        {
            cam.Priority = (cam.name == cameraName) ? 10 : 0;
        }
    }
}
