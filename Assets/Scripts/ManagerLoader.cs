using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/*
 * [������ ��]
 * Unity���� �ڱ� �ڽ��� ��ε��ϴ� ���� Unity�� �������� ������ ������ �߱��Ѵ�.
 * ��������
 */


/// <summary>
/// [ManagerLoader Ŭ����]
/// InitScene���� ���� Manager���� �� �� ���� Instantiate�ϰ�,
/// GameScene�� �񵿱�� �ε��� �� �������� �ʱ�ȭ�� �����ϴ� ������ �����ϴ� Ŭ����
/// </summary>
public class ManagerLoader : MonoBehaviour
{
    [Header("Manager Prefabs")]
    [SerializeField] private GameObject gGameManager;      //GameManager ������
    [SerializeField] private GameObject gUIManager;        //UIManager ������
    [SerializeField] private GameObject gCameraManager;    //CameraManager ������
    [SerializeField] private GameObject gTargetManager;    //TargetManager ������
    [SerializeField] private GameObject gSoundManager;     //SoundManager ������

    [Header("Loading UI")]
    [SerializeField] private GameObject gLoadingPanel;     //�ε� ȭ�� Panel
    [SerializeField] private TMP_Text textLoading;         //�ε� �ؽ�Ʈ
    [SerializeField] private Slider sliderProgress;        //�ε� �����

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); //���� ���� �� �ı����� �ʵ��� ����(�̱���)
    }

    private void Start()
    {
        //���� ���� �� �Ŵ������� �����ϰ� ���� �ε���
        f_SpawnManagers();                      //�Ŵ��� �����յ��� Instantiate
        StartCoroutine(f_LoadGameSceneAsync()); //GameScene �񵿱� �ε� + �ʱ�ȭ ��ƾ ����
    }

    /// <summary>Manager �������� �� ���� �����ϴ� �޼ҵ�</summary>
    private void f_SpawnManagers()
    {
        //�������� �ʴ� Manager�� Instantiate�Ͽ� �ߺ� ������ ������
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
     * �񵿱� �� �ε�(Asynchronous Scene Loading)��?
     * Unity������ ���� ��ȯ�� �� SceneManager.LoadScene()�� ����Ѵ�. 
     * �� ����� ����(Synchronous) ����̶� ���� �ε�Ǵ� ���� ������ �Ͻ������� ���߰Եȴ�.
     * �ݸ鿡, �񵿱�(Asynchronous) �ε��� ���� ��׶��忡�� �ε��ϰ�, �� ���� UI ǥ��, �ִϸ��̼� ��� ���� ��� ���� ������ ������ �ִ�.
     * �� ������ �ε�Ǳ� ������ �� ��ȯ�� ������ �� �ִٴ� ������ ä����
     * progress : �ε� �����
     * isDone : �ε� �Ϸ� ����
     * allowSceneActivation = false : �ڵ���ȯ ��� ���� �� 90%���� �ε� �� ���
     */

    /// <summary>GameScene�� �񵿱�� �ε��ϰ�, �� �Ŵ����� ���� �ʱ�ȭ Ÿ�̹��� �����ϴ� ������</summary>
    private IEnumerator f_LoadGameSceneAsync()
    {
        gLoadingPanel.SetActive(true); //�ε� �г� ǥ��(���)

        AsyncOperation asyncoperation = SceneManager.LoadSceneAsync("GameScene");
        asyncoperation.allowSceneActivation = false; //�ε� �Ϸ� ������ �� ��ȯ ����

        //����� UI ����
        while (asyncoperation.progress < 0.9f)
        {
            sliderProgress.value = asyncoperation.progress;
            textLoading.text = $"Loading... {asyncoperation.progress * 100:F0}%"; // $ string ǥ��
            yield return null;
        }

        //90% ���޽� �Ϸ� ǥ��
        sliderProgress.value = 1.0f;
        textLoading.text = "Loading Complete!";
        yield return new WaitForSeconds(0.5f); //0.5�� ���

        asyncoperation.allowSceneActivation = true; //�� ��ȯ

        //���� ������ �ε�� ������ ���
        yield return new WaitUntil(() => asyncoperation.isDone);

        //GameScene �� ��� ������Ʈ�� �����ǵ��� 2������ ���(�ϴ��� �ʱ�ȭ Ÿ�̹��� ������ ���� �ʿ�)
        yield return new WaitForEndOfFrame();
        yield return null;

        //����� ������� �ʱ�ȭ (UI �� Camera �� Target �� Game)
        UIManager.Instance?.f_Init();

        CameraManager.Instance?.f_Init();
        yield return null;

        TargetManager.Instance?.f_Init();
        yield return null; 

        GameManager.Instance?.f_Init(); 

        //SoundManager.Instance?.f_AutoPlayBGM(); //������� �ڵ� ���
    }
}