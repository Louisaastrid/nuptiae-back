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
        private SqlConnection db;
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

        public IEnumerable<CatalogTravel> GetTravel(int pageSize, int pageNum)
        {
            if (pageSize < 1 || pageSize > 50)
            {
                throw new ArgumentOutOfRangeException("PageSize must be in 1-50");
            }
            if (pageNum < 0)
            {
                throw new ArgumentOutOfRangeException("PageNum must be positive");
            }
            return db.Query<CatalogTravel>
                (
                $"{selectQuery} ORDER BY Noces.id_noces OFFSET @PageNum * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY ",
                         new { PageNum = pageNum, PageSize = pageSize }
                       );
        }
        public CatalogTravel GetTravelById(int id) =>
            db.QueryFirstOrDefault<CatalogTravel>($"{selectQuery} WHERE Noces.id_noces = @id", new { id = id });

    }
}
