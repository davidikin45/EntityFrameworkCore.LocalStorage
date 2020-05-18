using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.LocalStorage.Infrastructure.Internal
{
    public class LocalStorageOptionsExtension : IDbContextOptionsExtension
    {
        private ExtensionInfo _info;
        private readonly IJSRuntime _jSRuntime;
        private readonly LocalStorageOptions _options;

        public LocalStorageOptionsExtension(IJSRuntime jSRuntime, LocalStorageOptions options)
        {
            _jSRuntime = jSRuntime;
            _options = options;
        }

        public virtual LocalStorageOptions Options => _options;

        public virtual DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        public virtual void ApplyServices(IServiceCollection services) {
            services.AddEntityFrameworkLocalStorageDatabase(_jSRuntime, _options);         
        }

        public virtual void Validate(IDbContextOptions options)
        {
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private string _logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new LocalStorageOptionsExtension Extension
                => (LocalStorageOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append("Serializer=").Append(Extension.Options.Serializer).Append(' ');

                        if(!string.IsNullOrEmpty(Extension.Options.Password))
                            builder.Append("Password=").Append("<Password>").Append(' ');

                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override long GetServiceProviderHashCode() => 0L;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
                => new Dictionary<string, string>();
        }
    }
}
