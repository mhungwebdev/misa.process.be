using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.COMMON.MISAAttributes;
using MISA.PROCESS.DAL.Interfaces.InterfaceRepository;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.DAL.Repository
{
    public class BaseRepository<MISAEntity> : IBaseRepository<MISAEntity>
    {
        #region Contructor
        protected string connectString;

        protected MySqlConnection sqlConnection;

        public BaseRepository(IConfiguration configuration)
        {
            connectString = configuration.GetConnectionString("MISA_PROCESS_LOCALHOST");
        }
        #endregion

        #region Check unique
        /// <summary>
        /// Kiểm tra trùng lặp field unique
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="value">value của field muốn check</param>
        /// <param name="fieldName">Tên field muốn check</param>
        /// <param name="id">id của đối tượng muốn check</param>
        /// <returns>true nếu trùng và false với trường hợp ngược lại</returns>
        public bool CheckUnique(object value, string fieldName, Guid? id)
        {
            using (var sqlConnection = new MySqlConnection(connectString))
            {
                var propPK = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PrimaryKey))).FirstOrDefault();
                var tableName = (propPK.GetCustomAttributes(typeof(PrimaryKey), true).FirstOrDefault() as PrimaryKey).TableName;

                DynamicParameters parameters = new DynamicParameters();

                var sqlCommand = $"select * from {tableName} where {fieldName} = @{fieldName}";

                if (id != null)
                {
                    sqlCommand += $" AND {tableName}Id != @{tableName}Id";
                    parameters.Add($"@{tableName}Id", id);
                }

                parameters.Add($"@{fieldName}", value.ToString());
                var res = sqlConnection.QueryFirstOrDefault(sqlCommand, parameters);

                return res != null;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Xóa 1 bản ghi
        /// Author : mhungwebdev (28/8/2022)
        /// </summary>
        /// <param name="id">id của đối tượng muốn xóa</param>
        /// <returns>1 nếu thành công</returns>
        public int Delete(Guid id)
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                DynamicParameters parameters = new DynamicParameters();

                var sqlCommand = $"Delete from {tableName} where {tableName}Id = @{tableName}Id";
                parameters.Add($"@{tableName}Id", id);
                var res = sqlConnection.Execute(sqlCommand, parameters);

                return res;
            }
        }
        #endregion

        #region Get
        /// <summary>
        /// Lấy tất cả dữ liệu
        /// Author : mhungwebdev (28/8/2022)
        /// </summary>
        /// <returns>Trả về tất cả dữ liệu của bảng</returns>
        public IEnumerable<MISAEntity> Get()
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                var store = $"Proc_GetAll{tableName}";

                var res = sqlConnection.Query<MISAEntity>(store, commandType: CommandType.StoredProcedure);

                return res;
            }
        }
        #endregion

        #region Get by id
        /// <summary>
        /// Lấy bản ghi theo id
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="id">id đối tượng muốn lấy</param>
        /// <returns>Trả về đối tượng trùng id</returns>
        public virtual MISAEntity Get(Guid id)
        {
            using (sqlConnection = new MySqlConnection(connectString))
            {
                var tableName = typeof(MISAEntity).Name;
                var store = $"Proc_Get{tableName}ById";
                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("Id", id.ToString());
                var res = sqlConnection.QueryFirstOrDefault<MISAEntity>(store, param: parameters, commandType: CommandType.StoredProcedure);

                return res;
            }
        }
        #endregion

        #region Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">Đối tượng muốn thêm mới</param>
        /// <returns>1 nếu thành công</returns>
        public virtual int Insert(MISAEntity entity)
        {
            sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();
            using (var transaction = sqlConnection.BeginTransaction())
            {
                var tableName = typeof(MISAEntity).Name;
                var store = $"Proc_Insert{tableName}";

                var res = sqlConnection.Execute(store, param: entity, transaction: transaction, commandType: CommandType.StoredProcedure);

                transaction.Commit();
                sqlConnection.Dispose();
                sqlConnection.Close();

                return res;
            }
        }
        #endregion

        #region Insert Multi
        /// <summary>
        /// Thêm mới hàng loạt
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="entities">list record thêm mới</param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        public virtual int InsertMulti(List<MISAEntity> entities)
        {
            var sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();
            using(var transaction = sqlConnection.BeginTransaction())
            {
                DynamicParameters parameters = new DynamicParameters();
                var sqlInsertMulti = BuildSqlInsertMulti(entities,parameters);

                var res = sqlConnection.Execute(sqlInsertMulti,param:parameters,transaction: transaction);

                if (res < entities.Count)
                    transaction.Rollback();

                transaction.Commit();
                sqlConnection.Dispose();
                sqlConnection.Close();

                return res;
            }
        }
        #endregion

        #region BuildSqlInsertMulti
        /// <summary>
        /// Build câu lệnh thêm mới hàng loạt
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="entities">List record thêm mới hàng loạt</param>
        /// <param name="parameters">param của câu lệnh</param>
        /// <returns>Câu lệnh thêm mới hàng loạt</returns>
        protected string BuildSqlInsertMulti(List<MISAEntity> entities, DynamicParameters parameters)
        {
            var propPK = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PrimaryKey))).FirstOrDefault();
            var tableName = (propPK.GetCustomAttributes(typeof(PrimaryKey), true).FirstOrDefault() as PrimaryKey).TableName;
            var propInserts = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(FieldInsert)));

            string sqlInsertMulti = BuildFieldInsert(tableName, propInserts);
            sqlInsertMulti = BuildValueInsertMulti(entities, parameters, propInserts, sqlInsertMulti);

            return sqlInsertMulti;
        }
        #endregion

        #region BuildValueInsertMulti
        /// <summary>
        /// Build các bản ghi value insert vào câu lệnh insert multi
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="entities">List record thêm mới</param>
        /// <param name="parameters">param của câu lệnh thêm mới hàng loạt</param>
        /// <param name="propInserts">các property insert</param>
        /// <param name="sqlInsertMulti">Câu lệnh sql có các field và từ khóa values sẵn</param>
        /// <returns>Câu lệnh thêm mới hàng loạt hoàn chỉnh</returns>
        private static string BuildValueInsertMulti(List<MISAEntity> entities, DynamicParameters parameters, IEnumerable<System.Reflection.PropertyInfo> propInserts, string sqlInsertMulti)
        {
            int recordIndex = 0;
            foreach (MISAEntity entity in entities)
            {
                var valueRecord = "(";
                foreach (var propInsert in propInserts)
                {
                    var fieldName = propInsert.Name;
                    var value = propInsert.GetValue(entity);

                    if (fieldName.Equals("CreatedDate") || fieldName.Equals("ModifiedDate"))
                        value = DateTime.Now;

                    valueRecord += $"@{fieldName}{recordIndex},";
                    parameters.Add($"@{fieldName}{recordIndex}", value);
                }
                recordIndex++;
                valueRecord = valueRecord[..^1] + "),";
                sqlInsertMulti += valueRecord;
            }
            sqlInsertMulti = sqlInsertMulti[..^1];

            return sqlInsertMulti;
        }
        #endregion

        #region BuildFieldInsert
        /// <summary>
        /// Build câu lệnh insert có các field và từ khóa values sẵn
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="tableName">tên table</param>
        /// <param name="propInserts">list property insert</param>
        /// <returns>câu lệnh insert có các field và từ khóa values sẵn</returns>
        private static string BuildFieldInsert(string tableName, IEnumerable<System.Reflection.PropertyInfo> propInserts)
        {
            var sqlInsertMulti = $"insert into {tableName} (";
            foreach (var propInsert in propInserts)
            {
                var fieldName = propInsert.Name;
                sqlInsertMulti += $"{fieldName},";
            }
            sqlInsertMulti = sqlInsertMulti[..^1] + ") values ";
            return sqlInsertMulti;
        }
        #endregion

        #region Update
        /// <summary>
        /// Sửa một bản ghi
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">Thông tin mới của đối tượng được sửa </param>
        /// <returns>1 nếu thành công</returns>
        public int Update(MISAEntity entity, Guid id)
        {
            return 1;
        }
        #endregion
    }
}
