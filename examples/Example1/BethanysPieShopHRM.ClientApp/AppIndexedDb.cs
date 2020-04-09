using BethanysPieShopHRM.Shared;
using Blazor.IndexedDB.Framework;
using Microsoft.JSInterop;

namespace BethanysPieShopHRM.ClientApp
{
    //Reshiru.Blazor.IndexedDB.Framework
    public class AppIndexedDb : IndexedDb
    {
        public AppIndexedDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

        public IndexedSet<Country> Countries { get; set; }
        public IndexedSet<JobCategory> JobCategories { get; set; }
        public IndexedSet<Benefit> Benefits { get; set; }
        public IndexedSet<EmployeeBenefit> EmployeeBenefits { get; set; }
        public IndexedSet<Employee> Employees { get; set; }
    }
}
