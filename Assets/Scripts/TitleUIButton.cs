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

    public void f_GameStart() //���� ���� ��ư
    {
        //SoundManager.Instance.f_PlaySFX(SoundName.SFX_ButtonClick, 0.7f); 
        GameManager.Instance.f_OpenScene(SceneName.InitScene);
    }

    public void f_Exit()  //���� ��ư
    {
        //SoundManager.Instance.f_PlaySFX(SoundName.SFX_ButtonClick, 0.7f);

        //������ ����(������ �󿡼� ���α׷��� ����Ǳ� ������ ������ ������ ����)
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
