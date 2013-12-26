using System;
using System.Threading.Tasks;
using SmartRoutes.Model;

namespace SmartRoutes.ArchiveLoader
{
    public interface IArchiveLoader<TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        Task Download(Uri uri, bool force);
        Task Read(string filePath, bool force);
    }
}