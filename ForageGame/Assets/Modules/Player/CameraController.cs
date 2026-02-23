using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float followZDistance;
    public float followYDistance;
    public float followSharpness;
    public float velocityWeight;
    public float lookingDirectionOffset;

    void LateUpdate()
    {
        if (!Player.Instance) return;

        Vector3 targetPos = Player.Instance.transform.position;

        targetPos.y += followYDistance;
        targetPos.z -= followZDistance;
        targetPos += Player.Instance.characterController.velocity * velocityWeight;
        targetPos += Player.Instance._viewDirection * lookingDirectionOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSharpness * Time.deltaTime);
    }
}