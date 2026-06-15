using System;
using System.Collections.Generic;
using System.Text;

namespace Assets
{
    public interface IApplyConfig<T>
    {
        public void ApplyConfig(T Config);
    }
}
