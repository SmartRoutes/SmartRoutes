using System.Threading.Tasks;
using SmartRoutes.Model.Srds;
using SmartRoutes.SrdsReader.Support;

namespace SmartRoutes.SrdsReader.Readers
{
    public interface ISrdsCollectionReader
    {
        Task<SrdsCollection> Read(string filePath, SrdsArchive currentArchive);
    }
}