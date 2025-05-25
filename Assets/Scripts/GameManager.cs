using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; //���� ��ȯ�ϱ� ���� ���Ŵ��� ����Ʈ
using UnityEngine.SocialPlatforms.Impl;

//SRP(���� å�� ��Ģ)�� ���� GameManager�� ���� �Ŵ������� ����, �����ϴ� ������ ������

//�� �̸��� �����Է��ϴ� ���ڿ� �ϵ��ڵ��� �ٿ� �� ȣ�� ���������� ���� enum ���
public enum SceneName
{
    //������Ʈ �� �� ����â
    /*
     * ����
       ThirdStage, //��������3 ��
       ClearScene  //Ŭ���� ��
     */

    GameScene //���� ��
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private int nScore = 0;      //�÷��̾��� ����
    private int nTotalScore = 0; //�÷��̾��� �� ����
    private bool isCanShoot = false; //private �ʵ�
    private int nRemainingShots = 10; //���� ��ȸ (�ʱⰪ 10)


    //�б� ���� ������Ƽ, �ܺο����� �б⸸ ����
    public int RemainingShots { get { return nRemainingShots; } }
    public int TotalScore { get { return nTotalScore; } }
    public int Score { get { return nScore; } }

    //�߻� ���� ���� ������Ƽ
    public bool CanShoot { get { return isCanShoot; } set { isCanShoot = value; } }

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
        CameraManager.Instance.OnCameraBlendComplete += f_OnBlendComplete; //ī�޶󿡼� �߻���Ų Blend ���� �̺�Ʈ�� �����ϱ� ���� listener
        StartCoroutine(f_EnableFirstShoot());

        UIManager.Instance.f_UpdateShotCount(); //���� Ƚ�� �ʱⰪ ���
    }

    public void f_DecreaseShotCount()
    {
        nRemainingShots--;

        UIManager.Instance.f_UpdateShotCount(); //���� Ƚ�� UI ����
        Debug.Log($"���� ��ȸ : {nRemainingShots}");

        if (nRemainingShots <= 0)
        {
            f_GameOver();
        }
    }

    private void f_GameOver()
    {
        Debug.Log($"���� ����, ���� : {nTotalScore}");
        
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
        //SceneManager.LoadScene(SceneName);
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
        string sSceneName = null;

        sSceneName = SceneManager.GetActiveScene().name;

        return sSceneName;
    }
}
