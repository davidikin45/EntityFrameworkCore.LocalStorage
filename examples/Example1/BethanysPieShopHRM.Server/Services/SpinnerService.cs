using System;
using System.Collections.Generic;
using System.Text;

namespace BethanysPieShopHRM.Server.Services
{
	public class SpinnerService : ISpinnerService
	{
		public event Action OnShow;
		public event Action OnHide;

		public void Show()
		{
			OnShow?.Invoke();
		}

		public void Hide()
		{
			OnHide?.Invoke();
		}
	}
}
