using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Utils
{
    public static class EnumInfo<T> where T : Enum
    {
        public static readonly IReadOnlyList<T> Values;

        static EnumInfo()
        {
            Values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}