using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;

    [SerializeField] private CinemachineCamera camDefault = null;       //기본 카메라
    [SerializeField] private CinemachineCamera camZoomTarget = null;    //줌 카메라
    [SerializeField] private CinemachineCamera camTrajectory = null;    //궤적 카메라

    const int nDefaultPriority = 10;  //기본 우선순위
    const int nActivePriority = 20;   //활성화 우선순위

    private float fWaitTime = 2.0f;

    //------------------------[발사 금지 기능 구현]------------------------
    [SerializeField] private CinemachineBrain cinemachineBrain = null; //Blend 여부를 확인하기 위한 CinemachineBrain 컴포넌트를 불러오기 위함

    //델리게이트(delegate): 블렌드 완료 시 호출될 메소드 형식 정의
    public delegate void CameraBlendCompleteDelegate();
    
    //이벤트(event): 외부에서 구독할 수 있는 이벤트
    public event CameraBlendCompleteDelegate OnCameraBlendComplete;

    /// <summary>현재 카메라가 블렌드 상태가 아닌지 여부를 반환하는 프로퍼티</summary>
    public bool IsCameraReady
    {
        //기본 카메라가 활성화 상태 && cinemachineBrain가 null이 아님 && Blend 상태가 아닐 경우 IsCameraReady "true" 값 리턴
        get { return camDefault.Priority == 20 && cinemachineBrain != null && !cinemachineBrain.IsBlending; }
    }
    //------------------------[발사 금지 기능 구현]------------------------

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
        GameObject gCamera = GameObject.Find("Main Camera"); //메인 카메라 오브젝트 불러오기

        if(gCamera != null)
        {
            cinemachineBrain = gCamera.GetComponent<CinemachineBrain>(); //CinemachineBrain 기능 가져오기
        }
        else
        {
            Debug.LogWarning("Main Camera 오브젝트를 찾을 수 없습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>카메라 연출 시퀀스를 코루틴으로 실행하는 메소드</summary>
    public void f_MoveCameraRoutine()
    {
        //Coroutine : 일시 중단이 가능한 메소드, 여러 프레임에 걸쳐 작업을 나눠서 처리할 수 있도록 해주는 기능을 수행함
        StartCoroutine(CameraMoveSequence()); //CameraMoveSequence 실행
    }

    /// <summary>카메라 시퀀스 연출 후 이벤트를 명시적으로 호출</summary>
    private IEnumerator CameraMoveSequence()
    {
        /*
         * IEnumerator : C#의 인터페이스로, 반복 가능한 구조(Enumerable)를 반환
         * 기본적으로 Unity는 yield 문 다음에 프레임에 코루틴을 다시 시작한다
         */
        f_SetCameraPriority(camZoomTarget); //줌 카메라 활성화
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camTrajectory); //궤적 카메라 활성화
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camDefault); //기본 카메라 활성화

        yield return new WaitUntil(() => IsCameraReady); //Blend가 완료될때까지 루틴 대기
        yield return new WaitForSeconds(0.5f); //blend가 완료되었지만 카메라 원상복귀까지 0.5초가 추가로 필요함

        //event는 아무도 구독하지 않으면 null 상태임, null 체크를 하지 않고 Invoke() 하면 예외가 발생함
        if (OnCameraBlendComplete != null) //따라서 null 체크를 진행함
        {
            OnCameraBlendComplete.Invoke(); //외부 구독자에게 Blend 완료를 알림
        }
    }

    /// <summary>모든 카메라의 우선순위를 리셋하고 주어진 카메라만 활성화하는 메소드</summary>
    private void f_SetCameraPriority(CinemachineCamera camera)
    {
        camDefault.Priority = nDefaultPriority;       //기본 카메라 우선순위 기본상태로 변경
        camZoomTarget.Priority = nDefaultPriority;    //줌 카메라 우선순위 기본상태로 변경
        camTrajectory.Priority = nDefaultPriority;    //궤적 카메라 우선순위 기본상태로 변경

        camera.Priority = nActivePriority;   //매개변수 카메라 최우선순위 변경 및 활성화
    }
}
