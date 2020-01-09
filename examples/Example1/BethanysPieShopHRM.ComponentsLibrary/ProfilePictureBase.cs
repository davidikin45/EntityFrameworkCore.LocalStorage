using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BethanysPieShopHRM.ComponentsLibrary
{
    public class ProfilePictureBase: ComponentBase
    {
        protected string CssClass { get; set; } = "circle";

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
