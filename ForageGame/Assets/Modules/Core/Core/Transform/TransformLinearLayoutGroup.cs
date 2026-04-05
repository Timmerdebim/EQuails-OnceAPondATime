using System;
using System.Linq;
using UnityEngine;

namespace TDK.TransformSystem
{
    public class TransformLinearLayoutGroup : MonoBehaviour
    {
        [SerializeField] private Vector3 _axis = new(1, 0, 0);
        [SerializeField] private float _spacing = 1;
        [SerializeField] private bool _alphabetize = false;
        [SerializeField] private bool _setGlobalPosition = false;

        void OnValidate()
        {
            Transform[] transforms = transform.Cast<Transform>().ToArray();
            if (_alphabetize) transforms = Alphabetize(transforms);
            if (_setGlobalPosition) ApplyLayoutGlobal(transforms, _axis, _spacing);
            else ApplyLayoutLocal(transforms, _axis, _spacing);
        }

        private Transform[] Alphabetize(Transform[] transforms)
        {
            Transform[] newTransforms = transforms.OrderBy(go => go.name).ToArray();
            for (int i = 0; i < newTransforms.Length; i++)
                newTransforms[i].transform.SetSiblingIndex(i);
            return newTransforms;
        }

        private void ApplyLayoutLocal(Transform[] transforms, Vector3 axis, float spacing)
        {
            axis = axis.normalized;
            spacing = Math.Max(spacing, 0);
            for (int i = 0; i < transforms.Length; i++)
                transforms[i].localPosition = i * spacing * axis;
        }

        private void ApplyLayoutGlobal(Transform[] transforms, Vector3 axis, float spacing)
        {
            axis = axis.normalized;
            spacing = Math.Max(spacing, 0);
            for (int i = 0; i < transforms.Length; i++)
                transforms[i].position = i * spacing * axis;
        }
    }
}