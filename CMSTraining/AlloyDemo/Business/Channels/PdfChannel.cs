using EPiServer.Web;
using System.Web;

namespace AlloyDemo.Business.Channels
{
    public class PdfChannel : DisplayChannel
    {
        public override string ChannelName
        {
            get
            {
                return "PDF";
            }
        }

        public override bool IsActive(HttpContextBase context)
        {
            if (context == null) return false;

            return (context.Request.QueryString["pdf"] != null);
        }
    }
}