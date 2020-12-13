using System.Collections.Generic;

namespace WoolworthsWebAPI.Services
{
    public static class ListExtensions
    {
        public static void InitList(this List<int> list, int number, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                list.Add(number);
            }
        }
    }
}
