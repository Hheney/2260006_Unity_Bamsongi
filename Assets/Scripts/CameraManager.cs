using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;

    [SerializeField] private CinemachineCamera camDefault = null;       //기본 카메라
    [SerializeField] private CinemachineCamera camZoomTarget = null;    //줌 카메라
    [SerializeField] private CinemachineCamera camTrajectory = null;    //궤적 카메라

    float fWaitTime = 2.0f;

    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("CameraManager is null.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this; //this : 현재 인스턴스를 가리키는 레퍼런스
        }
        else if (_instance != this)
        {
            Debug.Log("CameraManager has another instance.");

            Destroy(gameObject); //현재 인스턴스 파괴(GameManger Object)
        }
        DontDestroyOnLoad(gameObject); //씬이 변경되어도 현재 게임 오브젝트를 유지시키는 메소드
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void f_MoveCameraRoutine()
    {
        StartCoroutine(CameraMoveSequence());
    }

    private IEnumerator CameraMoveSequence()
    {
        f_SetCameraPriority(camZoomTarget); //줌 카메라 활성화
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camTrajectory); //궤적 카메라 활성화
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camDefault); //기본 카메라 활성화
    }

    private void f_SetCameraPriority(CinemachineCamera camera)
    {
        camDefault.Priority = 10;       //기본 카메라 우선순위 기본상태로 변경
        camZoomTarget.Priority = 10;    //줌 카메라 우선순위 기본상태로 변경
        camTrajectory.Priority = 10;    //궤적 카메라 우선순위 기본상태로 변경

        camera.Priority = 20;   //매개변수 카메라 최우선순위 변경 및 활성화
    }
}
