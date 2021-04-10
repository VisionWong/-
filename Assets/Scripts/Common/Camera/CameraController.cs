using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VFramework;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 0.2f;

    private bool canDrag = true;
    private float leftBorder, rightBorder, upBorder, downBorder;

    private void Awake()
    {
        MessageCenter.Instance.AddListener<Vector3>(MessageType.OnChessMoving, MoveToTarget);
        MessageCenter.Instance.AddListener(MessageType.GlobalCanSelect, OnGlobalCanSelect);
        MessageCenter.Instance.AddListener(MessageType.GlobalCantSelect, OnGlobalCantSelect);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener<Vector3>(MessageType.OnChessMoving, MoveToTarget);
        MessageCenter.Instance.RemoveListener(MessageType.GlobalCanSelect, OnGlobalCanSelect);
        MessageCenter.Instance.RemoveListener(MessageType.GlobalCantSelect, OnGlobalCantSelect);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && canDrag)
        {
            CameraDrag();
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        transform.DOMove(new Vector3(target.x, target.y, transform.position.z), 0.5f);
    }

    public void SetBorder(float left, float right, float up, float down)
    {
        leftBorder = left;
        rightBorder = right;
        upBorder = up;
        downBorder = down;
    }

    private void CameraDrag()
    {
        float xAxis = Input.GetAxis("Mouse X");
        float yAxis = Input.GetAxis("Mouse Y");
        float targetX = transform.position.x - xAxis * dragSpeed;
        float targetY = transform.position.y - yAxis * dragSpeed;
        if (targetX >= leftBorder && targetX <= rightBorder) transform.DOMoveX(targetX, Time.deltaTime);
        if(targetY >= downBorder && targetY <= upBorder) transform.DOMoveY(targetY, Time.deltaTime);

    }

    private void OnGlobalCanSelect()
    {
        canDrag = true;
    }

    private void OnGlobalCantSelect()
    {
        canDrag = false;
    }
}
