using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BethanysPieShopHRM.ClientApp.Components;
using BethanysPieShopHRM.ClientApp.Services;
using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopHRM.ClientApp.Pages
{
    public class EmployeeOverviewBase: ComponentBase
    {
        [Inject]
        public AppDbContext AppDbContext { get; set; }

        public List<EmployeeModel> Employees { get; set; }

        protected AddEmployeeDialog AddEmployeeDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Employees = (await AppDbContext.Employees.ToListAsync()).Select(e=> e.ToModel()).ToList();
        }

        public async void AddEmployeeDialog_OnDialogClose()
        {
            Employees = (await AppDbContext.Employees.ToListAsync()).Select(e => e.ToModel()).ToList();
            StateHasChanged();
        }

        protected void QuickAddEmployee()
        {
            AddEmployeeDialog.Show();
        }
    }
}
