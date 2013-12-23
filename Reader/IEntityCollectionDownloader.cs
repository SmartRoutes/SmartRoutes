using System;
using System.Threading.Tasks;
using SmartRoutes.Model;

namespace SmartRoutes.Reader
{
    public interface IEntityCollectionDownloader<in TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        Task<TCollection> Download(Uri uri, TArchive currentArchive);
    }
}