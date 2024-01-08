using EPiServer.PlugIn;
using Newtonsoft.Json;

namespace Alloy.Models.Pages;

[PropertyDefinitionTypePlugIn(DisplayName = "Int List", GUID = "c2037a22-dab2-42b2-b64c-584de039d27e")]
public class PropertyIntList : PropertyLongString
{
    public override Type PropertyValueType
    {
        get
        {
            return typeof(List<int>);
        }
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

            return JsonConvert.DeserializeObject<IEnumerable<int>>(value);
        }

        set
        {
            if (value is IEnumerable<int>)
            {
                base.Value = JsonConvert.SerializeObject(value);
            }
            else
            {
                base.Value = value;
            }
        }
    }

    public override object SaveData(PropertyDataCollection properties)
    {
        return LongString;
    }
    
    public override void ParseToSelf(string value)
    {
        Value = PropertyNumber.Parse(value).Value;
    }
}