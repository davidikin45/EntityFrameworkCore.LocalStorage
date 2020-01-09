using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace EntityFrameworkCore.LocalStorage
{
    public class BlazorModelSource : ModelSource
    {
        public BlazorModelSource(ModelSourceDependencies dependencies):
            base(dependencies)
        {

        }

        protected override IModel CreateModel(DbContext context, IConventionSetBuilder conventionSetBuilder)
        {

            var conventions = conventionSetBuilder.CreateConventionSet();

            var c1 = conventions.PropertyAddedConventions.Where(c => c.GetType() == typeof(NonNullableReferencePropertyConvention)).ToList();
            c1.ForEach(c => conventions.PropertyAddedConventions.Remove(c));

            var c2 = conventions.PropertyFieldChangedConventions.Where(c => c.GetType() == typeof(NonNullableReferencePropertyConvention)).ToList();
            c2.ForEach(c => conventions.PropertyFieldChangedConventions.Remove(c));

            var c3 = conventions.NavigationAddedConventions.Where(c => c.GetType() == typeof(NonNullableNavigationConvention)).ToList();
            c3.ForEach(c => conventions.NavigationAddedConventions.Remove(c));

            var blazorModel = new BlazorModel(conventions);
            var internalModelBuilder = new InternalModelBuilder(blazorModel);
            internalModelBuilder.Metadata.SetProductVersion(ProductInfo.GetVersion());
            var modelBuilder = new ModelBuilder(blazorModel);

            return modelBuilder.FinalizeModel();
        }
    }
}
