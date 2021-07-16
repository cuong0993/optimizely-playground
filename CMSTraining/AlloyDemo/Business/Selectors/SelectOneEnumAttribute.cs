using System;
using System.Web.Mvc;
using EPiServer.Shell.ObjectEditing;

namespace AlloyDemo.Business.Selectors
{
    public class SelectOneEnumAttribute : SelectOneAttribute, IMetadataAware
    {
        public SelectOneEnumAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be an enum.");
            }
            this.EnumType = enumType;
        }

        public Type EnumType { get; protected set; }

        public new void OnMetadataCreated(ModelMetadata metadata)
        {
            this.SelectionFactoryType = typeof(EnumSelectionFactory<>)
                .MakeGenericType(this.EnumType);
            base.OnMetadataCreated(metadata);
        }
    }
}