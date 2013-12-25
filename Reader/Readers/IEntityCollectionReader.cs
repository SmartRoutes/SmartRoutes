using System.Threading.Tasks;
using SmartRoutes.Model;
using SmartRoutes.Reader.Support;

namespace SmartRoutes.Reader.Readers
{
    public interface IEntityCollectionReader<in TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        Task<TCollection> Read(string filePath, TArchive currentArchive);
    }
}