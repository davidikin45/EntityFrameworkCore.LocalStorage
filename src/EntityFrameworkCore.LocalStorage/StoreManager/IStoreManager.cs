using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.LocalStorage.StoreManager
{
    public interface IStoreManager
    {
        Dictionary<TKey, object[]> Deserialize<TKey>(Dictionary<TKey, object[]> newList);

        void Serialize<TKey>(Dictionary<TKey, object[]> list);
    }
}
