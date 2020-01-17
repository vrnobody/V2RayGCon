using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ProxySetter.Lib.Sys
{
    // https://github.com/NoNeil/ProxySetting/blob/master/MyProxy/ProxySetting.cs
    public static class WinInet
    {
        const int QueryOptionCount = 3;
        const int QueryIndexProxyMode = 0;
        const int QueryIndexProxyServer = 1;
        const int QueryIndexPacUrl = 2;

        #region public methods
        public static Model.Data.ProxySettings GetProxySettings()
        {
            var query = GenQureyOption(true);
            var success = false;
            var result = new Model.Data.ProxySettings();

            void action(IntPtr _listPtr, int _listSize)
            {
                var newSize = _listSize;
                success = InternetQueryOption(
                    IntPtr.Zero,
                    InternetOption.INTERNET_OPTION_PER_CONNECTION_OPTION,
                    _listPtr, ref newSize);

                if (success)
                {
                    InternetPerConnOptionList optList =
                        (InternetPerConnOptionList)Marshal.PtrToStructure(
                            _listPtr, typeof(InternetPerConnOptionList));
                    result = IntPtrToProxySettings(optList.options);
                }
            }

            SystemProxyHandler(GenQureyOption(true), action);
            if (!success)
            {
                SystemProxyHandler(GenQureyOption(false), action);
            }
            return result;
        }

        public static bool SetProxySettings(
            Model.Data.ProxySettings proxySettings)
        {
            var options = GenProxyOption(proxySettings);
            var result = false;

            void action(IntPtr listPtr, int listSize)
            {
                result = InternetSetOption(
                    IntPtr.Zero,
                    InternetOption.INTERNET_OPTION_PER_CONNECTION_OPTION,
                    listPtr,
                    listSize);
            }

            SystemProxyHandler(options, action);
            ForceSysProxyRefresh();
            return result;
        }

        public static void ForceSysProxyRefresh()
        {
            InternetSetOption(IntPtr.Zero, InternetOption.INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, InternetOption.INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
        #endregion

        #region helper functions
        static Model.Data.ProxySettings IntPtrToProxySettings(IntPtr ptr)
        {
            var options = new List<InternetConnectionOption>();
            int optSize = Marshal.SizeOf(typeof(InternetConnectionOption));

            for (int i = 0; i < QueryOptionCount; ++i)
            {
                IntPtr opt = new IntPtr(ptr.ToInt64() + (i * optSize));
                options.Add(
                    (InternetConnectionOption)Marshal.PtrToStructure(
                        opt, typeof(InternetConnectionOption)));
            }

            var result = new Model.Data.ProxySettings
            {
                proxyMode = options[QueryIndexProxyMode].m_Value.m_Int,

                proxyAddr = Marshal.PtrToStringAuto(
                    options[QueryIndexProxyServer].m_Value.m_StringPtr)
                    ?? string.Empty,

                pacUrl = Marshal.PtrToStringAuto(
                    options[QueryIndexPacUrl].m_Value.m_StringPtr)
                    ?? string.Empty,
            };
            return result;
        }

        static InternetConnectionOption[] GenQureyOption(bool isWin7OrNewer)
        {
            InternetConnectionOption[] options = new InternetConnectionOption[QueryOptionCount];
            options[QueryIndexProxyMode].m_Option = isWin7OrNewer ? PerConnOption.INTERNET_PER_CONN_FLAGS_UI : PerConnOption.INTERNET_PER_CONN_FLAGS;
            options[QueryIndexProxyServer].m_Option = PerConnOption.INTERNET_PER_CONN_PROXY_SERVER;
            options[QueryIndexPacUrl].m_Option = PerConnOption.INTERNET_PER_CONN_AUTOCONFIG_URL;
            return options;
        }

        static InternetConnectionOption[] GenProxyOption(
            Model.Data.ProxySettings proxySettings)
        {
            InternetConnectionOption[] options = new InternetConnectionOption[QueryOptionCount];

            // USE a proxy server ...
            options[QueryIndexProxyMode].m_Option = PerConnOption.INTERNET_PER_CONN_FLAGS;
            options[QueryIndexProxyMode].m_Value.m_Int = proxySettings.proxyMode;

            options[QueryIndexProxyServer].m_Option =
                PerConnOption.INTERNET_PER_CONN_PROXY_SERVER;
            options[QueryIndexProxyServer].m_Value.m_StringPtr =
                Marshal.StringToHGlobalAuto(proxySettings.proxyAddr ?? @"");

            options[QueryIndexPacUrl].m_Option =
                PerConnOption.INTERNET_PER_CONN_AUTOCONFIG_URL;
            options[QueryIndexPacUrl].m_Value.m_StringPtr =
                Marshal.StringToHGlobalAuto(proxySettings.pacUrl ?? @"");

            return options;
        }

        internal static void SystemProxyHandler(InternetConnectionOption[] options, Action<IntPtr, int> action)
        {
            InternetPerConnOptionList list = new InternetPerConnOptionList();

            // default stuff
            list.dwSize = Marshal.SizeOf(list);
            list.szConnection = IntPtr.Zero;
            list.dwOptionCount = options.Length;
            list.dwOptionError = 0;

            int optSize = Marshal.SizeOf(typeof(InternetConnectionOption));
            // make a pointer out of all that ...
            IntPtr optionsPtr = Marshal.AllocCoTaskMem(optSize * options.Length);
            // copy the array over into that spot in memory ...
            for (int i = 0; i < options.Length; ++i)
            {
                IntPtr opt = new IntPtr(optionsPtr.ToInt64() + (i * optSize));
                Marshal.StructureToPtr(options[i], opt, false);
            }

            list.options = optionsPtr;

            // and then make a pointer out of the whole list
            IntPtr ipcoListPtr = Marshal.AllocCoTaskMem((Int32)list.dwSize);
            Marshal.StructureToPtr(list, ipcoListPtr, false);

            // and finally, call the API method!
            action(ipcoListPtr, list.dwSize);

            // FREE the data ASAP
            Marshal.FreeCoTaskMem(optionsPtr);
            Marshal.FreeCoTaskMem(ipcoListPtr);
        }
        #endregion

        #region WinInet structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetPerConnOptionList
        {
            public int dwSize;               // size of the INTERNET_PER_CONN_OPTION_LIST struct
            public IntPtr szConnection;         // connection name to set/query options
            public int dwOptionCount;        // number of options to set/query
            public int dwOptionError;           // on error, which option failed
            //[MarshalAs(UnmanagedType.)]
            public IntPtr options;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetConnectionOption
        {
            static readonly int Size;
            public PerConnOption m_Option;
            public InternetConnectionOptionValue m_Value;
            static InternetConnectionOption()
            {
                InternetConnectionOption.Size = Marshal.SizeOf(typeof(InternetConnectionOption));
            }

            // Nested Types
            [StructLayout(LayoutKind.Explicit)]
            public struct InternetConnectionOptionValue
            {
                // Fields
                [FieldOffset(0)]
                public System.Runtime.InteropServices.ComTypes.FILETIME m_FileTime;
                [FieldOffset(0)]
                public int m_Int;
                [FieldOffset(0)]
                public IntPtr m_StringPtr;
            }
        }
        #endregion

        #region WinInet enums
        public enum ProxyModes : int
        {
            Direct = PerConnFlags.PROXY_TYPE_DIRECT,
            Proxy = PerConnFlags.PROXY_TYPE_PROXY | PerConnFlags.PROXY_TYPE_DIRECT,
            PAC = PerConnFlags.PROXY_TYPE_AUTO_PROXY_URL | PerConnFlags.PROXY_TYPE_DIRECT,
        }

        public enum OperationType
        {
            GetProxyOption,
            SetProxyOption,
        }

        //
        // options manifests for Internet{Query|Set}Option
        //
        public enum InternetOption : uint
        {
            INTERNET_OPTION_SETTINGS_CHANGED = 39,
            INTERNET_OPTION_REFRESH = 37,
            INTERNET_OPTION_PER_CONNECTION_OPTION = 75,
        }

        //
        // Options used in INTERNET_PER_CONN_OPTON struct
        //
        public enum PerConnOption
        {
            INTERNET_PER_CONN_FLAGS = 1, // Sets or retrieves the connection type. The Value member will contain one or more of the values from PerConnFlags 
            INTERNET_PER_CONN_PROXY_SERVER = 2, // Sets or retrieves a string containing the proxy servers.  
            INTERNET_PER_CONN_PROXY_BYPASS = 3, // Sets or retrieves a string containing the URLs that do not use the proxy server.  
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4, // Sets or retrieves a string containing the URL to the automatic configuration script.  
            INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5,
            INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6,
            INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9,
            INTERNET_PER_CONN_FLAGS_UI = 10,
        }

        //
        // PER_CONN_FLAGS
        //
        [Flags]
        public enum PerConnFlags
        {
            PROXY_TYPE_DIRECT = 0x00000001,  // direct to net
            PROXY_TYPE_PROXY = 0x00000002,  // via named proxy
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,  // autoproxy URL
            PROXY_TYPE_AUTO_DETECT = 0x00000008   // use autoproxy detection
        }
        #endregion

        #region wininet.dll
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool InternetQueryOption(IntPtr hInternet, InternetOption dwOption, IntPtr lpBuffer, ref int lpdwBufferLength);

        [DllImport("WinInet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool InternetSetOption(IntPtr hInternet, InternetOption dwOption, IntPtr lpBuffer, int dwBufferLength);
        #endregion

    }
}
