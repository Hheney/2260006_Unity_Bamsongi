using UnityEngine;
using UnityEngine.UI;

public class BamsongiGenerator : MonoBehaviour
{
    public GameObject gBamsongiPrefab = null;   //프리팹 설계도 전달을 위한 public GameObject 필드
    GameObject insBamsongiPrefab = null;        //Instantiate된 밤송이 오브젝트 저장 필드
    Vector3 vBamsongiWorldDir = Vector3.zero;   //밤송이 월드 좌표

    //------------------------[밤송이의 방향 틀어짐 수정]------------------------
    Vector3 vSpawnPosition = Vector3.zero;      //밤송이가 스폰될 좌표
    Quaternion vRotation = Quaternion.identity; //밤송이가 스폰되고 바라볼 방향

    bool isCharging = false;        //게이지의 충전 여부
    float fChargingTime = 0.0f;     //게이지의 좌,우 움직임을 위한 시간 필드
    float fGaugeSpeed = 1.5f;       //게이지의 좌, 우 움직임 속도
    float fGaugeValue = 0.0f;       //게이지 UI의 FillAmount에 연산된 값을 주기 위한 필드
    float fGaugeMaxValue = 1.0f;    //게이지의 최대값 필드
    float fGaugeLastValue = 0.0f;   //게이지의 마지막 값 필드
    float fThrowStrength = 0.0f;    //밤송이를 던지는 힘 필드

    [SerializeField] private float fMinThrowStrength = 300.0f;
    [SerializeField] private float fGaugePowerMultiplier = 1500.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.CanShoot /*|| !CameraManager.Instance.IsCameraReady*/) //CanShoot이 false값이면 return 시켜 마우스 동작을 금지시킴
        {
            return;
        }

        //마우스 좌클릭을 시작하면 게이지 충전 시작
        if (Input.GetMouseButtonDown(0))
        {
            f_StartChargeGauge();
        }

        //마우스 클릭 유지 중이면 충전 게이지 업데이트
        if (Input.GetMouseButton(0) && isCharging) 
        {
            f_UpdateChargeGauge();
        }

        //마우스 클릭 해제 시 밤송이를 발사
        if (Input.GetMouseButtonUp(0) && isCharging) //마우스를 뗄 경우
        {
            SoundManager.Instance.f_PlaySFX(SoundName.SFX_PopSound, 1.0f); //밤송이 발사 효과음
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

    /// <summary> 마우스 클릭을 해제시 밤송이를 생성하고 클릭 방향으로 발사하는 메소드 </summary>
    void f_ReleaseGaugeAndShoot()
    {
        /*
         * Ray 클래스
         * Ray(레이)는 이름 그대로 광선이며, 광원의 좌표(Origin)와 광선을 방향(direction)을 멤버 변수로 갖음
         * Ray는 콜라이더가 적용된 오브젝트와 충돌을 감지하는 특징이 있음
         * ScreenPointToRay 메소드의 반환값으로 얻을 수 있는 Ray는 Origin이 Main camera의 좌표고,
         *      direction이 카메라에서 탭한 좌표로 향하는 벡터  
         */

        isCharging = false; //충전중 아님 상태 전환

        fGaugeLastValue = UIManager.Instance.GaugeFillAmount; //UI 게이지의 최종 fillAmount 값 저장
        fThrowStrength = fMinThrowStrength + fGaugeLastValue * fGaugePowerMultiplier; //밤송이를 발사할 힘을 연산


        //------------------------[밤송이의 방향 틀어짐 수정]------------------------

        Ray ScreenPointToRayBamsongi = Camera.main.ScreenPointToRay(Input.mousePosition); //클릭 지점에서 Ray 생성
        vBamsongiWorldDir = ScreenPointToRayBamsongi.direction.normalized; //Ray의 방향 벡터를 정규화(normalized) 하여 방향 벡터(발사할 방향)로 저장

        vSpawnPosition = ScreenPointToRayBamsongi.origin + vBamsongiWorldDir * 0.5f; //밤송이가 생성될 위치 연산(클릭 지점 기반)

        vRotation = Quaternion.LookRotation(vBamsongiWorldDir); //LookRotation 메소드를 사용하여 밤송이가 바라봐야할 방향 연산
        
        insBamsongiPrefab = Instantiate(gBamsongiPrefab, vSpawnPosition, vRotation); //밤송이(gBamsongiPrefab)를 vSpawnPosition 위치에서 vRotation 방향으로 생성

        insBamsongiPrefab.GetComponent<BamsongiController>().f_TargetShoot(vBamsongiWorldDir * fThrowStrength); //밤송이에 힘을 전달하는 메소드에 연산한 힘 전달

        //------------------------[밤송이의 방향 틀어짐 수정]------------------------

        //게이지 UI 초기화
        UIManager.Instance.f_SetGaugeAmount(0.0f);
        UIManager.Instance.f_ActivePowerGauge(false);
    }
    
}