using UnityEngine;
using DG.Tweening;
using System;

namespace Project.CustomTweenUtils
{
    [Serializable]
    public class TweenOptions
    {
        public float duration = 0;
        public Ease ease = Ease.Unset;
        public AnimationCurve customEase = new();
        public bool snapping = false;
    }

    [Serializable]
    public class TweenTransformOptions
    {
        public Transform target = null;
        public TweenOptions position = new();
        public TweenOptions rotation = new();
    }

    public static class CustomTweenUtils
    {
        public static Sequence DOMoveTransform(this Transform transform, TweenTransformOptions options)
        {
            Sequence sequence = DOTween.Sequence();

            if (options.position.ease == Ease.Unset)
                sequence.Append(transform.DOMove(options.target.position, options.position.duration).SetEase(options.position.customEase));
            else
                sequence.Append(transform.DOMove(options.target.position, options.position.duration).SetEase(options.position.ease));
            Debug.Log(0);

            if (options.rotation.ease == Ease.Unset)
                sequence.Join(transform.DORotateQuaternion(options.target.rotation, options.rotation.duration).SetEase(options.rotation.customEase));
            else
                sequence.Join(transform.DORotateQuaternion(options.target.rotation, options.rotation.duration).SetEase(options.rotation.ease));

            Debug.Log(1);

            return sequence;
        }
    }



}