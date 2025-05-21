using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using System.Security.Cryptography; //AES ��ȣȭ �˰����� �����ϱ� ����

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textScore; //���� Text UI
    [SerializeField] private TMP_Text textTotalScore; //���� Text UI

    GameObject gPowerGauge = null;                  //PowerGauge ������Ʈ �ʵ�
    UnityEngine.UI.Image imgPowerGaugeFill = null;  //ĸ��ȭ�� ���� private ���� FillAmount �ʵ�
    
    private static UIManager _instance = null;
    public float GaugeFillAmount //FillAmount Read-Only ������Ƽ
    {
        get => imgPowerGaugeFill != null ? imgPowerGaugeFill.fillAmount : 0.0f; //���׿����� ��� : null�� ��� 0.0f, �ƴҰ�� fillAmount ��ȯ
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

        gPowerGauge.SetActive(false); //PowerGauge�� ���۽� ��Ȱ��ȭ;
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

    /// <summary>�������� ȭ�鿡 ǥ���ϰų� ����� �޼ҵ�</summary>
    public void f_ActivePowerGauge(bool isActive)
    {
        if(gPowerGauge != null)
        {
            gPowerGauge.SetActive(isActive);
        }
    }

    /// <summary>�������� Fill���� �����ϴ� �޼ҵ�</summary>
    public void f_SetGaugeAmount(float Amount)
    {
        if(imgPowerGaugeFill != null)
        {
            imgPowerGaugeFill.fillAmount = Amount;
        }
    }

    /// <summary>���� UI�� ������Ʈ �ϴ� �޼ҵ�</summary>
    public void f_UpdateTotalScore()
    {
        string sTotalScore = $"TotalScore : {GameManager.Instance.TotalScore}";
        textTotalScore.text = sTotalScore;
    }

    /// <summary>���� UI�� ������Ʈ �ϴ� �޼ҵ�</summary>
    public void f_UpdateScore()
    {
        string sScore = $"Score : {GameManager.Instance.Score}";
        textScore.text = sScore;
    }
}
