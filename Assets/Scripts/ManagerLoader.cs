using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/*
 * [깨달은 점]
 * Unity에서 자기 자신을 재로드하는 것은 Unity의 고질적인 구조적 문제을 야기한다.
 * 쓰지말자
 */


/// <summary>
/// [ManagerLoader 클래스]
/// InitScene에서 각종 Manager들을 단 한 번만 Instantiate하고,
/// GameScene을 비동기로 로드한 후 안정적인 초기화를 수행하는 역할을 수행하는 클래스
/// </summary>
public class ManagerLoader : MonoBehaviour
{
    [Header("Manager Prefabs")]
    [SerializeField] private GameObject gGameManager;      //GameManager 프리팹
    [SerializeField] private GameObject gUIManager;        //UIManager 프리팹
    [SerializeField] private GameObject gCameraManager;    //CameraManager 프리팹
    [SerializeField] private GameObject gTargetManager;    //TargetManager 프리팹
    [SerializeField] private GameObject gSoundManager;     //SoundManager 프리팹

    [Header("Loading UI")]
    [SerializeField] private GameObject gLoadingPanel;     //로딩 화면 Panel
    [SerializeField] private TMP_Text textLoading;         //로딩 텍스트
    [SerializeField] private Slider sliderProgress;        //로딩 진행바

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); //최초 생성 시 파괴되지 않도록 설정(싱글톤)
    }

    private void Start()
    {
        //게임 시작 시 매니저들을 생성하고 씬을 로드함
        f_SpawnManagers();                      //매니저 프리팹들을 Instantiate
        StartCoroutine(f_LoadGameSceneAsync()); //GameScene 비동기 로드 + 초기화 루틴 실행
    }

    /// <summary>Manager 프리팹을 한 번만 생성하는 메소드</summary>
    private void f_SpawnManagers()
    {
        //존재하지 않는 Manager만 Instantiate하여 중복 생성을 방지함
        if (GameManager.Instance == null)
        {
            Instantiate(gGameManager);
        }

        if (UIManager.Instance == null)
        {
            Instantiate(gUIManager);
        }

        if (CameraManager.Instance == null)
        {
            Instantiate(gCameraManager);
        }

        if (TargetManager.Instance == null)
        {
            Instantiate(gTargetManager);
        }

        if (SoundManager.Instance == null)
        {
            Instantiate(gSoundManager);
        }
    }

    /*
     * 비동기 씬 로드(Asynchronous Scene Loading)란?
     * Unity에서는 씬을 전환할 때 SceneManager.LoadScene()을 사용한다. 
     * 이 방식은 동기(Synchronous) 방식이라서 씬이 로드되는 동안 게임이 일시적으로 멈추게된다.
     * 반면에, 비동기(Asynchronous) 로딩은 씬을 백그라운드에서 로딩하고, 그 동안 UI 표시, 애니메이션 재생 등을 계속 수행 가능한 장점이 있다.
     * 또 완전히 로드되기 전까지 씬 전환을 보류할 수 있다는 점에서 채택함
     * progress : 로딩 진행률
     * isDone : 로딩 완료 여부
     * allowSceneActivation = false : 자동전환 기능 해제 및 90%까지 로딩 후 대기
     */

    /// <summary>GameScene을 비동기로 로딩하고, 각 매니저들 간의 초기화 타이밍을 조율하는 열거자</summary>
    private IEnumerator f_LoadGameSceneAsync()
    {
        gLoadingPanel.SetActive(true); //로딩 패널 표시(기믹)

        AsyncOperation asyncoperation = SceneManager.LoadSceneAsync("GameScene");
        asyncoperation.allowSceneActivation = false; //로딩 완료 전까지 씬 전환 보류

        //진행률 UI 갱신
        while (asyncoperation.progress < 0.9f)
        {
            sliderProgress.value = asyncoperation.progress;
            textLoading.text = $"Loading... {asyncoperation.progress * 100:F0}%"; // $ string 표현
            yield return null;
        }

        //90% 도달시 완료 표시
        sliderProgress.value = 1.0f;
        textLoading.text = "Loading Complete!";
        yield return new WaitForSeconds(0.5f); //0.5초 대기

        asyncoperation.allowSceneActivation = true; //씬 전환

        //씬이 완전히 로드될 때까지 대기
        yield return new WaitUntil(() => asyncoperation.isDone);

        //GameScene 내 모든 오브젝트가 생성되도록 2프레임 대기(하단의 초기화 타이밍의 조율을 위해 필요)
        yield return new WaitForEndOfFrame();
        yield return null;

        //영향력 순서대로 초기화 (UI → Camera → Target → Game)
        UIManager.Instance?.f_Init();

        CameraManager.Instance?.f_Init();
        yield return null;

        TargetManager.Instance?.f_Init();
        yield return null; 

        GameManager.Instance?.f_Init(); 

        //SoundManager.Instance?.f_AutoPlayBGM(); //배경음악 자동 재생
    }
}