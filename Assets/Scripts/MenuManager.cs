using UnityEngine;
using UnityEngine.UI;
using UnityEditor; //EditorApplication 사용을 위해 임포트

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button StageStart;
    [SerializeField] private Button HowToPlay; //HowToPlay 버튼
    [SerializeField] private Button X; //X 버튼
    [SerializeField] private Button Quit; //종료 버튼
    GameObject m_HowtoPlay = null; //HowToPlay UI 오브젝트
    GameObject m_X = null; //X 버튼 오브젝트
    GameObject m_Quit = null;       //메인메뉴 - 종료

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_HowtoPlay = GameObject.Find("i_HowtoPlay"); //HowToPlay UI 오브젝트 찾기
        m_X = GameObject.Find("X"); //X 버튼 오브젝트 찾기
        m_HowtoPlay.SetActive(false); //초기에는 비활성화
        m_X.SetActive(false); //X 버튼 오브젝트 비활성화
        m_Quit = GameObject.Find("Quit");
    }

    private void Update()
    {

        StageStart.onClick.AddListener(OnClickStart);
        HowToPlay.onClick.AddListener(OnClickHowToPlay);
        X.onClick.AddListener(OnClickX);
        Quit.onClick.AddListener(QuitDown); //종료 버튼 클릭 시 QuitDown 메소드 호출
    }

    void OnClickStart()
    {
        GameManager.Instance.f_OpenScene(SceneName.GameScene);
    }
    void OnClickHowToPlay()
    {
        m_HowtoPlay.SetActive(true);
        m_X.SetActive(true); //X 버튼 활성화
    }

    void OnClickX()
    {
        m_HowtoPlay.SetActive(false);
        m_X.SetActive(false); //X 버튼 비활성화
    }
    public void QuitDown()  //종료 버튼
    {

        //에디터 종료(에디터 상에서 프로그램이 실행되기 때문에 에디터 실행을 종료)
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
