using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;


namespace Identity.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityRepo : IIdentityRepo
    {

        private SqlConnection db;
        private List<IdentityUser> _user = new List<IdentityUser>();

        private const string _selectQuery = @"SELECT
            Utilisateurs.nom As Name
            , Utilisateurs.prenom As Surname
            , Utilisateurs.email As Email
            , Utilisateurs.adresse As Adress
            , Utilisateurs.id_utilisateur As Id
            , Utilisateurs.code_postale As ZipCode
            , Utilisateurs.num_telephone AS PhoneNumber
            FROM Utilisateurs WITH(NOLOCK)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public IdentityRepo(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            db.Dispose();
        }


        public IEnumerable<IdentityUser> GetUser(int pageSize, int pageNum)
        {
            if (pageSize < 1 || pageSize > 50)
            {
                throw new ArgumentOutOfRangeException("PageSize must be in 1-50");
            }
            if (pageNum < 0)
            {
                throw new ArgumentOutOfRangeException("PageNum must be positive");
            }
            return db.Query<IdentityUser>
                (
                $"{_selectQuery} ORDER BY Utilisateurs.id_utilisateur OFFSET @PageNum * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY ",
                         new { PageNum = pageNum, PageSize = pageSize }
                       );

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="entity"></param>
        /// <param name="newUser"></param>
        /// <returns></returns>
        private void SaveInList<T>(List<T> list, T entity) where T : IEntityWithId
        {
            if (entity.UniqueId.HasValue)
            {
                // Modified entity : save modifications
                list[FindIndex(list, entity.UniqueId)] = entity;

                // or using for :
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].UniqueId == entity.UniqueId)
                    {
                        list[i] = entity;
                        break;
                    }
                }
            }
            else
            {
                entity.InitWithUniqueId(list);
                list.Add(entity);
            }
        }
        private int FindIndex<T>(List<T> list, int? id) where T : IEntityWithId
        {
            int index = list.FindIndex(u => u.UniqueId == id);

            if (index < 0)
            {
                throw new ArgumentException("Entry no longer exists");
            }
            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void Save(IdentityUser user) => SaveInList(_user, user);


        public void NewUser(IdentityUser newUser)
        {

            var ctx = new IdentityUser();
            ctx.Add
            if (newUser.Id_User == 0)
            {
                var sql = @" INSERT INTO Utilisateurs
                        (nom, prenom, email, adresse, code_postale, num_telephone, mot_de_passe)
                    VALUES
                        (@Name, @Surname, @Email, @Adress, @ZipCode, @PhoneNumber, @Password);
                SELECT CAST (SCOPE_IDENTITY() as int )";
                var returnId = db.ExecuteScalar<int>(sql, newUser);


                newUser.Id_User = returnId;
                _allUsers.Add(newUser.Id_User, newUser);
            }
            else
            {
                // Mise à jour
                db.Execute("UPDATE IdentityUser SET nom=@Name" +
                    "nom=@Name" +
                    "prenom=@Surname" +
                    "email=@Email" +
                    "adresse=@Adress" +
                    "code_postale=@ZipCode" +
                    "num_telephone=@PhoneNumber" +
                    "mot_de_passe=@Password" +

                    " WHERE id_utilisateur=@Id_User", newUser);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="client"></param>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="adress"></param>
        /// <param name="adress2"></param>
        /// <param name="postalCode"></param>
        /// <param name="city"></param>
        /// <param name="country"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void PostFormConnexion(IdentityUser newUser)
        {
            _db.Execute("INSERT INTO Utilisateurs (Admin, Client, LastName, FirstName, PhoneNumber, Adress, Adress2, PostalCode, City, Country, Email, Password) " +
                "VALUES (@Admin, @Client, @LastName, @FirstName, @PhoneNumber, @Adress, @Adress2, @PostalCode, @City, @Country, @Email, @Password)", new { Admin = admin, Client = client, LastName = lastName, FirstName = firstName, PhoneNumber = phoneNumber, Adress = adress, Adress2 = adress2, PostalCode = postalCode, City = city, Country = country, Email = email, Password = password });
        }


    }
}
