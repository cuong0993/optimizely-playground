using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;
using TedGustaf.Episerver.GoogleMapsEditor.Models;
using TedGustaf.Episerver.GoogleMapsEditor.Shell;

namespace AlloyDemo.Models.Blocks
{
    [EditorDescriptorRegistration(TargetType = typeof(IGoogleMapsCoordinates), UIHint = UIHint, EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = UIHint, EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    public class CustomGoogleMapsEditorDescriptor : GoogleMapsEditorDescriptorBase
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            // API key for the Google Maps JavaScript API
            metadata.EditorConfiguration["apiKey"] = "your-api-key";

            // Default zoom level from 1 (least) to 20 (most)
            metadata.EditorConfiguration["defaultZoom"] = 5;

            // Default coordinates when no property value is set
            metadata.EditorConfiguration["defaultCoordinates"] = new { latitude = 59.336, longitude = 18.063 };
        }
    }
}