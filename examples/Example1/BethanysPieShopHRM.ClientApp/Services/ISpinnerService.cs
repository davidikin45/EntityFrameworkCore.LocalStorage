using System;
using System.Collections.Generic;
using System.Text;

namespace BethanysPieShopHRM.ClientApp.Services
{
    public interface ISpinnerService
    {
        event Action OnShow;
        event Action OnHide;

        void Show();
        void Hide();

    }
}
