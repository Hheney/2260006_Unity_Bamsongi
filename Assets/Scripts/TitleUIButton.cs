using UnityEngine;

public class TitleUIButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void f_GameStart() //게임 시작 버튼
    {
        //SoundManager.Instance.f_PlaySFX(SoundName.SFX_ButtonClick, 0.7f); 
        GameManager.Instance.f_OpenScene(SceneName.InitScene);
    }

    public void f_Exit()  //종료 버튼
    {
        //SoundManager.Instance.f_PlaySFX(SoundName.SFX_ButtonClick, 0.7f);

        //에디터 종료(에디터 상에서 프로그램이 실행되기 때문에 에디터 실행을 종료)
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
