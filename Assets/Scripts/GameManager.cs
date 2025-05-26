using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

//SRP(단일 책임 원칙)을 위해 GameManager는 여러 매니저들을 초기화, 연결, 제어하는 로직을 수행함

public enum SceneName //씬 이름을 직접입력하는 문자열 하드코딩을 줄여 씬 호출 오류방지를 위해 enum 사용
{
    MainMenuScene, //메인메뉴 씬
    TitleScene, //타이틀 씬
    InitScene,  //초기화 전용 씬
    GameScene   //게임 씬
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("GameManager is null.");
            }
            return _instance;
        }
    }

    private int nScore = 0;             //플레이어의 점수
    private int nTotalScore = 0;        //플레이어의 총 점수
    private int nRemainingShots = 10;   //남은 기회 (초기값 10)
    private bool isCanShoot = false;    //발사 가능 여부
    private const int nClearScore = 50; //클리어 조건 점수

    //읽기 전용 프로퍼티, 외부에서는 읽기만 가능
    public int Score { get { return nScore; } }
    public int TotalScore { get { return nTotalScore; } }
    public int RemainingShots { get { return nRemainingShots; } }

    //발사 가능 여부 프로퍼티
    public bool CanShoot { get { return isCanShoot; } set { isCanShoot = value; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.Log("GameManager has another instance.");
            Destroy(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60; //디바이스 성능에 따른 실행결과 차이 없애기
    }

    /// <summary> GameScene 진입 후에 명시적으로 호출할 초기화 메서드 </summary>
    public void f_Init()
    {
        Application.targetFrameRate = 60;

        f_ResetGameState();
        UIManager.Instance?.f_UpdateShotCount();
        CameraManager.Instance.OnCameraBlendComplete += f_OnBlendComplete;

        StartCoroutine(f_EnableFirstShoot());
    }

    /// <summary>남은 기회를 차감하는 메소드</summary>
    public void f_DecreaseShotCount()
    {
        nRemainingShots--;
        UIManager.Instance.f_UpdateShotCount(); //남은 횟수 UI 갱신

        Debug.Log($"남은 기회 : {nRemainingShots}");

        if (nRemainingShots <= 0)
        {
            f_GameOver();
        }

        if (nTotalScore >= nClearScore)
        {
            f_GameClear();
        }
    }

    private void f_GameClear()
    {
        Debug.Log($"게임 클리어! 총점: {nTotalScore}");

        TargetManager.Instance?.f_StopTargetRoutine();
        TargetManager.Instance?.f_Reset();

        //SoundManager.Instance?.f_PlaySFX(SoundName.SFX_GameClear, 1.0f);
    }

    private void f_GameOver()
    {
        Debug.Log($"게임 오버, 총점 : {nTotalScore}");

        TargetManager.Instance?.f_StopTargetRoutine(); //씬 전환전에 루틴은 필수적으로 종료되어야 함
        TargetManager.Instance?.f_Reset();

        //f_OpenScene(SceneName.GameScene);
        //GameScene에서 GameScene 로드시 유니티의 고질적인 구조적 문제를 동반함(해결 불가)

        f_OpenScene(SceneName.InitScene); //디버깅중에는 초기화 씬으로 이동해서 자동으로 다시 로드
    }

    /// <summary> 첫 시작 시 0.5초후 발사 허용 (버그 방지) </summary>
    private IEnumerator f_EnableFirstShoot()
    {
        yield return new WaitForSeconds(0.5f);
        isCanShoot = true; //isCanShoot의 기본상태는 false이므로 처음이면 true로 변경해야함
    }

    /// <summary> Blend가 종료되면 발사를 허용하는 메소드 </summary>
    private void f_OnBlendComplete()
    {
        StartCoroutine(f_ResumeAfterBlend());
    }

    /// <summary> Blend가 완전히 종료되면 발사와 타겟 루틴 재개 </summary>
    private IEnumerator f_ResumeAfterBlend()
    {
        while (!CameraManager.Instance.IsCameraReady)
        {
            yield return null;
        }

        isCanShoot = true; //발사 가능 상태로 전환
        TargetManager.Instance.f_ResumeTargetRoutine(); //Blend가 완전히 종료되면 과녁 루틴 재개
    }

    
    /// <summary> 거리 기반으로 점수 계산 후 총점에 가산하는 메소드 </summary>
    public void f_AddScoreByDistance(float fDistance, float fMaxRadius)
    {
        nScore = f_CalculateScoreByZone(fDistance, fMaxRadius);
        nTotalScore += nScore;

        Debug.Log($"거리: {fDistance:F3} 점수: {nScore} 총점: {nTotalScore}");
    }

    /// <summary> 거리 기반 점수 계산 메소드, 중심에 가까울수록 높은 점수 리턴(최대 10점, 최소 0점) </summary>
    private int f_CalculateScoreByZone(float fDistance, float fMaxRadius)
    {
        float fStep = fMaxRadius / 6f;

        if (fDistance <= fStep * 1f) return 10;
        else if (fDistance <= fStep * 2f) return 9;
        else if (fDistance <= fStep * 3f) return 8;
        else if (fDistance <= fStep * 4f) return 7;
        else if (fDistance <= fStep * 5f) return 6;
        else if (fDistance <= fStep * 6f) return 5;
        else return 0;
    }
    
    /// <summary> 매개변수로 받은 씬으로 이동하는 메소드 </summary>
    public void f_OpenScene(SceneName sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }

    /// <summary> 현재 씬 명칭을 정의된 Enum과 매핑하는 메소드 </summary>
    public SceneName f_GetCurrentSceneName()
    {
        /*
         * GetActiveScene()은 현재 씬 이름을 문자열로 반환한다.
         * 그러나 씬 명칭 입력을 enum을 사용할 예정이므로,
         * 씬 이름을 enum으로 매핑해주는 메소드를 생성함
         */

        string sceneName = SceneManager.GetActiveScene().name;

        //활성화된 string 타입 씬 이름을 enum에 정의된 이름일 경우 현재 씬 반환
        if (System.Enum.TryParse(sceneName, out SceneName currentScene))
        {
            return currentScene;
        }
        else
        {
            Debug.LogWarning($"씬 {sceneName}이 SceneName Enum에 존재하지 않습니다.");
        }

        //!에러방지 임시코드!
        return SceneName.GameScene;
        //return SceneName.TitleScene; //예외 사항이 발생시 타이틀화면으로 이동
    }

    /// <summary> 활성화된 씬 네임을 불러오는 메소드 </summary>
    public string f_GetSceneName() //활성화된 씬 이름을 불러와서 씬에 맞는 BGM 재생을 자동화 하기위함
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>게임 점수 및 상태 초기화 전용 메소드</summary>
    public void f_ResetGameState()
    {
        nScore = 0;
        nTotalScore = 0;
        nRemainingShots = 10;
        isCanShoot = false;
    }
}
