using UnityEngine;
using UnityEngine.UI;
using UnityEditor; //EditorApplication ����� ���� ����Ʈ

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button StageStart;
    [SerializeField] private Button HowToPlay; //HowToPlay ��ư
    [SerializeField] private Button X; //X ��ư
    [SerializeField] private Button Quit; //���� ��ư
    GameObject m_HowtoPlay = null; //HowToPlay UI ������Ʈ
    GameObject m_X = null; //X ��ư ������Ʈ
    GameObject m_Quit = null;       //���θ޴� - ����

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_HowtoPlay = GameObject.Find("i_HowtoPlay"); //HowToPlay UI ������Ʈ ã��
        m_X = GameObject.Find("X"); //X ��ư ������Ʈ ã��
        m_HowtoPlay.SetActive(false); //�ʱ⿡�� ��Ȱ��ȭ
        m_X.SetActive(false); //X ��ư ������Ʈ ��Ȱ��ȭ
        m_Quit = GameObject.Find("Quit");
    }

    private void Update()
    {

        StageStart.onClick.AddListener(OnClickStart);
        HowToPlay.onClick.AddListener(OnClickHowToPlay);
        X.onClick.AddListener(OnClickX);
        Quit.onClick.AddListener(QuitDown); //���� ��ư Ŭ�� �� QuitDown �޼ҵ� ȣ��
    }

    void OnClickStart()
    {
        GameManager.Instance.f_OpenScene(SceneName.GameScene);
    }
    void OnClickHowToPlay()
    {
        m_HowtoPlay.SetActive(true);
        m_X.SetActive(true); //X ��ư Ȱ��ȭ
    }

    void OnClickX()
    {
        m_HowtoPlay.SetActive(false);
        m_X.SetActive(false); //X ��ư ��Ȱ��ȭ
    }
    public void QuitDown()  //���� ��ư
    {

        //������ ����(������ �󿡼� ���α׷��� ����Ǳ� ������ ������ ������ ����)
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
