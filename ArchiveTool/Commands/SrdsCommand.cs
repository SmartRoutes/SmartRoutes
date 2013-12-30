using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.ArchiveLoader;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.ArchiveTool.Commands
{
    public class SrdsCommand : ArchiveCommand<SrdsArchive, SrdsCollection>
    {
        public SrdsCommand(IArchiveLoader<SrdsArchive, SrdsCollection> archiveLoader) : base("srds", archiveLoader)
        {
        }
    }
}
