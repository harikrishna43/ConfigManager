using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace ConfigManager.Core.Components
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ConfigManagerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<ConfigManagerComponent>();
        }
    }
}
