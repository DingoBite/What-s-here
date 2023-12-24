using System.Threading.Tasks;
using UnityEngine;

namespace Configurator.Core
{
    public abstract class ConfigReloadProvider : MonoBehaviour
    {
        public abstract void ReloadConfig();
        public abstract Task ReloadConfigAsync();


        public abstract void UpdateConfig();
        public abstract Task UpdateConfigAsync();

        public abstract void SetName(string configName);
    }
}