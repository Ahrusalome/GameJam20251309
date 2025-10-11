using Unity.Cinemachine;
using UnityEngine;

public class CinemachineMovements : MonoBehaviour
{
    [SerializeField] float zoomFactor;
    private CinemachineCamera cinemachineCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (cinemachineCamera.Lens.OrthographicSize <= 5.9)
            cinemachineCamera.Lens.OrthographicSize += zoomFactor;
    }
}
