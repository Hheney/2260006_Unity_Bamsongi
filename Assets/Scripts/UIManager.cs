using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * [주의사항]
 * UIManger 사용시, GameScene이 시작되었을 때 명시적으로 초기화되어야함
 * GameManager → Start() → UIManager.Instance.f_Init();
 */

/// <summary> 게임 내 UI 요소들을 관리하는 UI매니저 클래스 </summary>
public class UIManager : MonoBehaviour
{
    //싱글톤 패턴 적용
    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("UIManager is null.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); //UI매니저 유지
        }
        else if (_instance != this)
        {
            Debug.Log("UIManager has another instance.");
            Destroy(gameObject);
        }
    }

    private TMP_Text textShotCount = null;
    private TMP_Text textScore = null;
    private TMP_Text textTotalScore = null;

    private GameObject gPowerGauge = null;
    private Image imgPowerGaugeFill = null;
    
    //게이지 FillAmount Read-Only(읽기전용) 프로퍼티
    public float GaugeFillAmount
    {
        get => imgPowerGaugeFill != null ? imgPowerGaugeFill.fillAmount : 0.0f;
        //삼항연산자 사용 : null일 경우 0.0f, 아닐경우 fillAmount 반환
    }

    //Unity에서 런타임 = 플레이 모드 상태 (플레이 버튼 누른 이후)
    /// <summary> UI 관련 오브젝트를 런타임(Runtime)에서 찾아 연결하는 초기화 메소드 </summary>
    public void f_Init()
    {
        /*
         * 코드의 가독성과 예외 발생 방지를 위해 C#의 null 조건 연산자인 '?.'를 사용함
         * 예시 : gPowerGauge?.GetComponent<Image>() 에서 gPowerGauge가 null이 아니면 GetComponent<Image>()를 접근
         *        null이면 null을 반환하고 예외를 발생시키지 않음
         */

        gPowerGauge = GameObject.Find("PowerGauge");
        imgPowerGaugeFill = gPowerGauge?.GetComponent<Image>();

        textShotCount = GameObject.Find("TextShotCount")?.GetComponent<TMP_Text>();
        textScore = GameObject.Find("TextScore")?.GetComponent<TMP_Text>();
        textTotalScore = GameObject.Find("TextTotalScore")?.GetComponent<TMP_Text>();

        if (gPowerGauge != null)
        {
            gPowerGauge.SetActive(false); //PowerGauge를 시작시 비활성화;
        }

        //디버깅
        if (textShotCount == null || textScore == null || textTotalScore == null)
        {
            Debug.LogWarning("일부 UI 텍스트가 연결되지 않았습니다.");
        }
    }

    /// <summary>게이지를 화면에 표시하거나 숨기는 메소드</summary>
    public void f_ActivePowerGauge(bool isActive)
    {
        if(gPowerGauge != null)
        {
            gPowerGauge.SetActive(isActive);
        }
    }

    /// <summary>게이지의 Fill값을 설정하는 메소드</summary>
    public void f_SetGaugeAmount(float Amount)
    {
        if(imgPowerGaugeFill != null)
        {
            imgPowerGaugeFill.fillAmount = Amount;
        }
    }

    /// <summary>총점 UI를 업데이트 하는 메소드</summary>
    public void f_UpdateTotalScore()
    {
        string sTotalScore = $"TotalScore : {GameManager.Instance.TotalScore}";
        textTotalScore.text = sTotalScore;
    }

    /// <summary>점수 UI를 업데이트 하는 메소드</summary>
    public void f_UpdateScore()
    {
        string sScore = $"Score : {GameManager.Instance.Score}";
        textScore.text = sScore;
    }

    /// <summary>남은 기회 수 UI 갱신 메소드</summary>
    public void f_UpdateShotCount()
    {
        int nRemain = GameManager.Instance.RemainingShots;
        textShotCount.text = $"RemainCount : {nRemain} / 10";
    }
}
