using System;
using System.Runtime.InteropServices;

namespace RIVS.ASAK.Core.Tools.WinApi
{
    public class Reboot
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetLogger("Reboot");
        internal const int SE_PRIVILEGE_ENABLED = 2;
        internal const int TOKEN_QUERY = 8;
        internal const int TOKEN_ADJUST_PRIVILEGES = 32;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        [DllImport("advapi32.dll", EntryPoint = "InitiateSystemShutdownEx")]
        private static extern int InitiateSystemShutdown(
          string lpMachineName,
          string lpMessage,
          int dwTimeout,
          bool bForceAppsClosed,
          bool bRebootAfterShutdown);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();


        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(
          IntPtr htok,
          bool disall,
          ref Reboot.TokPriv1Luid newst,
          int len,
          IntPtr prev,
          IntPtr relen);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        private void SetPriv()
        {
            IntPtr zero = IntPtr.Zero;

            var processToken = Reboot.OpenProcessToken(Reboot.GetCurrentProcess(), 40, ref zero);
            if(!processToken)
            {
                _logger.Error($"Reboot.OpenProcessToken(Reboot.GetCurrentProcess(), 40, ref zero) errorCode: {GetLastError()}");
                return;
            }
               
            Reboot.TokPriv1Luid newst;
            newst.Count = 1;
            newst.Attr = 2;
            newst.Luid = 0L;

            var privValue = Reboot.LookupPrivilegeValue((string)null, "SeShutdownPrivilege", ref newst.Luid);
            if (!privValue)
            {
                _logger.Error($"Reboot.LookupPrivilegeValue((string)null, \"SeShutdownPrivilege\", ref newst.Luid) errorCode: {GetLastError()}");
            }

            var adjustTokenPriv =  Reboot.AdjustTokenPrivileges(zero, false, ref newst, 0, IntPtr.Zero, IntPtr.Zero);
            if (!adjustTokenPriv)
            {
                _logger.Error($"Reboot.AdjustTokenPrivileges(zero, false, ref newst, 0, IntPtr.Zero, IntPtr.Zero) errorCode: {GetLastError()}");
            }
        }

        public int halt(bool RSh, bool Force)
        {
            SetPriv();
            //Нужно-ли логировать возвращаемое значение?
            return InitiateSystemShutdown(null, null, 0, Force, RSh);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }
    }
}
