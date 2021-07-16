using System.Web;
using EPiServer.Web;

namespace AlloyDemo.Business.Channels
{
    public class PdfChannel : DisplayChannel
    {
        public override string ChannelName => "PDF";

        public override bool IsActive(HttpContextBase context)
        {
            if (context == null) return false;

            return context.Request.QueryString["pdf"] != null;
        }
    }
}