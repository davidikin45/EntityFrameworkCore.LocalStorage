using BethanysPieShopHRM.ClientApp.Services;
using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientApp.Pages
{
    public class EmployeeEditBase : ComponentBase
    {
        [Inject]
        public AppDbContext AppDbContext { get; set; }


        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string EmployeeId { get; set; }

        public InputText LastNameInputText { get; set; }

        public EmployeeModel Employee { get; set; } = new EmployeeModel();

        //needed to bind to select to value
        protected string CountryId = string.Empty;
        protected string JobCategoryId = string.Empty;

        //used to store state of screen
        protected string Message = string.Empty;
        protected string StatusClass = string.Empty;
        protected bool Saved;

        public List<Country> Countries { get; set; } = new List<Country>();
        public List<JobCategory> JobCategories { get; set; } = new List<JobCategory>();

        protected override async Task OnInitializedAsync()
        {
            Saved = false;
            Countries = await AppDbContext.Countries.ToListAsync();
            JobCategories = await AppDbContext.JobCategories.ToListAsync();

            int.TryParse(EmployeeId, out var employeeId);

  

            if (employeeId == 0) //new employee is being created
            {
                //add some defaults
                Employee = new EmployeeModel { CountryId = 1, JobCategoryId = 1, BirthDate = DateTime.Now, JoinedDate = DateTime.Now };
            }
            else
            {
                Employee = (await AppDbContext.Employees.FindAsync(int.Parse(EmployeeId))).ToModel();
            }

            CountryId = Employee.CountryId.ToString();
            JobCategoryId = Employee.JobCategoryId.ToString();
        }

        protected async Task HandleValidSubmit()
        {
            Employee.CountryId = int.Parse(CountryId);
            Employee.JobCategoryId = int.Parse(JobCategoryId);

            if (Employee.EmployeeId == 0) //new
            {
                var employee = new Employee();
                Employee.UpdateEntity(employee);
                var addedEmployee = await AppDbContext.Employees.AddAsync(employee);
                await AppDbContext.SaveChangesAsync();

                if (addedEmployee != null)
                {
                    StatusClass = "alert-success";
                    Message = "New employee added successfully.";
                    Saved = true;
                }
                else
                {
                    StatusClass = "alert-danger";
                    Message = "Something went wrong adding the new employee. Please try again.";
                    Saved = false;
                }
            }
            else
            {
                var foundEmployee = AppDbContext.Employees.FirstOrDefault(e => e.EmployeeId == Employee.EmployeeId);
                Employee.UpdateEntity(foundEmployee);
                await AppDbContext.SaveChangesAsync();

                StatusClass = "alert-success";
                Message = "Employee updated successfully.";
                Saved = true;
            }
        }

        protected void HandleInvalidSubmit()
        {
            StatusClass = "alert-danger";
            Message = "There are some validation errors. Please try again.";
        }

        protected async Task DeleteEmployee()
        {
            var foundEmployee = AppDbContext.Employees.FirstOrDefault(e => e.EmployeeId == Employee.EmployeeId);
            if (foundEmployee != null)
            {
                AppDbContext.Employees.Remove(foundEmployee);
                await AppDbContext.SaveChangesAsync();
            }

            StatusClass = "alert-success";
            Message = "Deleted successfully";

            Saved = true;
        }

        protected void NavigateToOverview()
        {
            NavigationManager.NavigateTo("/employeeoverview");
        }
    }

}