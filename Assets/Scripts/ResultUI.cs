using UnityEngine;

public class ResultUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 게임을 재시작하는 메소드(Onclick으로 호출)
    /// </summary>
    public void f_RetryGame()
    {
        GameManager.Instance.f_OpenScene(SceneName.InitScene); //비동기 로딩을 수행하는 매니저 초기화 씬으로 이동
    }
}
