using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dust.Expandable
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class Description : Attribute
    {
        public readonly string strResId;
        public Description(string strResId)
        {
            this.strResId = strResId;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class NonSettable : Attribute { }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class NumericOption : Attribute
    {
        public readonly decimal maximun, minimun, increment;
        public readonly int decimalPlaces;

        public NumericOption(long maximun, long minimun, long increment)
        {
            this.maximun = maximun;
            this.minimun = minimun;
            this.increment = increment;
            this.decimalPlaces = 0;
        }

        public NumericOption(ulong maximun, ulong minimun, ulong increment)
        {
            this.maximun = maximun;
            this.minimun = minimun;
            this.increment = increment;
            this.decimalPlaces = 0;
        }

        public NumericOption(double maximun, double minimun, double increment)
            : this(maximun, minimun, increment, 3)
        {
            
        }

        public NumericOption(double maximun, double minimun, double increment, int decimalPlaces)
        {
            this.maximun = Convert.ToDecimal(maximun);
            this.minimun = Convert.ToDecimal(minimun);
            this.increment = Convert.ToDecimal(increment);
            this.decimalPlaces = decimalPlaces;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ComboBoxOption : Attribute
    {

        public readonly string key;

        public ComboBoxOption(string key)
        {
            this.key = key;
        }        
    }
}
