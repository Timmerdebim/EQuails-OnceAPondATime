using UnityEngine;
using System.Threading.Tasks;

namespace Project.Menus
{
    public class MenuManager : MonoBehaviour
    {
        public Menu currentMenu { get; private set; } = null;
        private bool _isBusy = false;

        private async Task MenuTransition(Menu fromMenu, Menu toMenu)
        {
            if (_isBusy) return;
            _isBusy = true;
            if (fromMenu) await fromMenu.ExitMenu();
            currentMenu = toMenu;
            if (toMenu) await toMenu.EnterMenu();
            _isBusy = false;
        }

        public async Task ToMenu(Menu toMenu) => await MenuTransition(currentMenu, toMenu);

        public void Escape()
        {
            if (currentMenu != null)
                currentMenu.Escape();
        }
    }
}