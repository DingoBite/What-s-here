#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace _Project.Utils
{
    public class EnumGenerator : MonoBehaviour
    {
        [SerializeField] private List<string> _values;

        [SerializeField] private List<string> _enumNames;
        
        [SerializeField] private string _enumName;
        [SerializeField] private string _destination;

        [Button]
        public void GenerateEnum()
        {
            var filePathAndName = $"{_destination}/{_enumName}.cs";
            var stringBuilder = new StringBuilder();
            foreach (var t in _enumNames)
            {
                stringBuilder.Append($"\n\t{t},");
            }

            var enumString = 
$@"public enum {_enumName}
{{{stringBuilder}
}}";
            
            File.WriteAllText(filePathAndName, enumString);
            AssetDatabase.Refresh();
        }
        
        [Button]
        public void GenerateEnumToValues()
        {
            var enumToValuesName = $"{_enumName}ToValues";
            var filePathAndName = $"{_destination}/{enumToValuesName}.cs";
            
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < _enumNames.Count; i++)
            {
                var t = _enumNames[i];
                stringBuilder.Append($"\n\t\t{{{_enumName}.{t}, \"{_values[i]}\"}},");
            }

            var enumToValuesString = 
$@"
using System.Collections.Generic;

public static class {enumToValuesName}
{{
    static {enumToValuesName}()
    {{
        _valuesToEnum = new Dictionary<string, {_enumName}>();
        foreach (var (key, value) in _enumToValues)
        {{
            _valuesToEnum.Add(value, key);
        }}
    }}
    
    private static readonly Dictionary<{_enumName}, string> _enumToValues = new Dictionary<{_enumName}, string>
    {{{stringBuilder}
    }};

    private static readonly Dictionary<string, {_enumName}> _valuesToEnum;

    public static IReadOnlyDictionary<string, {_enumName}> ValuesToEnum => _valuesToEnum;
	public static IReadOnlyDictionary<{_enumName}, string> EnumToValues => _enumToValues;
}}
";
            File.WriteAllText(filePathAndName, enumToValuesString);
            AssetDatabase.Refresh();
        }
    }
}
#endif