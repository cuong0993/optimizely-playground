using System;
using System.Web.Mvc;
using EPiServer.Shell.ObjectEditing;

namespace AlloyDemo.Business.Selectors
{
    public class SelectManyEnumAttribute : SelectManyAttribute, IMetadataAware
    {
        public SelectManyEnumAttribute(Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentException("Type must be an enum.");
            EnumType = enumType;
        }

        public Type EnumType { get; protected set; }

        public new void OnMetadataCreated(ModelMetadata metadata)
        {
            SelectionFactoryType = typeof(EnumSelectionFactory<>)
                .MakeGenericType(EnumType);
            base.OnMetadataCreated(metadata);
        }
    }
}