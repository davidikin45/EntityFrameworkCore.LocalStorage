using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.LocalStorage.Storage.Internal
{
    public interface ISerializableTable : IInMemoryTable
    {
        public void Save();
        public bool Exists(IUpdateEntry entry);
    }
}
