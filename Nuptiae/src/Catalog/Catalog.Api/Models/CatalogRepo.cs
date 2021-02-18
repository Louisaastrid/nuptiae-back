using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Models
{
    public class CatalogRepo : ICatalogRepo
    {
        private readonly SqlConnection db;
        private Dictionary<int, CatalogTravel> _allTravels = new Dictionary<int, CatalogTravel>();
        //private Dictionary<int, CatalogTravel> _allTravels = new Dictionary<int, CatalogTravel>();



        const string selectQuery = @"SELECT Noces.*,Noces.description_travel As Description,  Noces.nom As Name , Noces.date_dep As Departure, Noces.prix As Price , Noces.id_noces As Id, Ville.nom As Town , Pays.nom AS Country
                FROM Noces  
                INNER JOIN Ville ON  Ville.id_ville =Noces.id_ville
                INNER JOIN  Pays ON   Pays.id_pays = Noces.id_pays";

        public CatalogRepo(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }
        public void Dispose()
        {
            db.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CatalogTravel>> GetTravel(int pageSize, int pageNum)
        {
            if (pageSize < 1 || pageSize > 50)
            {
                throw new ArgumentOutOfRangeException("PageSize must be in 1-50");
            }
            if (pageNum < 0)
            {
                throw new ArgumentOutOfRangeException("PageNum must be positive");
            }
            return await db.QueryAsync<CatalogTravel>
                (
                $"{selectQuery} ORDER BY Noces.id_noces OFFSET @PageNum * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY ",
                         new { PageNum = pageNum, PageSize = pageSize }
                       );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CatalogTravel> GetTravelById(int id) =>
           await db.QueryFirstOrDefaultAsync<CatalogTravel>($"{selectQuery} WHERE Noces.id_noces = @id", new { id = id });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<CatalogTravel> FindTravelByCountry(string search) =>
         await db.QueryFirstOrDefaultAsync<CatalogTravel>($"{selectQuery} WHERE Pays.nom LIKE @Search", new { Search = search + '%' });



        public async Task<IEnumerable<CatalogTravel>> FindTravelsByCountry(string search, int pageSize, int pageNum)
        {
            if (pageSize < 1 || pageSize > 50)
            {
                throw new ArgumentOutOfRangeException("PageSize must be in 1-50");
            }
            if (pageNum < 0)
            {
                throw new ArgumentOutOfRangeException("PageNum must be positive");
            }
            return await db.QueryAsync<CatalogTravel>
               ($"{selectQuery} WHERE Pays.nom LIKE @Search",
                         new { Search = search + '%', PageNum = pageNum, PageSize = pageSize });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void RemoveTavel(int id)
        {
            db.Execute("Delete FROM Noces WHERE Noces.id_noces=@id", new { id = id });
            _allTravels.Remove(id);

        }

    }
}
