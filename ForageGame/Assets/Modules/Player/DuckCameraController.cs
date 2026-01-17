using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class DuckCameraController : MonoBehaviour
    {
        public float followZDistance;
        public float followYDistance;
        public float followSharpness;
        public float velocityWeight;
        public float lookingDirectionOffset;
        public DuckController player;

        void LateUpdate()
        {
            if (!player) return;

            Vector3 targetPos = player.transform.position;
            targetPos.y += followYDistance;
            targetPos.z -= followZDistance;
            targetPos += player.characterController.velocity * velocityWeight;
            targetPos += player._viewDirection * lookingDirectionOffset;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSharpness * Time.deltaTime);
        }
    }
}