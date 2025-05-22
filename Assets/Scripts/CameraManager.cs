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
    //Coroutine : 일시 중단이 가능한 메소드, 여러 프레임에 걸쳐 작업을 나눠서 처리할 수 있도록 해주는 기능을 수행함
    public void f_MoveCameraRoutine()
    {
        StartCoroutine(CameraMoveSequence()); //CameraMoveSequence 실행
    }

    //IEnumerator : C#의 인터페이스로, 반복 가능한 구조(Enumerable)를 반환
    //기본적으로 Unity는 yield 문 다음에 프레임에 코루틴을 다시 시작한다
    private IEnumerator CameraMoveSequence()
    {
        //Blend가 끝나지 않은걸 확인하지 못하면 Priority는 변
        //중복 클릭과 클릭방지를 위해서 Blend 여부 판단 추가해야함
        //GameManager에서는 판단을 위한 Bool 필드를 get set으로 참조
        //BamsongiController 클래스내 전역변수로 CanClick 메소드를 생성하여 bool변수를 변동시킬 수 있도록 해야함

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
