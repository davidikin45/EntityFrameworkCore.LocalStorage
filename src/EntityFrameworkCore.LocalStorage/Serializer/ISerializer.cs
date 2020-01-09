using System.Collections.Generic;

namespace EntityFrameworkCore.LocalStorage.Serializer
{
    interface ISerializer
    {
        Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList);

        string Serialize<TKey>(Dictionary<TKey, object[]> list);
    }
}
