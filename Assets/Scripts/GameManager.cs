using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

//SRP(���� å�� ��Ģ)�� ���� GameManager�� ���� �Ŵ������� �ʱ�ȭ, ����, �����ϴ� ������ ������

public enum SceneName //�� �̸��� �����Է��ϴ� ���ڿ� �ϵ��ڵ��� �ٿ� �� ȣ�� ���������� ���� enum ���
{
    MainMenuScene, //���θ޴� ��
    TitleScene, //Ÿ��Ʋ ��
    InitScene,  //�ʱ�ȭ ���� ��
    GameScene   //���� ��
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

    private int nScore = 0;             //�÷��̾��� ����
    private int nTotalScore = 0;        //�÷��̾��� �� ����
    private int nRemainingShots = 10;   //���� ��ȸ (�ʱⰪ 10)
    private bool isCanShoot = false;    //�߻� ���� ����
    private const int nClearScore = 50; //Ŭ���� ���� ����

    //�б� ���� ������Ƽ, �ܺο����� �б⸸ ����
    public int Score { get { return nScore; } }
    public int TotalScore { get { return nTotalScore; } }
    public int RemainingShots { get { return nRemainingShots; } }

    //�߻� ���� ���� ������Ƽ
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
        Application.targetFrameRate = 60; //����̽� ���ɿ� ���� ������ ���� ���ֱ�
    }

    /// <summary> GameScene ���� �Ŀ� ��������� ȣ���� �ʱ�ȭ �޼��� </summary>
    public void f_Init()
    {
        Application.targetFrameRate = 60;

        f_ResetGameState();
        UIManager.Instance?.f_UpdateShotCount();
        CameraManager.Instance.OnCameraBlendComplete += f_OnBlendComplete;

        StartCoroutine(f_EnableFirstShoot());
    }

    /// <summary>���� ��ȸ�� �����ϴ� �޼ҵ�</summary>
    public void f_DecreaseShotCount()
    {
        nRemainingShots--;
        UIManager.Instance.f_UpdateShotCount(); //���� Ƚ�� UI ����

        Debug.Log($"���� ��ȸ : {nRemainingShots}");

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
        Debug.Log($"���� Ŭ����! ����: {nTotalScore}");

        TargetManager.Instance?.f_StopTargetRoutine();
        TargetManager.Instance?.f_Reset();

        //SoundManager.Instance?.f_PlaySFX(SoundName.SFX_GameClear, 1.0f);
    }

    private void f_GameOver()
    {
        Debug.Log($"���� ����, ���� : {nTotalScore}");

        TargetManager.Instance?.f_StopTargetRoutine(); //�� ��ȯ���� ��ƾ�� �ʼ������� ����Ǿ�� ��
        TargetManager.Instance?.f_Reset();

        //f_OpenScene(SceneName.GameScene);
        //GameScene���� GameScene �ε�� ����Ƽ�� �������� ������ ������ ������(�ذ� �Ұ�)

        f_OpenScene(SceneName.InitScene); //������߿��� �ʱ�ȭ ������ �̵��ؼ� �ڵ����� �ٽ� �ε�
    }

    /// <summary> ù ���� �� 0.5���� �߻� ��� (���� ����) </summary>
    private IEnumerator f_EnableFirstShoot()
    {
        yield return new WaitForSeconds(0.5f);
        isCanShoot = true; //isCanShoot�� �⺻���´� false�̹Ƿ� ó���̸� true�� �����ؾ���
    }

    /// <summary> Blend�� ����Ǹ� �߻縦 ����ϴ� �޼ҵ� </summary>
    private void f_OnBlendComplete()
    {
        StartCoroutine(f_ResumeAfterBlend());
    }

    /// <summary> Blend�� ������ ����Ǹ� �߻�� Ÿ�� ��ƾ �簳 </summary>
    private IEnumerator f_ResumeAfterBlend()
    {
        while (!CameraManager.Instance.IsCameraReady)
        {
            yield return null;
        }

        isCanShoot = true; //�߻� ���� ���·� ��ȯ
        TargetManager.Instance.f_ResumeTargetRoutine(); //Blend�� ������ ����Ǹ� ���� ��ƾ �簳
    }

    
    /// <summary> �Ÿ� ������� ���� ��� �� ������ �����ϴ� �޼ҵ� </summary>
    public void f_AddScoreByDistance(float fDistance, float fMaxRadius)
    {
        nScore = f_CalculateScoreByZone(fDistance, fMaxRadius);
        nTotalScore += nScore;

        Debug.Log($"�Ÿ�: {fDistance:F3} ����: {nScore} ����: {nTotalScore}");
    }

    /// <summary> �Ÿ� ��� ���� ��� �޼ҵ�, �߽ɿ� �������� ���� ���� ����(�ִ� 10��, �ּ� 0��) </summary>
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
    
    /// <summary> �Ű������� ���� ������ �̵��ϴ� �޼ҵ� </summary>
    public void f_OpenScene(SceneName sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }

    /// <summary> ���� �� ��Ī�� ���ǵ� Enum�� �����ϴ� �޼ҵ� </summary>
    public SceneName f_GetCurrentSceneName()
    {
        /*
         * GetActiveScene()�� ���� �� �̸��� ���ڿ��� ��ȯ�Ѵ�.
         * �׷��� �� ��Ī �Է��� enum�� ����� �����̹Ƿ�,
         * �� �̸��� enum���� �������ִ� �޼ҵ带 ������
         */

        string sceneName = SceneManager.GetActiveScene().name;

        //Ȱ��ȭ�� string Ÿ�� �� �̸��� enum�� ���ǵ� �̸��� ��� ���� �� ��ȯ
        if (System.Enum.TryParse(sceneName, out SceneName currentScene))
        {
            return currentScene;
        }
        else
        {
            Debug.LogWarning($"�� {sceneName}�� SceneName Enum�� �������� �ʽ��ϴ�.");
        }

        //!�������� �ӽ��ڵ�!
        return SceneName.GameScene;
        //return SceneName.TitleScene; //���� ������ �߻��� Ÿ��Ʋȭ������ �̵�
    }

    /// <summary> Ȱ��ȭ�� �� ������ �ҷ����� �޼ҵ� </summary>
    public string f_GetSceneName() //Ȱ��ȭ�� �� �̸��� �ҷ��ͼ� ���� �´� BGM ����� �ڵ�ȭ �ϱ�����
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>���� ���� �� ���� �ʱ�ȭ ���� �޼ҵ�</summary>
    public void f_ResetGameState()
    {
        nScore = 0;
        nTotalScore = 0;
        nRemainingShots = 10;
        isCanShoot = false;
    }
}
