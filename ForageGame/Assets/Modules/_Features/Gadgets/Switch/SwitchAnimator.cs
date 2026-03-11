using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets.Switch
{
    [RequireComponent(typeof(Animator))]
    public class SwitchAnimator : MonoBehaviour
    {
        [SerializeField] private Transform _hingeTransform;
        [SerializeField] private float _rotationAngle = 20;
        [SerializeField] private float _duration = 1;
        private Sequence sequence;

        public void ToStateOn(Action callback = null, bool instant = false)
        {
            if (instant) ToStateOnInstant(callback);
            else ToStateOnAnimated(callback);
        }

        public void ToStateOnInstant(Action callback = null)
        {
            sequence?.Kill();
            _hingeTransform.localEulerAngles = _rotationAngle * Vector3.forward;
            callback?.Invoke();
        }

        public void ToStateOnAnimated(Action callback = null)
        {
            sequence?.Kill();
            sequence = DOTween.Sequence()
            .Append(_hingeTransform.DOLocalRotate(_rotationAngle * Vector3.forward, _duration))
            .OnComplete(() => callback?.Invoke());
        }

        public void ToStateOff(Action callback = null, bool instant = false)
        {
            if (instant) ToStateOffInstant(callback);
            else ToStateOffAnimated(callback);
        }
        public void ToStateOffInstant(Action callback = null)
        {
            sequence?.Kill();
            _hingeTransform.localEulerAngles = -_rotationAngle * Vector3.forward;
            callback?.Invoke();
        }

        public void ToStateOffAnimated(Action callback = null)
        {
            sequence?.Kill();
            sequence = DOTween.Sequence()
            .Append(_hingeTransform.DOLocalRotate(-_rotationAngle * Vector3.forward, _duration))
            .OnComplete(() => callback?.Invoke());
        }

        void OnDestroy()
        {
            sequence?.Kill();
        }
    }
}