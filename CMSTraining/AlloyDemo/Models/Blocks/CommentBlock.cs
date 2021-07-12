using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace AlloyDemo.Models.Blocks
{
    [ContentType(
        AvailableInEditMode = false,
        DisplayName = "Comment", 
        Description = "Used to store a comment for a page.")]
    public class CommentBlock : BlockData
    {
        [Display(
            Name = "When",
            Description = "When the comment was created.",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        public virtual DateTime When { get; set; }

        [Display(
            Name = "Name",
            Description = "Name of the person making the comment",
            GroupName = SystemTabNames.Content,
            Order = 200)]
        public virtual string CommentName { get; set; }

        [Display(
            Name = "Text",
            Description = "The actual comment text",
            GroupName = SystemTabNames.Content,
            Order = 300)]
        public virtual string CommentText { get; set; }
    }
}