using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using System.Security.Cryptography; //AES 암호화 알고리즘을 적용하기 위함

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textScore; //점수 Text UI
    [SerializeField] private TMP_Text textTotalScore; //총점 Text UI

    GameObject gPowerGauge = null;                  //PowerGauge 오브젝트 필드
    UnityEngine.UI.Image imgPowerGaugeFill = null;  //캡슐화를 위해 private 접근 FillAmount 필드
    
    private static UIManager _instance = null;
    public float GaugeFillAmount //FillAmount Read-Only 프로퍼티
    {
        get => imgPowerGaugeFill != null ? imgPowerGaugeFill.fillAmount : 0.0f; //삼항연산자 사용 : null일 경우 0.0f, 아닐경우 fillAmount 반환
    }

    public static UIManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("UIManager is null.");
            return _instance;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gPowerGauge = GameObject.Find("PowerGauge");
        imgPowerGaugeFill = gPowerGauge.GetComponent<UnityEngine.UI.Image>();

        gPowerGauge.SetActive(false); //PowerGauge를 시작시 비활성화;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.Log("UIManager has another instance.");
            Destroy(gameObject);
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
}
