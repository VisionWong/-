using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VFramework;

public class CameraController : MonoBehaviour
{
    private void Awake()
    {
        MessageCenter.Instance.AddListener<Vector3>(MessageType.OnChessMoving, MoveToTarget);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener<Vector3>(MessageType.OnChessMoving, MoveToTarget);
    }

    public void MoveToTarget(Vector3 target)
    {
        transform.DOMove(new Vector3(target.x, target.y, transform.position.z), 0.5f);
    }
}
