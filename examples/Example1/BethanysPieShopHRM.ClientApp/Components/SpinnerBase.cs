using BethanysPieShopHRM.ClientApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BethanysPieShopHRM.ClientApp.Components
{
    public class SpinnerBase : ComponentBase
    {
        [Inject]
        public ISpinnerService SpinnerService { get; set; }

        protected bool IsVisible { get; set; }

        protected override void OnInitialized()
        {
            SpinnerService.OnShow += ShowSpinner;
            SpinnerService.OnHide += HideSpinner;
        }

        public void ShowSpinner()
        {
            IsVisible = true;
            StateHasChanged();
        }

        public void HideSpinner()
        {
            IsVisible = false;
            StateHasChanged();
        }
    }
}
