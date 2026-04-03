using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace TDK.PlayerSystem
{
    [RequireComponent(typeof(SpriteLibrary))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerVisuals : MonoBehaviour
    {
        [System.Serializable]
        public class DuckOrientationGroup
        {
            public SpriteLibraryAsset FrontLeft;
            public SpriteLibraryAsset FrontRight;
            public SpriteLibraryAsset BackLeft;
            public SpriteLibraryAsset BackRight;

            public SpriteLibraryAsset GetSpriteLibrary(bool isFacingLeft, bool isFacingFront)
            {
                if (isFacingLeft)
                {
                    if (isFacingFront) return FrontLeft;
                    else return BackLeft;
                }
                else
                {
                    if (isFacingFront) return FrontRight;
                    else return BackRight;
                }
            }
        }
        [SerializeField] private DuckOrientationGroup[] _duckOrientationGroup = new DuckOrientationGroup[4];
        private SpriteLibrary spriteLibrary;
        private SpriteRenderer spriteRenderer;
        private bool _isFacingLeft = true;
        private bool _isFacingFront = true;
        private int _wingLevel = 0;

        void Awake()
        {
            spriteLibrary = GetComponent<SpriteLibrary>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateVisuals()
        {
            UpdateWingVisuals();
            UpdateViewVisuals();
        }

        public void UpdateWingVisuals()
        {
            int oldWingLevel = _wingLevel;
            _wingLevel = Player.Instance.playerData.wingLevel;
            if (oldWingLevel == _wingLevel) return;
            SetSpriteLibraryAsset();
        }

        public void UpdateViewVisuals()
        {
            UpdateViewVisualsX();
            UpdateViewVisualsZ();
        }

        public void UpdateViewVisualsX()
        {
            bool oldIsFacingLeft = _isFacingLeft;
            if (Player.Instance.playerController.ViewDirection.x < 0) _isFacingLeft = true;
            else if (Player.Instance.playerController.ViewDirection.x > 0) _isFacingLeft = false;
            // else if == 0, we do nothing as not to snap the player around
            if (oldIsFacingLeft == _isFacingLeft) return;
            spriteRenderer.flipX = !_isFacingLeft;
        }

        public void UpdateViewVisualsZ()
        {
            bool oldIsFacingFront = _isFacingFront;
            _isFacingFront = Player.Instance.playerController.ViewDirection.z <= 0;
            // We do snap the player to face forward if given the option
            if (oldIsFacingFront == _isFacingFront) return;
            SetSpriteLibraryAsset();
        }

        private void SetSpriteLibraryAsset()
        {
            spriteLibrary.spriteLibraryAsset = _duckOrientationGroup[_wingLevel].GetSpriteLibrary(_isFacingLeft, _isFacingFront);
        }
    }
}