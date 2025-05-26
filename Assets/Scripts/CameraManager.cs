using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/*
 * [주의사항]
 * CameraManager 사용시, GameManager 또는 InitScene 에서 초기화 필수
 * GameManager → CameraManager.Instance.f_Init();
 */

/// <summary> 카메라 우선순위를 조절하고 Blend 이벤트를 처리하는 카메라 매니저 클래스 </summary>
public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;
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
            _instance = this;
            DontDestroyOnLoad(gameObject); //카메라 매니저 유지
        }
        else if (_instance != this)
        {
            Debug.Log("CameraManager has another instance.");
            Destroy(gameObject);
        }
    }

    // 카메라 참조 필드
    private CinemachineCamera camDefault = null;        //기본 카메라
    private CinemachineCamera camZoomTarget1 = null;    //줌 1번 카메라
    private CinemachineCamera camZoomTarget2 = null;    //줌 2번 카메라
    private CinemachineCamera camZoomTarget3 = null;    //줌 3번 카메라
    private CinemachineCamera camTrajectory = null;     //궤적 카메라
    
    private CinemachineBrain cinemachineBrain = null;   //Blend 여부를 확인하기 위한 CinemachineBrain 컴포넌트를 불러오기 위함

    //카메라 우선순위 상수값
    private const int nDefaultPriority = 10;  //기본 우선순위
    private const int nActivePriority = 20;   //활성화 우선순위
    private const float fWaitTime = 2.0f;     //카메라 전환간 대기시간

    public delegate void CameraBlendCompleteDelegate(); //델리게이트(delegate): 블렌드 완료 시 호출될 메소드 형식 정의
    public event CameraBlendCompleteDelegate OnCameraBlendComplete; //이벤트(event): 외부에서 구독할 수 있는 이벤트

    /// <summary> 카메라가 완전히 Blend 종료된 상태인지 반환하는 프로퍼티 </summary>
    public bool IsCameraReady
    {
        get
        {
            return camDefault != null &&
                   camDefault.Priority == nActivePriority &&
                   cinemachineBrain != null &&
                   !cinemachineBrain.IsBlending;
            //기본 카메라가 활성화 상태 && cinemachineBrain가 null이 아님 && Blend 상태가 아닐 경우 IsCameraReady "true" 값 리턴
        }
    }

    /// <summary> 런타임(Runtime)에서 카메라와 컴포넌트를 연결하는 초기화 메소드 </summary>
    public void f_Init()
    {
        /*
         * 코드의 가독성과 예외 발생 방지를 위해 C#의 null 조건 연산자인 '?.'를 사용함
         * 예시 : gPowerGauge?.GetComponent<Image>() 에서 gPowerGauge가 null이 아니면 GetComponent<Image>()를 접근
         *        null이면 null을 반환하고 예외를 발생시키지 않음
         */

        camDefault = GameObject.Find("Cam_Default")?.GetComponent<CinemachineCamera>();
        camZoomTarget1 = GameObject.Find("Cam_ZoomTarget1")?.GetComponent<CinemachineCamera>();
        camZoomTarget2 = GameObject.Find("Cam_ZoomTarget2")?.GetComponent<CinemachineCamera>();
        camZoomTarget3 = GameObject.Find("Cam_ZoomTarget3")?.GetComponent<CinemachineCamera>();
        camTrajectory = GameObject.Find("Cam_Trajectory")?.GetComponent<CinemachineCamera>();

        GameObject gMainCamera = GameObject.Find("Main Camera");
        if (gMainCamera != null)
        {
            cinemachineBrain = gMainCamera.GetComponent<CinemachineBrain>(); //CinemachineBrain 컴포넌트 가져오기
        }

        //디버그
        if (camDefault == null || camZoomTarget1 == null || camTrajectory == null)
        {
            Debug.LogWarning("일부 카메라가 연결되지 않았습니다. 이름을 확인해주세요.");
        }

        f_SetCameraPriority(camDefault); //f_Init 호출시, 기본 카메라로 초기화
    }

    /// <summary> 카메라 연출 시퀀스를 코루틴으로 실행하는 메소드 </summary>
    public void f_MoveCameraRoutine()
    {
        //Coroutine : 일시 중단이 가능한 메소드, 여러 프레임에 걸쳐 작업을 나눠서 처리할 수 있도록 해주는 기능을 수행함
        StartCoroutine(CameraMoveSequence()); //CameraMoveSequence 실행
    }

    /*
     * IEnumerator : C#의 인터페이스로, 반복 가능한 구조(Enumerable)를 반환
     * 기본적으로 Unity는 yield 문 다음에 프레임에 코루틴을 다시 시작한다
     */

    /// <summary>카메라 시퀀스 연출 후 이벤트를 명시적으로 호출</summary>
    private IEnumerator CameraMoveSequence()
    {
        int nActiveTargetIndex = TargetManager.Instance.f_GetActiveTargetIndex(); //활성화 과녁 Index값 들고오기

        switch(nActiveTargetIndex) //Index = 0 부터 시작
        {
            case 0:
                f_SetCameraPriority(camZoomTarget1); //1번 줌 카메라 활성화
                break;

            case 1:
                f_SetCameraPriority(camZoomTarget2); //2번 줌 카메라 활성화
                break;

            case 2:
                f_SetCameraPriority(camZoomTarget3); //3번 줌 카메라 활성화
                break;

            default:
                f_SetCameraPriority(camDefault);    //기본 카메라 활성화
                break;
        }
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camTrajectory); //궤적 카메라 활성화
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camDefault); //기본 카메라 활성화
        yield return new WaitUntil(() => IsCameraReady); //Blend가 완료될때까지 루틴 대기
        yield return new WaitForSeconds(0.5f); //blend가 완료되었지만 카메라 원상복귀까지 0.5초가 추가로 필요함(버그 방지)

        /*
         * event는 아무도 구독하지 않으면 null 상태, null 체크를 하지 않고 Invoke() 하면 예외가 발생한다.
         * 따라서 null 체크를 해야한다. null 조건 연산자를 통해 null체크 후 Invoke() 메소드 호출
         */
        OnCameraBlendComplete?.Invoke(); //외부 구독자에게 Blend 완료를 알림

        /*
        if (OnCameraBlendComplete != null) 
        {
            OnCameraBlendComplete.Invoke();
        }
        */
    }

    /// <summary>모든 카메라의 우선순위를 리셋하고 주어진 카메라만 활성화하는 메소드</summary>
    private void f_SetCameraPriority(CinemachineCamera camera)
    {
        camDefault.Priority = nDefaultPriority;
        camZoomTarget1.Priority = nDefaultPriority;
        camZoomTarget2.Priority = nDefaultPriority;
        camZoomTarget3.Priority = nDefaultPriority;
        camTrajectory.Priority = nDefaultPriority;

        if (camera != null)
        {
            camera.Priority = nActivePriority;
        }
    }
}
