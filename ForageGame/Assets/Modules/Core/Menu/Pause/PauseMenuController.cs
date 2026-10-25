using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Project.Menus
{
    public class PauseMenuController : MonoBehaviour
    {
        public static PauseMenuController Instance { get; private set; }
        [SerializeField] private MenuManager _menuManager;
        [SerializeField] private Menu _pauseMenu;
        [SerializeField] private CanvasGroup _backgroundImage;
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private Tween _tween;

        void Start()
        {
            _backgroundImage.alpha = 0;
            _tween?.Kill();
            _tween = _backgroundImage.DOFade(1, 0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
            _ = _menuManager.ToMenu(_pauseMenu);
        }

        public async Task ExitAllMenus()
        {
            _tween?.Kill();
            _tween = _backgroundImage.DOFade(0, 0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
            await _menuManager.ToMenu(null);
        }

        public void Escape() => _menuManager.Escape();

        void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}