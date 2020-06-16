﻿using GlobalHook.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace GlobalHook.Core.MessageLoop
{
    public interface IMessageLoop
    {
        /// <summary>
        /// The message loop must be started in the main thread.
        /// It locks the thread, automatically installs hooks (<see cref="IHook.Install"/>) at startup 
        /// and uninstalls them (<see cref="IHook.Uninstall"/>) after it's finished.
        /// </summary>
        /// <param name="hooks">Hooks to be launched.</param>
        /// <param name="processId">The identifier of the target process.</param>
        /// <param name="goOnPredicate">
        /// Callback that indicates whether to continue (<see langword="true"/>) the message loop.
        /// <para/>
        /// Its parameter describes whether an exit from the loop itself has been requested.
        /// </param>
        void Run(IEnumerable<IHook> hooks, long processId, Func<bool, bool> goOnPredicate);

        /// <inheritdoc cref="Run(IEnumerable{IHook}, long, Func{bool, bool})"/>
        void Run(IEnumerable<IHook> hooks, Func<bool, bool> goOnPredicate);

        /// <summary>
        /// This is an aggregated event for all running hooks.
        /// </summary>
        event EventHandler<IHookEventArgs>? OnEvent;


        public static IEnumerable<IMessageLoop> Load(Assembly assembly) => Loader<IMessageLoop>.Load(assembly);

        public static IEnumerable<IMessageLoop> Load(string directory, string searchPattern, EnumerationOptions options) => Loader<IMessageLoop>.Load(directory, searchPattern, options);

        public static IEnumerable<IMessageLoop> Load(string directory, string searchPattern, SearchOption options) => Loader<IMessageLoop>.Load(directory, searchPattern, options);

        public static IEnumerable<IMessageLoop> Load(string directory, string searchPattern) => Loader<IMessageLoop>.Load(directory, searchPattern);

        public static IEnumerable<IMessageLoop> Load(string path) => Loader<IMessageLoop>.Load(path);
    }

    public static class MessageLoopExtensions
    {
        /// <inheritdoc cref="IMessageLoop.Run(IEnumerable{IHook}, Func{bool, bool})"/>
        /// <param name="loop">Message loop instance.</param>
        /// <param name="process">Target process.</param>
        public static void Run(this IMessageLoop loop, long processId, Func<bool, bool> goOnPredicate, params IHook[] hooks) => loop.Run(hooks, processId, goOnPredicate);

        /// <inheritdoc cref="Run(IMessageLoop, long, Func{bool, bool}, IHook[])"/>
        public static void Run(this IMessageLoop loop, Process process, Func<bool, bool> goOnPredicate, params IHook[] hooks) => loop.Run(hooks, process.Id, goOnPredicate);

        /// <inheritdoc cref="Run(IMessageLoop, long, Func{bool, bool}, IHook[])"/>
        public static void Run(this IMessageLoop loop, Func<bool, bool> goOnPredicate, params IHook[] hooks) => loop.Run(hooks, goOnPredicate);

        /// <inheritdoc cref="Run(IMessageLoop, long, Func{bool, bool}, IHook[])"/>
        /// <param name="cancellationToken">The message <paramref name="loop"/> will be stopped immediately after the cancel request for this token.</param>
        public static void Run(this IMessageLoop loop, IEnumerable<IHook> hooks, long processId, CancellationToken cancellationToken) => loop.Run(hooks, processId, quit => !(quit || cancellationToken.IsCancellationRequested));

        /// <inheritdoc cref="Run(IMessageLoop, IEnumerable{IHook}, long, CancellationToken)"/>
        public static void Run(this IMessageLoop loop, IEnumerable<IHook> hooks, Process process, CancellationToken cancellationToken) => loop.Run(hooks, process.Id, quit => !(quit || cancellationToken.IsCancellationRequested));
        
        /// <inheritdoc cref="Run(IMessageLoop, IEnumerable{IHook}, long, CancellationToken)"/>
        public static void Run(this IMessageLoop loop, IEnumerable<IHook> hooks, CancellationToken cancellationToken) => loop.Run(hooks, quit => !(quit || cancellationToken.IsCancellationRequested));

        /// <inheritdoc cref="Run(IMessageLoop, IEnumerable{IHook}, long, CancellationToken)"/>
        public static void Run(this IMessageLoop loop, long processId, CancellationToken cancellationToken, params IHook[] hooks) => loop.Run(hooks, processId, quit => !(quit || cancellationToken.IsCancellationRequested));

        /// <inheritdoc cref="Run(IMessageLoop, long, CancellationToken, IHook[])"/>
        public static void Run(this IMessageLoop loop, Process process, CancellationToken cancellationToken, params IHook[] hooks) => loop.Run(hooks, process.Id, quit => !(quit || cancellationToken.IsCancellationRequested));
        
        /// <inheritdoc cref="Run(IMessageLoop, IEnumerable{IHook}, long, CancellationToken)"/>
        public static void Run(this IMessageLoop loop, CancellationToken cancellationToken, params IHook[] hooks) => loop.Run(hooks, quit => !(quit || cancellationToken.IsCancellationRequested));

        /// <inheritdoc cref="Run(IMessageLoop, long, Func{bool, bool}, IHook[])"/>
        public static void Run(this IMessageLoop loop, long processId, params IHook[] hooks) => loop.Run(hooks, processId, quit => !quit);

        /// <inheritdoc cref="Run(IMessageLoop, long, IHook[])"/>
        public static void Run(this IMessageLoop loop, Process process, params IHook[] hooks) => loop.Run(hooks, process.Id, quit => !quit);

        /// <inheritdoc cref="Run(IMessageLoop, long, Func{bool, bool}, IHook[])"/>
        public static void Run(this IMessageLoop loop, IEnumerable<IHook> hooks, long processId) => loop.Run(hooks, processId, quit => !quit);

        /// <inheritdoc cref="Run(IMessageLoop, IEnumerable{IHook}, long)"/>
        public static void Run(this IMessageLoop loop, IEnumerable<IHook> hooks, Process process) => loop.Run(hooks, process.Id, quit => !quit);

        /// <inheritdoc cref="Run(IMessageLoop, long, Func{bool, bool}, IHook[])"/>
        public static void Run(this IMessageLoop loop, params IHook[] hooks) => loop.Run(hooks, quit => !quit);

        /// <inheritdoc cref="Run(IMessageLoop, long, Func{bool, bool}, IHook[])"/>
        public static void Run(this IMessageLoop loop, IEnumerable<IHook> hooks) => loop.Run(hooks, quit => !quit);
    }
}
