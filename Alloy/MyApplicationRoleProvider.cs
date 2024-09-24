using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;

namespace Alloy;

public class MyApplicationUserProvider : ApplicationUserProvider<ApplicationUser>
{
    private readonly ServiceAccessor<ApplicationRoleProvider<ApplicationUser>> roleProvider;

    public MyApplicationUserProvider(ServiceAccessor<ApplicationUserManager<ApplicationUser>> userManager) : base(userManager)
    {
    }

    public override async Task<CreateUserResult> CreateUserAsync(
        string username,
        string password,
        string email,
        string passwordQuestion,
        string passwordAnswer,
        bool isApproved)
    {
        CreateUserResult createUserResult = await base.CreateUserAsync(username, password, email, passwordQuestion, passwordAnswer, isApproved);
        
        return createUserResult;
    }
}