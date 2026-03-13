using System;
using DG.Tweening;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace TDK.Gadgets
{
    public class HindgeAnimator : MonoBehaviour
    {
        [SerializeField] private Transform _hingeTransform;
        [SerializeField] private Vector3 _openAngle = new(30, 0, 0);
        [SerializeField] private float _openDuration = 1;
        [SerializeField] private CurveField _openCurve;
        [SerializeField] private Vector3 _closedAngle = new(30, 0, 0);
        [SerializeField] private float _closeDuration = 1;
        [SerializeField] private CurveField _closeCurve;
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

        public void AnimateInstant(float targetAngle)
        {
            sequence?.Kill();
            _hingeTransform.localEulerAngles = targetAngle * Vector3.forward;
            callback?.Invoke();
        }

        public void Animate()

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