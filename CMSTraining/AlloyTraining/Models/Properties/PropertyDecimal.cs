using EPiServer.Core;
using EPiServer.PlugIn;
using System;

namespace AlloyTraining.Models.Properties
{
    [PropertyDefinitionTypePlugIn(
        Description = "A property for decimal values.",
        DisplayName = "Decimal")]
    public class PropertyDecimal : PropertyLongString
    {
        public override Type PropertyValueType => typeof(decimal);

        public override object SaveData(PropertyDataCollection properties)
        {
            return LongString;
        }

        public override object Value
        {
            get
            {
                var value = base.Value as string;

                if (value == null)
                {
                    return null;
                }

                return decimal.Parse(value);
            }
            set
            {
                base.Value = value.ToString();
            }
        }
    }
}