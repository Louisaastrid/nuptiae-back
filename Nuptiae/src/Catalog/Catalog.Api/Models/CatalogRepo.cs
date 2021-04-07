using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        private Dictionary<int, CatalogTravel> _travelsCache = null;
        private readonly bool _useCache;

        //CONSTANTES 
        internal const string alldestinations = "allDestinationTravel";
        internal const string destinationByid = "destinationsNuptiaeById";
        internal const string searchCountry = "searchCountry";
        internal const string destinationsByPage = "destinationsByPage";
        internal const string destinationTravelByCountry = "destinationTravelByCountry";
        internal const string getCountryIdtravelTest = "getCountryIdtravelTest";



        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="getDb">A delegate to get the SQL connection.</param>
        /// <param name="useCache">Indicates if cache should be used.</param>
        /// <exception cref="ArgumentNullException"><paramref name="getDb"/> is <c>Null</c>.</exception>
        public CatalogRepo(Func<SqlConnection> getDb, bool useCache)
        {
            _getDb = getDb ?? throw new ArgumentNullException(nameof(getDb));
            _useCache = useCache;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CatalogTravel>> GetTravelAsync(int pageSize, int pageNum)
        {
            CheckPaginationParameters(pageSize, pageNum);

            if (_useCache)
            {
                if (_travelsCache == null)
                {
                    using var db = _getDb();
                    var results = await db
                        .QueryAsync<CatalogTravel>(alldestinations, commandType: CommandType.StoredProcedure)
                        .ConfigureAwait(false);

                    _travelsCache = results?.ToDictionary(_ => _.Id.Value, _ => _)
                        ?? new Dictionary<int, CatalogTravel>();
                }

                return _travelsCache.Values
                    .Skip(pageNum * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                using var db = _getDb();
                var results = await db
                    .QueryAsync<CatalogTravel>(
                       destinationsByPage,
                        new
                        {
                            PageNum = pageNum,
                            PageSize = pageSize
                        }, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                return results?.ToList();
            }
        }

        /// <inheritdoc />
        public async Task<CatalogTravel> GetTravelByIdAsync(int id)
        {
            if (_useCache && _travelsCache != null)
            {
                return _travelsCache.ContainsKey(id) ? _travelsCache[id] : null;
            }

            using var db = _getDb();
            return await db
                .QueryFirstOrDefaultAsync<CatalogTravel>(
                    destinationByid,
                    new { Id = id }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CatalogTravel> GetFirstTravelByCountryAsync(string search)
        {
            if (_useCache && _travelsCache != null)
            {
                return _travelsCache
                    .Values
                    .Where(_ => _.Country.StartsWith(search, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();
            }

            // TODO: sans clause ORDER BY, tu n'as pas de garantie absolue que le résultat sera le même entre 2 exécutions
            using var db = _getDb();
            return await db
                .QueryFirstOrDefaultAsync<CatalogTravel>(
                   searchCountry,
                    new
                    {
                        Search = search + '%'
                    }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<CatalogTravel>> FindTravelsByCountryAsync(string search, int pageSize, int pageNum)
        {
            CheckPaginationParameters(pageSize, pageNum);

            if (_useCache && _travelsCache != null)
            {
                return _travelsCache
                    .Values
                    .Where(_ => _.Country.StartsWith(search, StringComparison.InvariantCultureIgnoreCase))
                    .Skip(pageSize * pageNum)
                    .Take(pageSize)
                    .ToList();
            }


            using var db = _getDb();
            var results = await db
                .QueryAsync<CatalogTravel>(
                    destinationTravelByCountry,
                    new
                    {
                        Search = search + '%',
                        PageNum = pageNum,
                        PageSize = pageSize
                    }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return results?.ToList();
        }

        /// <inheritdoc />
        public async Task RemoveTravelAsync(int id)
        {
            using var db = _getDb();
            await db
                .ExecuteAsync(
                    "Delete FROM Noces WHERE Noces.id_noces=@Id",
                    new { Id = id })
                .ConfigureAwait(false);

            _travelsCache?.Remove(id);
        }

        /// <inheritdoc />
        public async Task<int?> AddNewTravelAsync(CatalogTravel newTravel)
        {
            if (newTravel == null)
            {
                throw new ArgumentNullException(nameof(newTravel));
            }
            // TODO: vérifier qu'un travel identique n'existe pas déjà ? (might be hard)

            int? countryId = await GetCountryIdAsync(newTravel.Country).ConfigureAwait(false);
            if (!countryId.HasValue)
            {
                countryId = await InsertCountryAsync(newTravel.Country).ConfigureAwait(false);
            }



            int? townId = await GetTownIdAsync(newTravel.Country).ConfigureAwait(false);
            if (!townId.HasValue)
            {
                townId = await InsertTownAsync(newTravel.Country).ConfigureAwait(false);
            }

            // TODO: pas certain à 100% d'avoir mis toutes les colonnes dans le INSERT
            using var db = _getDb();
            var results = await db
                .QueryAsync<int>(@"
                    INSERT INTO Noces
                        (description_travel, nom, date_dep, prix, id_ville, id_pays)
                    VALUES
                        (@Description, @Name, @Departure, @Price, @Town, @Country);
                    SELECT SCOPE_IDENTITY();",
                    new
                    {
                        newTravel.Description,
                        newTravel.Name,
                        newTravel.Departure,
                        newTravel.Price,
                        Town = townId,
                        Country = countryId
                    })
                .ConfigureAwait(false);

            var id = results?.FirstOrDefault();

            if (id.HasValue)
            {
                newTravel.Id = id;
                _travelsCache?.Add(id.Value, newTravel);
            }

            return id;
        }

        private Task<int?> InsertTownAsync(string country)
        {
            throw new NotImplementedException();
        }

        private Task<int?> InsertCountryAsync(string country)
        {
            throw new NotImplementedException();
        }

        private async Task<int?> GetCountryIdAsync(string country)
        {
            // TODO: J'ai un peu la flemme mais tu vois l'idée :)
            if (country == null)
            {
                throw new NotImplementedException();
            }
            return 2;
        }

        private async Task<int?> GetTownIdAsync(string town)
        {
            // TODO: J'ai un peu la flemme mais tu vois l'idée :)
            //throw new NotImplementedException();
            return 2;
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
