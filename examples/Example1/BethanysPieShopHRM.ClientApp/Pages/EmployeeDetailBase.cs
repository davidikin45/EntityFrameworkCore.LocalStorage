using BethanysPieShopHRM.ComponentsLibrary.Map;
using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientApp.Pages
{
    public class EmployeeDetailBase : ComponentBase
    {
        [Inject]
        public AppDbContext AppDbContext { get; set; }

        [Parameter]
        public string EmployeeId { get; set; }

        public List<Marker> MapMarkers { get; set; } = new List<Marker>();

        protected string JobCategory = string.Empty;

        public EmployeeModel Employee { get; set; } = new EmployeeModel();

        protected override async Task OnInitializedAsync()
        {
            Employee = (await AppDbContext.Employees.FindAsync(int.Parse(EmployeeId))).ToModel();

            MapMarkers = new List<Marker>
            {
                new Marker{Description = $"{Employee.FirstName} {Employee.LastName}",  ShowPopup = false, X = Employee.Longitude, Y = Employee.Latitude}
            };
            JobCategory = (await AppDbContext.JobCategories.FindAsync(Employee.JobCategoryId)).JobCategoryName;
        }
    }
}
