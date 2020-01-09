using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientApp.Components
{
    public class AddEmployeeDialogBase : ComponentBase
    {
        public bool ShowDialog { get; set; }

        [CascadingParameter]
        public Theme Theme { get; set; }

        public EmployeeModel Employee { get; set; } = new EmployeeModel { CountryId = 1, JobCategoryId = 1, BirthDate = DateTime.Now, JoinedDate = DateTime.Now };

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        [Inject] 
        public AppDbContext AppDbContext { get; set; }

        public void Show()
        {
            ResetDialog();
            ShowDialog = true;
        }

        private void ResetDialog()
        {
            Employee = new EmployeeModel { CountryId = 1, JobCategoryId = 1, BirthDate = DateTime.Now, JoinedDate = DateTime.Now };
        }

        public void Close()
        {
            ShowDialog = false;
        }

        protected async Task HandleValidSubmit()
        {
            var employee = new Employee();
            Employee.UpdateEntity(employee);
            await AppDbContext.Employees.AddAsync(employee);

            await AppDbContext.SaveChangesAsync();

            ShowDialog = false;

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
