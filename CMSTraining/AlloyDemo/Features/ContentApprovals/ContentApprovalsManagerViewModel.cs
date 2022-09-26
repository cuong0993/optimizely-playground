using AlloyDemo.Models.ViewModels;
using EPiServer.Approvals.ContentApprovals;

namespace AlloyDemo.Features.ContentApprovals
{
    public class ContentApprovalsManagerViewModel : PageViewModel<ContentApprovalsManagerPage>
    {
        public ContentApprovalsManagerViewModel(
            ContentApprovalsManagerPage currentPage) : base(currentPage)
        {
        }

        public ContentApprovalDefinition ApprovalDefinition { get; set; }
        public ContentApproval Approval { get; set; }
    }
}