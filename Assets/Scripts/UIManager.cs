using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * [���ǻ���]
 * UIManger ����, GameScene�� ���۵Ǿ��� �� ��������� �ʱ�ȭ�Ǿ����
 * GameManager �� Start() �� UIManager.Instance.f_Init();
 */

/// <summary> ���� �� UI ��ҵ��� �����ϴ� UI�Ŵ��� Ŭ���� </summary>
public class UIManager : MonoBehaviour
{
    //�̱��� ���� ����
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
            DontDestroyOnLoad(gameObject); //UI�Ŵ��� ����
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
    
    //������ FillAmount Read-Only(�б�����) ������Ƽ
    public float GaugeFillAmount
    {
        get => imgPowerGaugeFill != null ? imgPowerGaugeFill.fillAmount : 0.0f;
        //���׿����� ��� : null�� ��� 0.0f, �ƴҰ�� fillAmount ��ȯ
    }

    //Unity���� ��Ÿ�� = �÷��� ��� ���� (�÷��� ��ư ���� ����)
    /// <summary> UI ���� ������Ʈ�� ��Ÿ��(Runtime)���� ã�� �����ϴ� �ʱ�ȭ �޼ҵ� </summary>
    public void f_Init()
    {
        /*
         * �ڵ��� �������� ���� �߻� ������ ���� C#�� null ���� �������� '?.'�� �����
         * ���� : gPowerGauge?.GetComponent<Image>() ���� gPowerGauge�� null�� �ƴϸ� GetComponent<Image>()�� ����
         *        null�̸� null�� ��ȯ�ϰ� ���ܸ� �߻���Ű�� ����
         */

        gPowerGauge = GameObject.Find("PowerGauge");
        imgPowerGaugeFill = gPowerGauge?.GetComponent<Image>();

        textShotCount = GameObject.Find("TextShotCount")?.GetComponent<TMP_Text>();
        textScore = GameObject.Find("TextScore")?.GetComponent<TMP_Text>();
        textTotalScore = GameObject.Find("TextTotalScore")?.GetComponent<TMP_Text>();

        if (gPowerGauge != null)
        {
            gPowerGauge.SetActive(false); //PowerGauge�� ���۽� ��Ȱ��ȭ;
        }

        //�����
        if (textShotCount == null || textScore == null || textTotalScore == null)
        {
            Debug.LogWarning("�Ϻ� UI �ؽ�Ʈ�� ������� �ʾҽ��ϴ�.");
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

    /// <summary>���� ��ȸ �� UI ���� �޼ҵ�</summary>
    public void f_UpdateShotCount()
    {
        int nRemain = GameManager.Instance.RemainingShots;
        textShotCount.text = $"RemainCount : {nRemain} / 10";
    }
}
