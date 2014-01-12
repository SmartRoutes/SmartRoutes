using System;
using SmartRoutes.Model;

namespace SmartRoutes.ArchiveTool.Commands
{
    public interface IArchiveCommand : ICommand
    {
        string FilePath { get; set; }
        Uri Url { get; set; }
        bool Force { get; set; }
    }
}