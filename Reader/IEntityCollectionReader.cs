using System.Threading.Tasks;
using SmartRoutes.Model;

namespace SmartRoutes.Reader
{
    public interface IEntityCollectionReader<in TArchive, TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        Task<TCollection> Read(string filePath, TArchive currentArchive);
    }
}