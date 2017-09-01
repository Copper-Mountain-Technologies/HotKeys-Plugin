// Used to prohibit the application from running multiple instances of itself.
// see http://sanity-free.org/143/csharp_dotnet_single_instance_application.html
//     http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c/229567

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

internal class SingleGlobalInstanceChecker : IDisposable
{
    public bool isSingleInstance = false;
    private Mutex mutex;

    public SingleGlobalInstanceChecker()
    {
        string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
        string mutexId = string.Format("Global\\{{{0}}}", appGuid);
        mutex = new Mutex(false, mutexId);

        var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
        var securitySettings = new MutexSecurity();
        securitySettings.AddAccessRule(allowEveryoneRule);
        mutex.SetAccessControl(securitySettings);

        try
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                isSingleInstance = true;
            }
            else
            {
                isSingleInstance = false;
            }
        }
        catch (AbandonedMutexException)
        {
            isSingleInstance = true;
        }
    }

    public void Dispose()
    {
        if (mutex != null)
        {
            if (isSingleInstance)
            {
                mutex.ReleaseMutex();
            }
            mutex.Dispose();
        }
    }
}