using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private int nScore = 0;      //�÷��̾��� ����
    private int nTotalScore = 0; //�÷��̾��� �� ����

    //�б� ���� ������Ƽ, �ܺο����� �б⸸ ����
    public int TotalScore { get { return nTotalScore; } }
    public int Score { get { return nScore; } }

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
            _instance = this; //this : ���� �ν��Ͻ��� ����Ű�� ���۷���
        }
        else if (_instance != this)
        {
            Debug.Log("GameManager has another instance.");

            Destroy(gameObject); //���� �ν��Ͻ� �ı�(GameManger Object)
        }
        DontDestroyOnLoad(gameObject); //���� ����Ǿ ���� ���� ������Ʈ�� ������Ű�� �޼ҵ�
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //����̽� ���ɿ� ���� ������ ���� ���ֱ�
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �Ÿ� ������� ���� ��� �� ������ �����ϴ� �޼ҵ�
    /// </summary>
    /// <param name="fDistance"></param>
    /// <param name="fMaxRadius"></param>
    public void f_AddScoreByDistance(float fDistance, float fMaxRadius)
    {
        nScore = f_CalculateScoreByZone(fDistance, fMaxRadius);
        nTotalScore += nScore;

        Debug.Log($"�Ÿ�: {fDistance:F3} ����: {nScore} ����: {nTotalScore}");
    }

    /// <summary>
    /// �Ÿ� ��� ���� ��� �޼ҵ�, �߽ɿ� �������� ���� ���� ����(�ִ� 10��, �ּ� 0��)
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
