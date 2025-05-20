using UnityEngine;
using UnityEngine.UI;

public class BamsongiGenerator : MonoBehaviour
{
    public GameObject gBamsongiPrefab = null;   //프리팹 설계도 전달을 위한 public GameObject 필드
    GameObject insBamsongiPrefab = null;        //Instantiate된 밤송이 오브젝트 저장 필드
    Vector3 vBamsongiWorldDir = Vector3.zero;   //밤송이 월드 좌표

    bool isCharging = false;        //게이지의 충전 여부
    float fChargingTime = 0.0f;     //게이지의 좌,우 움직임을 위한 시간 필드
    float fGaugeSpeed = 1.5f;       //게이지의 좌, 우 움직임 속도
    float fGaugeValue = 0.0f;       //게이지 UI의 FillAmount에 연산된 값을 주기 위한 필드
    float fGaugeMaxValue = 1.0f;    //게이지의 최대값
    float fGaugeLastValue = 0.0f;   //게이지의 마지막 값 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) //마우스 좌클릭시
        {
            f_StartChargeGauge();
        }

        if(Input.GetMouseButton(0) && isCharging) //마우스 좌클릭중 && 게이지 차징중 
        {
            f_UpdateChargeGauge();
        }

        if(Input.GetMouseButtonUp(0) && isCharging) //마우스를 뗄 경우
        {
            f_ReleaseGaugeAndShoot();
        }

    }

    
    void f_StartChargeGauge()
    {
        isCharging = true;
        fChargingTime = 0.0f;

        UIManager.Instance.f_ActivePowerGauge(true); //PowerGauge 활성화
    }

    void f_UpdateChargeGauge()
    {
        fChargingTime += Time.deltaTime * fGaugeSpeed;
        fGaugeValue = Mathf.PingPong(fChargingTime, fGaugeMaxValue);

        UIManager.Instance.f_SetGaugeAmount(fGaugeValue);
    }

    void f_ReleaseGaugeAndShoot()
    {
        isCharging = false;

        fGaugeLastValue = UIManager.Instance.GaugeFillAmount;

        //게임을 실행하는 도중에 밤송이 오브젝트를 생성
        insBamsongiPrefab = Instantiate(gBamsongiPrefab);

        /*
         * Ray 클래스
         * Ray(레이)는 이름 그대로 광선이며, 광원의 좌표(Origin)와 광선을 방향(direction)을 멤버 변수로 갖음
         * Ray는 콜라이더가 적용된 오브젝트와 충돌을 감지하는 특징이 있음
         * ScreenPointToRay 메소드의 반환값으로 얻을 수 있는 Ray는 Origin이 Main camera의 좌표고,
         *      direction이 카메라에서 탭한 좌표로 향하는 벡터
         * direction 방향으로 밤송이를 날리기 때문에 direction 벡터가 가진 nomalized 변수를 사용해 길이가 1인 벡터로 만든 후
         * 힘을 2000 곱한다. 일단 길이를 1 벡터로 해서 direction 벡터 크기에 관계없이 밤송이에 일정한 힘을 가할 수 있음    
         */
        Ray ScreenPointToRayBamsongi = Camera.main.ScreenPointToRay(Input.mousePosition);
        vBamsongiWorldDir = ScreenPointToRayBamsongi.direction;

        float fShootPower = 500.0f + 1200.0f * fGaugeLastValue;

        insBamsongiPrefab.GetComponent<BamsongiController>().f_TargetShoot(vBamsongiWorldDir.normalized * fShootPower);

        UIManager.Instance.f_SetGaugeAmount(0.0f);
        UIManager.Instance.f_ActivePowerGauge(false);
    }
    
}