using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Catalog.Api.Models
{
    /// <summary>
    /// Travel catalog repository implementation.
    /// </summary>
    /// <seealso cref="ICatalogRepo"/>
    public class CatalogRepo : ICatalogRepo
    {
        private readonly Func<SqlConnection> _getDb;
        private readonly Dictionary<int, CatalogTravel> _travelsCache = null;

        // TODO: comme j'ai enlevé le ".*", il manque peut être des colonnes (à spécifier explicitement)
        private const string _selectQuery = @"SELECT
            Noces.description_travel As Description
            , Noces.nom As Name
            , Noces.date_dep As Departure
            , Noces.prix As Price
            , Noces.id_noces As Id
            , Ville.nom As Town
            , Pays.nom AS Country
            FROM Noces WITH(NOLOCK)
            INNER JOIN Ville WITH(NOLOCK) ON Ville.id_ville = Noces.id_ville
            INNER JOIN Pays WITH(NOLOCK) ON Pays.id_pays = Noces.id_pays";

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="getDb">A delegate to get the SQL connection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="getDb"/> is <c>Null</c>.</exception>
        public CatalogRepo(Func<SqlConnection> getDb)
        {
            _getDb = getDb ?? throw new ArgumentNullException(nameof(getDb));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CatalogTravel>> GetTravel(int pageSize, int pageNum)
        {
            CheckPaginationParameters(pageSize, pageNum);
            using var db = _getDb();
            return await db.QueryAsync<CatalogTravel>(
                $"{_selectQuery} ORDER BY Noces.id_noces OFFSET @PageNum * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY",
                new
                {
                    PageNum = pageNum,
                    PageSize = pageSize
                });
        }

        /// <inheritdoc />
        public async Task<CatalogTravel> GetTravelById(int id)
        {
            using var db = _getDb();
            return await db.QueryFirstOrDefaultAsync<CatalogTravel>(
                $"{_selectQuery} WHERE Noces.id_noces = @Id",
                new { Id = id });
        }

        /// <inheritdoc />
        public async Task<CatalogTravel> GetFirstTravelByCountry(string search)
        {
            // TODO: sans clause ORDER BY, tu n'as pas de garantie absolue que le résultat sera le même entre 2 exécutions
            // TODO: "% + search + %" (suggestion :))
            using var db = _getDb();
            return await db.QueryFirstOrDefaultAsync<CatalogTravel>(
                $"{_selectQuery} WHERE Pays.nom LIKE @Search",
                new
                {
                    Search = search + '%'
                });
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CatalogTravel>> FindTravelsByCountry(string search, int pageSize, int pageNum)
        {
            // TODO: "% + search + %" (suggestion :))
            CheckPaginationParameters(pageSize, pageNum);
            using var db = _getDb();
            return await db.QueryAsync<CatalogTravel>(
                $"{_selectQuery} WHERE Pays.nom LIKE @Search OFFSET @PageNum * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY",
                new
                {
                    Search = search + '%',
                    PageNum = pageNum,
                    PageSize = pageSize
                });
        }

        /// <inheritdoc />
        public void RemoveTavel(int id)
        {
            using var db = _getDb();
            db.Execute(
                "Delete FROM Noces WHERE Noces.id_noces=@Id",
                new { Id = id });
            _travelsCache.Remove(id);
        }

        private static void CheckPaginationParameters(int pageSize, int pageNum)
        {
            if (pageSize < 1 || pageSize > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Must be in 1-50.");
            }

            if (pageNum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNum), pageNum, "Must be positive."); ;
            }
        }
    }
}
