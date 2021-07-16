using EPiServer.Shell.ObjectEditing;
using System.Collections.Generic;

namespace AlloyDemo.Models
{
    public static class EpiserverYouTube
    {
        public static List<SelectItem> Videos = new List<SelectItem>();

        static EpiserverYouTube()
        {
            Videos.Add(new SelectItem
            {
                Value = "xbNRxExM-sY",
                Text = "Episerver Advanced CMS Authoring"
            });
            Videos.Add(new SelectItem
            {
                Value = "v8tWqYVArVY",
                Text = "Episerver Ascend Las Vegas 2017"
            });
            Videos.Add(new SelectItem
            {
                Value = "ErHS21Js0Do",
                Text = "Episerver Find"
            });
            Videos.Add(new SelectItem
            {
                Value = "8CJgklPCAiA",
                Text = "Episerver Forms"
            });
            Videos.Add(new SelectItem
            {
                Value = "vaGZGpQB394",
                Text = "Episerver PowerSlice"
            });
            Videos.Add(new SelectItem
            {
                Value = "Bf0eoyJv8xE",
                Text = "Episerver Projects"
            });
        }
    }
}