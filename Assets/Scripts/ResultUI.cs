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
    /// ������ ������ϴ� �޼ҵ�(Onclick���� ȣ��)
    /// </summary>
    public void f_RetryGame()
    {
        GameManager.Instance.f_OpenScene(SceneName.InitScene); //�񵿱� �ε��� �����ϴ� �Ŵ��� �ʱ�ȭ ������ �̵�
    }
}
