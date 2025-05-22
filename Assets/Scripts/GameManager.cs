using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; //씬을 전환하기 위한 씬매니저 임포트
using UnityEngine.SocialPlatforms.Impl;

//씬 이름을 직접입력하는 문자열 하드코딩을 줄여 씬 호출 오류방지를 위해 enum 사용
public enum SceneName
{
    //프로젝트 내 씬 구성창
    /*
     * 예시
       ThirdStage, //스테이지3 씬
       ClearScene  //클리어 씬
     */

    GameScene //게임 씬
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private int nScore = 0;      //플레이어의 점수
    private int nTotalScore = 0; //플레이어의 총 점수

    //읽기 전용 프로퍼티, 외부에서는 읽기만 가능
    public int TotalScore { get { return nTotalScore; } }
    public int Score { get { return nScore; } }

    //------------------------[발사 금지 기능 구현]------------------------
    //private 필드
    private bool isCanShoot = false;

    //프로퍼티
    public bool CanShoot { get { return isCanShoot; } set { isCanShoot = value; } }
    //------------------------[발사 금지 기능 구현]------------------------

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

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this; //this : 현재 인스턴스를 가리키는 레퍼런스
        }
        else if (_instance != this)
        {
            Debug.Log("GameManager has another instance.");

            Destroy(gameObject); //현재 인스턴스 파괴(GameManger Object)
        }
        DontDestroyOnLoad(gameObject); //씬이 변경되어도 현재 게임 오브젝트를 유지시키는 메소드
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //디바이스 성능에 따른 실행결과 차이 없애기
        Application.targetFrameRate = 60;

        //------------------------[발사 금지 기능 구현]------------------------
        CameraManager.Instance.OnCameraBlendComplete += f_OnBlendComplete;

        StartCoroutine(f_EnableFirstShoot()); //버그 방지를 위해 시작시 0.5초 대기후 발사 가능하도록 함
        //------------------------[발사 금지 기능 구현]------------------------
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //------------------------[발사 금지 기능 구현]------------------------
    /// <summary> Blend가 종료되면 발사를 허용하는 메소드 </summary>
    private void f_OnBlendComplete()
    {
        /*
        if (CameraManager.Instance.IsCameraReady) //Blend가 끝나고 카메라가 기본 카메라로 변경된 경우
        {
            isCanShoot = true; //발사 허용
        }*/
        StartCoroutine(f_WaitUntilCameraReadyThenAllowShoot());
    }

    private IEnumerator f_WaitUntilCameraReadyThenAllowShoot()
    {
        while (!CameraManager.Instance.IsCameraReady)
        {
            yield return null; //1프레임 대기
        }

        isCanShoot = true;
    }

    private IEnumerator f_EnableFirstShoot()
    {
        yield return new WaitForSeconds(0.5f);
        
        isCanShoot = true; //isCanShoot의 기본상태는 false이므로 처음이면 true로 변경해야함
    }
    //------------------------[발사 금지 기능 구현]------------------------

    /*
     * 유니티에서 씬을 로드하는 것은 SceneManger.LoadScene() 메소드를 사용
     * 씬 이름이나 빌드 설정 인덱스를 파라미터로 전달하여 특정 씬을 로드할 수 있음
     * 씬 이름으로 로드 : SceneManger.LoadScene("MySceneName");
     * 빌드 설정 인덱스로 로드 : SceneManger.LoadScene(1); (두 번째 씬을 로드)
     * SceneManger 클래스의 LoadScene 메소드를 사용해 게임 씬으로 전환
     */

    /// <summary> 매개변수로 받은 씬으로 이동하는 메소드 </summary>
    public void f_OpenScene(SceneName sceneName)
    {
        //SceneManager.LoadScene(SceneName);
        SceneManager.LoadScene(sceneName.ToString());
    }

    /*
     * GetActiveScene()은 현재 씬 이름을 문자열로 반환한다.
     * 그러나 씬 명칭 입력을 enum을 사용할 예정이므로,
     * 씬 이름을 enum으로 매핑해주는 메소드를 생성함
     */

    /// <summary> 현재 씬 명칭을 정의된 Enum과 매핑하는 메소드 </summary>
    public SceneName f_GetCurrentSceneName()
    {
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
        string sSceneName = null;

        sSceneName = SceneManager.GetActiveScene().name;

        return sSceneName;
    }


    /// <summary>
    /// 거리 기반으로 점수 계산 후 총점에 가산하는 메소드
    /// </summary>
    /// <param name="fDistance"></param>
    /// <param name="fMaxRadius"></param>
    public void f_AddScoreByDistance(float fDistance, float fMaxRadius)
    {
        nScore = f_CalculateScoreByZone(fDistance, fMaxRadius);
        nTotalScore += nScore;

        Debug.Log($"거리: {fDistance:F3} 점수: {nScore} 총점: {nTotalScore}");
    }

    /// <summary>
    /// 거리 기반 점수 계산 메소드, 중심에 가까울수록 높은 점수 리턴(최대 10점, 최소 0점)
    /// </summary>
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

}
