using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientApp.Components
{
    public class BenefitSelectorBase : ComponentBase
    {
        protected IEnumerable<BenefitModel> Benefits { get; set; }
        protected bool SaveButtonDisabled { get; set; } = true;

        [Inject]
        public AppDbContext AppDbContext { get; set; }

        [Parameter]
        public EmployeeModel Employee { get; set; }

        [Parameter]
        public EventCallback<bool> OnPremiumToggle { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Benefits = await GetForEmployee(Employee.EmployeeId);
        }

        public async Task<IEnumerable<BenefitModel>> GetForEmployee(int employeeId)
        {
            var employeeBenefits = await AppDbContext.EmployeeBenefits
                .Where(eb => eb.EmployeeId == employeeId).ToListAsync();

            var list = new List<BenefitModel>();

            foreach (var benefit in await AppDbContext.Benefits.ToListAsync())
            {
                var employeeBenefit = employeeBenefits
                    .SingleOrDefault(eb => eb.BenefitId == benefit.BenefitId);

                list.Add(new BenefitModel
                {
                    BenefitId = benefit.BenefitId,
                    Selected = employeeBenefit != null,
                    Description = benefit.Description,
                    StartDate = employeeBenefit?.StartDate,
                    EndDate = employeeBenefit?.EndDate,
                    Premium = benefit.Premium
                });
            }


            return list;
        }

        public async Task CheckBoxChanged(ChangeEventArgs e, BenefitModel benefit)
        {
            var newValue = (bool)e.Value;
            benefit.Selected = newValue;
            SaveButtonDisabled = false;

            if (newValue)
            {
                benefit.StartDate = DateTime.Now;
                benefit.EndDate = DateTime.Now.AddYears(1);
            }

            await OnPremiumToggle.InvokeAsync(Benefits.Any(b => b.Premium && b.Selected));
        }

        public async Task SaveClick()
        {
            await UpdateForEmployee(Employee.EmployeeId, Benefits);
            SaveButtonDisabled = true;
        }

        public async Task UpdateForEmployee(int employeeId, IEnumerable<BenefitModel> model)
        {
            var existingBenefits = await AppDbContext.EmployeeBenefits
                .Where(eb => eb.EmployeeId == employeeId).ToListAsync();
            AppDbContext.RemoveRange(existingBenefits);

            var entities = model
                .Where(m => m.Selected)
                .Select(m => new EmployeeBenefit
                {
                    BenefitId = m.BenefitId,
                    EmployeeId = employeeId,
                    StartDate = m.StartDate.Value,
                    EndDate = m.EndDate.Value
                });
            await AppDbContext.EmployeeBenefits.AddRangeAsync(entities);
            await AppDbContext.SaveChangesAsync();
        }
    }
}
