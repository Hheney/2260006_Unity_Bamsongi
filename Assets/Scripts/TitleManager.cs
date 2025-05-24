using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Button GameStart;

    private void Start()
    {
        GameStart.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        GameManager.Instance.f_OpenScene(SceneName.MenuScene);
    }
}