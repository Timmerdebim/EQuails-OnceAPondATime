using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TDK.SaveSystem;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Project.Menus
{
    public class ConfirmSettingsController : MonoBehaviour
    {
        public static ConfirmSettingsController Instance { get; private set; }
        [SerializeField] private MenuManager _menuManager;
        [SerializeField] private Menu _settingsMenu;
        [SerializeField] private TransitionScreenController _tsc;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public async Task Load()
        {
            _tsc.FadeIn();
            await _menuManager.ToMenu(_settingsMenu);
        }

        public void ConfirmSettings() => _ = Unload();

        private async Task Unload()
        {
            await _tsc.FadeOutAsync();
            await AppController.Instance.ContinueToNewWorld();
        }
    }
}