/*
 * 마우스를 클릭하면 밤송이가 과녁으로 날아가는 동작 제어 스크립트
 */
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //디바이스 성능에 따른 실행결과 차이 없애기
        Application.targetFrameRate = 60;

        /*
         * 밤송이가 화면 안쪽으로 날아가도록 +Z축 방향의 벡터를 매개변수로 전달하고 f_TargetShoot 메소드 호출
         * Y축 방향으로 힘을 200.0f 가하는 이유는 밤송이가 과녁에 닫기 전에 중력의 영향을 받아
         * 지면으로 낙하하는 것을 막기 위함
         * Start 메소드를 호출하는 시작과 동시에 밤송이가 과녁으로 날아감
         */
        f_TargetShoot(new Vector3(0.0f, 200.0f, 2000.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 매개변수 방향으로 밤송이에게 힘을 가하는 메소드
    /// </summary>
    /// <param name="argDir">방향</param>
    public void f_TargetShoot(Vector3 argDir)
    {
        //매개변수로 전달된 Vector값으로 힘을 가한다.
        GetComponent<Rigidbody>().AddForce(argDir);
    }

    //Physics를 사용하므로 과녁과 밤송이가 충돌하면 OnCollisionEnter 메소드가 호출되어 실행됨
    private void OnCollisionEnter(Collision collision)
    {
        /*
         * 밤송이가 과녁에 닿는 순간 밤송이 움직임이 멈추므로, Rigidbody 컴포넌트의 isKinematic 메소드를 true로 설정
         * isKinematic 메소드를 true로 설정 하면, 오브젝트에 작용하는 힘을 무시하고 밤송이를 정지시킴
         * isKinematic 메소드 : 외부에서 가해지는 물리적 힘에 반응하지 않는 오브젝트라는 의미. 중력과 충돍에 반응하지 않도록 함
         */
        GetComponent<Rigidbody>().isKinematic = true;

        /*
         */
        GetComponent<ParticleSystem>().Play();
    }
}
