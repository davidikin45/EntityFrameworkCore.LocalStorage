using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.LocalStorage
{
    public class BlazorModel : Model
    {
        public BlazorModel() : base()
        {

        }
        public BlazorModel(ConventionSet conventions) : base(conventions)
        {

        }

        public override ConfigurationSource? FindIsOwnedConfigurationSource(Type clrType)
        {
            return null;
        }

    }
}
