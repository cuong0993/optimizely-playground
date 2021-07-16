using System;
using System.Collections.Generic;
using EPiServer.Framework.Localization;
using EPiServer.Shell.ObjectEditing;

namespace AlloyDemo.Business.Selectors
{
    public class EnumSelectionFactory<TEnum> : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(
            ExtendedMetadata metadata)
        {
            var values = Enum.GetValues(typeof(TEnum));
            foreach (var value in values)
            {
                yield return new SelectItem
                {
                    //Text = Enum.GetName(typeof(TEnum), value),
                    Text = GetLocalizedName(value),
                    Value = value
                };
            }
        }

        // this method exists so that we can localize the string values
        private string GetLocalizedName(object value)
        {
            var staticName = Enum.GetName(typeof(TEnum), value);

            string localizationPath = string.Format(
                "/enum/{0}/{1}",
                typeof(TEnum).Name.ToLowerInvariant(),
                staticName.ToLowerInvariant());

            string localizedName;
            if (LocalizationService.Current.TryGetString(
                localizationPath,
                out localizedName))
            {
                return localizedName;
            }

            return staticName;
        }
    }
}