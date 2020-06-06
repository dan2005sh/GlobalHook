﻿using GlobalHook.Core.Windows.Interop.Libs;
using System;
using System.Diagnostics;

namespace GlobalHook.Core.Windows
{
    internal class ExceptionHelper
    {
        public static void ThrowIfProcessHasNoWindow()
        {
            IntPtr currentProcessWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            
            if (currentProcessWindowHandle == IntPtr.Zero || currentProcessWindowHandle == Kernel32.GetConsoleWindow())
                throw new NotSupportedException($"This process doesn't provide a built-in message loop.\n\nTo successfully install a hook, use `MessageLoop.Run(hook)`.");
        }

        public static void ThrowHookMustBeGlobal() => throw new NotSupportedException("Low level hook can't be installed for one specific thread/process.");

        public static void ThrowHookIsAlreadyInstalled() => throw new InvalidOperationException("The hook is already installed.");
    }
}
