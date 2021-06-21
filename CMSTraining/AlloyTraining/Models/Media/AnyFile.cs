using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlloyTraining.Models.Media
{
    [ContentType(DisplayName = "AnyFile", GUID = "88dbc414-d586-4f4d-80a2-9f454e8ac631", Description = "Use this to upload any type of file.")]
    public class AnyFile : MediaData
    {

                [CultureSpecific]
                [Editable(true)]
                [Display(
                    Name = "Description",
                    Description = "Description field's description",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual string Description { get; set; }
    }
}