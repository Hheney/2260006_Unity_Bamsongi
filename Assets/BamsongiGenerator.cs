using UnityEngine;

public class BamsongiGenerator : MonoBehaviour
{
    //프리팹 설계도 전달을 위한 public GameObject 변수
    public GameObject gBamsongiPrefab = null;

    //Instantiate된 밤송이 오브젝트 저장 변수
    GameObject insBamsongiPrefab = null;

    //밤송이 월드 좌표
    Vector3 vBamsongiWorldDir = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //게임화면을 마우스로 클릭했을 때 작동하는 함수
        if(Input.GetMouseButtonDown(0))
        {
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

            insBamsongiPrefab.GetComponent<BamsongiController>().f_TargetShoot(vBamsongiWorldDir.normalized * 2000);
        }
    }
}
