/*
 * Target �������� Box Collider�� Center ���� �������� TargetCenter ������Ʈ�� �ڵ� ���Ľ�Ű�� ��ũ��Ʈ
 */
using System.Net.Sockets;
using UnityEngine;

public class AlignToBoxColliderCenter : MonoBehaviour
{
    BoxCollider boxCollider = null; //�߾� ������ ���� �ڽ� �ݶ��̴� �ʱ�ȭ
    Transform parentObj = null;     //�θ� ������Ʈ�� ã������ Transform �ʱ�ȭ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
         * �߾� ������ ���� TargetCenter�� �θ� ������Ʈ�� Box Collider�� Center ���� �����ؾ� ��
         * ���� TargetCenter�� �θ� ������Ʈ�� �ҷ����� �ҷ��� �θ��� BoxCollider�� ã������
         */
        parentObj = transform.parent;
        boxCollider = parentObj.GetComponent<BoxCollider>();

        f_AlignToParentColliderCenter(); //���� �޼ҵ� ȣ��
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �θ� ������Ʈ�� �ݶ��̴� Center���� �������� ������Ʈ�� �����ϴ� �޼ҵ�
    /// </summary>
    void f_AlignToParentColliderCenter()
    {
        if (parentObj == null)
        {
            Debug.LogWarning($"{gameObject.name} ������Ʈ�� �θ� ������Ʈ�� �����Ǿ����ϴ�.");

            return; //������Ʈ�� ������� ����
        }

        if(boxCollider != null) //BoxCollider�� ���� ���
        {
            transform.localPosition = boxCollider.center; //BoxCollider�� Center������ TargetCenter�� localPosition�� ����

            Debug.Log($"{gameObject.name}�� {parentObj.name}�� BoxCollider �߽� {boxCollider.center}�� ���ĵǾ����ϴ�.");
        }
        else
        {
            Debug.LogWarning($"{parentObj.name}�� Box Collider�� �����ϴ�.");
        }
    }

}
