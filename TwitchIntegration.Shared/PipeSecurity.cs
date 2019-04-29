using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace TwitchIntegration.Shared
{
    public static class Security
    {
        public static PipeSecurity GetDefault()
        {
            PipeSecurity ps = new PipeSecurity();

            ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                                                PipeAccessRights.ReadWrite,
                                                AccessControlType.Allow));

            ps.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().Owner,
                                                PipeAccessRights.FullControl,
                                                AccessControlType.Allow));

            return ps;
        }
    }
}
