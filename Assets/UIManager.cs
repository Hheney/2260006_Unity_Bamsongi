using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text textScore; //Á¡¼ö Text UI
    public TMP_Text textTotalScore; //ÃÑÁ¡ Text UI

    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("UIManager is null.");
            return _instance;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.Log("UIManager has another instance.");
            Destroy(gameObject);
        }
    }

    public void f_UpdateTotalScore()
    {
        string sTotalScore = $"TotalScore : {GameManager.Instance.TotalScore}";
        textTotalScore.text = sTotalScore;
    }

    public void f_UpdateScore()
    {
        string sScore = $"Score : {GameManager.Instance.Score}";
        textScore.text = sScore;
    }
}
