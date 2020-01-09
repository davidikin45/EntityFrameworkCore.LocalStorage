using System;
using System.Collections.Generic;
using System.Text;

namespace BethanysPieShopHRM.Server.Services
{
    public interface ISpinnerService
    {
        event Action OnShow;
        event Action OnHide;

        void Show();
        void Hide();

    }
}
