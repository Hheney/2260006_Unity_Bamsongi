/*
 * Target 오브젝의 Box Collider의 Center 값을 기준으로 TargetCenter 오브젝트를 자동 정렬시키는 스크립트
 */
using System.Net.Sockets;
using UnityEngine;

public class AlignToBoxColliderCenter : MonoBehaviour
{
    BoxCollider boxCollider = null; //중앙 정렬을 위한 박스 콜라이더 초기화
    Transform parentObj = null;     //부모 오브젝트를 찾기위한 Transform 초기화

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
         * 중앙 정렬을 위해 TargetCenter는 부모 오브젝트의 Box Collider의 Center 값을 참조해야 함
         * 따라서 TargetCenter는 부모 오브젝트를 불러오고 불러온 부모의 BoxCollider를 찾도록함
         */
        parentObj = transform.parent;
        boxCollider = parentObj.GetComponent<BoxCollider>();

        f_AlignToParentColliderCenter(); //정렬 메소드 호출
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 부모 오브젝트의 콜라이더 Center값을 기준으로 오브젝트를 정렬하는 메소드
    /// </summary>
    void f_AlignToParentColliderCenter()
    {
        if (parentObj == null)
        {
            Debug.LogWarning($"{gameObject.name} 오브젝트의 부모 오브젝트가 누락되었습니다.");

            return; //오브젝트가 없을경우 리턴
        }

        if(boxCollider != null) //BoxCollider가 있을 경우
        {
            transform.localPosition = boxCollider.center; //BoxCollider의 Center값으로 TargetCenter의 localPosition을 변경

            Debug.Log($"{gameObject.name}가 {parentObj.name}의 BoxCollider 중심 {boxCollider.center}에 정렬되었습니다.");
        }
        else
        {
            Debug.LogWarning($"{parentObj.name}에 Box Collider가 없습니다.");
        }
    }

}
