using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NerdyMishka.Util.Text
{
    internal class DefaultStringBuliderPool : IStringBuilderPool
    {
        private readonly StringBuilder[] builders = new StringBuilder[100];

        public StringBuilder Get()
        {
            int count = this.builders.Length;

            for (int builderIndex = 0; builderIndex < count; builderIndex++)
            {
                StringBuilder builder = this.builders[builderIndex];
#pragma warning disable CS8601 // Possible null reference assignment.
                if (builder != null && builder == Interlocked.CompareExchange(ref this.builders[builderIndex], null, builder))
#pragma warning restore CS8601 // Possible null reference assignment.
                {
                    return builder;
                }
            }

            return new StringBuilder();
        }

        public void Return(StringBuilder builder)
        {
            int count = this.builders.Length;

            for (int builderIndex = 0; builderIndex < count; builderIndex++)
            {
                if (this.builders[builderIndex] == null)
                {
                    builder.Clear();
                    this.builders[builderIndex] = builder;
                    break;
                }
            }
        }
    }
}
