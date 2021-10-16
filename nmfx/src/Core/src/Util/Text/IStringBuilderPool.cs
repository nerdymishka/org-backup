using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Util.Text
{
    public interface IStringBuilderPool
    {
        StringBuilder Get();

        void Return(StringBuilder builder);
    }
}
