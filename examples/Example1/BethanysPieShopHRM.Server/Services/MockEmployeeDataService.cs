using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanysPieShopHRM.Shared;

namespace BethanysPieShopHRM.Server.Services
{
    public class MockEmployeeDataService : IEmployeeDataService
    {
        private List<EmployeeModel> _employees;
        private List<Country> _countries;
        private List<JobCategory> _jobCategories;

        private IEnumerable<EmployeeModel> Employees
        {
            get
            {
                if (_employees == null)
                    InitializeEmployees();
                return _employees;
            }
        }

        private List<Country> Countries
        {
            get
            {
                if (_countries == null)
                    InitializeCountries();
                return _countries;
            }
        }

        private List<JobCategory> JobCategories
        {
            get
            {
                if (_jobCategories == null)
                    InitializeJobCategories();
                return _jobCategories;
            }
        }

        private void InitializeJobCategories()
        {
            _jobCategories = new List<JobCategory>()
            {
                new JobCategory{JobCategoryId = 1, JobCategoryName = "Pie research"},
                new JobCategory{JobCategoryId = 2, JobCategoryName = "Sales"},
                new JobCategory{JobCategoryId = 3, JobCategoryName = "Management"},
                new JobCategory{JobCategoryId = 4, JobCategoryName = "Store staff"},
                new JobCategory{JobCategoryId = 5, JobCategoryName = "Finance"},
                new JobCategory{JobCategoryId = 6, JobCategoryName = "QA"},
                new JobCategory{JobCategoryId = 7, JobCategoryName = "IT"},
                new JobCategory{JobCategoryId = 8, JobCategoryName = "Cleaning"},
                new JobCategory{JobCategoryId = 9, JobCategoryName = "Bakery"},
                new JobCategory{JobCategoryId = 9, JobCategoryName = "Bakery"}

            };
        }

        private void InitializeCountries()
        {
            _countries = new List<Country>
            {
                new Country {CountryId = 1, Name = "Belgium"},
                new Country {CountryId = 2, Name = "Netherlands"},
                new Country {CountryId = 3, Name = "USA"},
                new Country {CountryId = 4, Name = "Japan"},
                new Country {CountryId = 5, Name = "China"},
                new Country {CountryId = 6, Name = "UK"},
                new Country {CountryId = 7, Name = "France"},
                new Country {CountryId = 8, Name = "Brazil"}
            };
        }

        private void InitializeEmployees()
        {
            if (_employees == null)
            {
                EmployeeModel e1 = new EmployeeModel
                {
                    CountryId = 1,
                    MaritalStatus = MaritalStatus.Single,
                    BirthDate = new DateTime(1979, 1, 16),
                    City = "Brussels",
                    Email = "bethany@bethanyspieshop.com",
                    EmployeeId = 1,
                    FirstName = "Bethany",
                    LastName = "Smith",
                    Gender = Gender.Female,
                    PhoneNumber = "324777888773",
                    Smoker = false,
                    Street = "Grote Markt 1",
                    Zip = "1000",
                    JobCategoryId = 1, 
                    Comment = "Lorem Ipsum",
                    ExitDate = null,
                    JoinedDate = new DateTime(2015, 3, 1)
                };
                _employees = new List<EmployeeModel>() { e1 };
            }
        }

        public Task<IEnumerable<EmployeeModel>> GetAllEmployees()
        {
            return Task.Run(() => Employees);
        }

        public Task<List<Country>> GetAllCountries()
        {
            return Task.Run(() => Countries);
        }

        public Task<List<JobCategory>> GetAllJobCategories()
        {
            return Task.Run(() => JobCategories);
        }

        public Task<EmployeeModel> GetEmployeeDetails(int employeeId)
        {
            return Task.Run(() => { return Employees.FirstOrDefault(e => e.EmployeeId == employeeId); });
        }

        public Task<EmployeeModel> AddEmployee(EmployeeModel employee)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEmployee(int employeeId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateEmployee(EmployeeModel employee)
        {
            throw new NotImplementedException();
        }
    }
}
