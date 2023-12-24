using System;
using System.Linq;

namespace _Project.Utils
{
    public class EnumSwitcher<T> where T : Enum
    {
        public T SelectedValue { get; private set; }
        
        public void Select(T value)
        {
            SelectedValue = value;
        }
        
        public T MoveNext()
        {
            var enums = EnumInfo<T>.Values.ToList();
            var index = enums.IndexOf(SelectedValue);
            index++;
            index %= enums.Count;
            var value = enums[index];
            Select(value);
            return value;
        }
    }
}