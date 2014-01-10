using System;
using System.Collections.Generic;
using System.Linq;
using ManyConsole;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.ArchiveTool.Commands;

namespace SmartRoutes.ArchiveTool
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            IEnumerable<ConsoleCommand> commands = GetCommands();

            // then run them.
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        private static IEnumerable<ConsoleCommand> GetCommands()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind(c => c
                .From("SmartRoutes.ArchiveTool.exe", "SmartRoutes.ArchiveLoader.dll", "SmartRoutes.Reader.dll")
                .SelectAllClasses()
                .BindAllInterfaces());

            return kernel.GetAll<IArchiveCommand>().OfType<ConsoleCommand>();
        }
    }
}