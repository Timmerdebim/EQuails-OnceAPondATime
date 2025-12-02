using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class DuckCameraController : MonoBehaviour
    {
        public float followZDistance;
        public float followFixedY;
        public float followSharpness;
        public Transform player;

        void LateUpdate()
        {
            if (!player) return;

            Vector3 targetPos = player.position;
            targetPos.y = followFixedY;
            targetPos.z -= followZDistance;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSharpness * Time.deltaTime);
        }
    }
}