using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace PgLib2.SSH;

internal static class LocalPortAllocator
{
    [DllImport("ws2_32.dll")]
    static extern int WSAStartup(ushort wVersionRequested, out WSADATA lpWSAData);

    [StructLayout(LayoutKind.Sequential)]
    struct WSADATA
    {
        public short wVersion;
        public short wHighVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
        public string szDescription;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
        public string szSystemStatus;
        public short iMaxSockets;
        public short iMaxUdpDg;
        public IntPtr lpVendorInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct SOCKADDR_IN
    {
        public short sin_family;
        public ushort sin_port;
        public IN_ADDR sin_addr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] sin_zero;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IN_ADDR
    {
        public uint S_addr;
    }

    [DllImport("ws2_32.dll", SetLastError = true)]
    static extern IntPtr socket(int af, int type, int protocol);

    [DllImport("ws2_32.dll", SetLastError = true)]
    static extern int bind(IntPtr s, ref SOCKADDR_IN addr, int namelen);

    [DllImport("ws2_32.dll", SetLastError = true)]
    static extern int getsockname(IntPtr s, ref SOCKADDR_IN addr, ref int namelen);

    [DllImport("ws2_32.dll")]
    static extern int closesocket(IntPtr s);

    [DllImport("ws2_32.dll")]
    static extern ushort htons(ushort hostshort);

    [DllImport("ws2_32.dll")]
    static extern ushort ntohs(ushort netshort);

    [DllImport("ws2_32.dll")]
    static extern uint inet_addr(string cp);
    static LocalPortAllocator()
    {
        WSAStartup(0x202, out _);
    }
    public static uint Allocate()
    {
        var sock = socket(
            (int)AddressFamily.InterNetwork,
            (int)SocketType.Stream,
            (int)ProtocolType.Tcp);

        if (sock == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            var addr = new SOCKADDR_IN
            {
                sin_family = (short)AddressFamily.InterNetwork,
                sin_port = htons(0),
                sin_addr = new IN_ADDR { S_addr = inet_addr("127.0.0.1") }
            };

            if (bind(sock, ref addr, Marshal.SizeOf<SOCKADDR_IN>()) != 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var len = Marshal.SizeOf<SOCKADDR_IN>();
            if (getsockname(sock, ref addr, ref len) != 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return ntohs(addr.sin_port);
        }
        finally
        {
            closesocket(sock);
        }
    }
}
