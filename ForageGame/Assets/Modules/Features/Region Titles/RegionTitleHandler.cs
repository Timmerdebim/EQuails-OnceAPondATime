using DG.Tweening;
using TDK.PlayerSystem;
using TMPro;
using UnityEngine;

namespace TDK.RegionTitles
{
    public class RegionTitleHandler : MonoBehaviour
    {
        [SerializeField] private string _regionText;

        void OnTriggerEnter(Collider other)
        {
            if (other.transform == Player.Instance.transform)
                RegionTitleManager.Instance?.TriggerRegionTitle(_regionText);
        }
    }
}