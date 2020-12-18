using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RicartAgrawalaAlgorithm
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InternalMessage
    {
        public IntPtr hwnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public int time;
        public Point pt;
        public int lPrivate;
    }

    public class ThreadApi
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PeekMessage(out InternalMessage lpInternalMessage, IntPtr hWnd, uint wMsgFilterMin,
            uint wMsgFilterMax, uint wRemoveMsg);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostThreadMessage(uint threadId, uint msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern uint GetTickCount();

        public static bool PostThreadMessage(ApiMessage message)
        {
            return PostThreadMessage(message.ToId, (uint)message.Type, (UIntPtr)message.FromId,
                (IntPtr)message.Section);
        }

        public static bool PeekMessage(out ApiMessage message)
        {
            var messageExists = PeekMessage(out var internalMessage, IntPtr.Zero, 0, 0, 1);
            var isSystemMessage = messageExists && !Enum.IsDefined(typeof(MessageType), (int)internalMessage.message);
            while (isSystemMessage)
            {
                messageExists = PeekMessage(out internalMessage, IntPtr.Zero, 0, 0, 1);
                isSystemMessage = messageExists && !Enum.IsDefined(typeof(MessageType), (int)internalMessage.message);
            }

            if (messageExists)
            {
                message = new ApiMessage(internalMessage);
                return true;
            }

            message = null;
            return false;
        }
    }
}