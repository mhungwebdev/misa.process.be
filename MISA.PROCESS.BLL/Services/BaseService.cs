using MISA.core.Exceptions;
using MISA.PROCESS.BLL.Interfaces.InterfaceService;
using MISA.PROCESS.COMMON.Entities;
using MISA.PROCESS.COMMON.Enum;
using MISA.PROCESS.COMMON.MISAAttributes;
using MISA.PROCESS.COMMON.Resources;
using MISA.PROCESS.DAL.Interfaces.InterfaceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.PROCESS.BLL.Services
{
    public class BaseService<MISAEntity> : IBaseService<MISAEntity>
    {
        #region contructor
        IBaseRepository<MISAEntity> _baseRepository;

        public BaseService(IBaseRepository<MISAEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }
        #endregion

        #region Insert Service
        /// <summary>
        /// Thêm mới bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">đối tượng muốn thêm mới</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        public int Insert(MISAEntity entity)
        {
            Dictionary<string, string> errors = ValidateGeneral(entity, null);

            if (errors.Count() > 0)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("devMsg", "");
                result.Add("userMsg", Resources.InputInValid);
                result.Add("errors", errors);

                throw new MISAException(result);
            }

            var res = _baseRepository.Insert(entity);
            return res;
        }
        #endregion

        #region Update Service
        /// <summary>
        /// Sửa một bản ghi có thực hiện nghiệp vụ
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">Đối tượng muốn update</param>
        /// <returns>1 nếu thành công, 0 nếu thất bại</returns>
        public int Update(MISAEntity entity, Guid id)
        {
            Dictionary<string, string> errors = ValidateGeneral(entity, id);

            if (errors.Count() > 0)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("devMsg", "");
                result.Add("userMsg", Resources.InputInValid);
                result.Add("errors", errors);

                throw new MISAException(result);
            }

            var res = _baseRepository.Update(entity, id);
            return res;
        }
        #endregion

        #region Validate General
        /// <summary>
        /// Validate dữ liệu chung dựa vào custom Attribute
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">Đối tượng cần validate</param>
        /// <param name="id">id của đối tượng validate</param>
        /// <returns>Đối tượng errors có kiểu Dictionary<string,string>, key là tên field, value là thông báo lỗi</returns>
        protected Dictionary<string, string> ValidateGeneral(MISAEntity entity, Guid? id)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            //kiểm tra field require
            var propRequires = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Require)));
            foreach (var propRequire in propRequires)
            {
                var value = propRequire.GetValue(entity);
                string fieldName = propRequire.Name;

                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    string nameDisplay = propRequire.Name;

                    var propNames = propRequire.GetCustomAttributes(typeof(PropertyName), true);

                    if (propNames.Length > 0)
                    {
                        nameDisplay = (propNames[0] as PropertyName).Name;
                    }

                    errors.Add(fieldName, string.Format(Resources.EmptyFieldRequireMsg, nameDisplay));
                }
            }

            //kiểm tra field unique
            var propUniques = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Unique)));
            foreach (var propUnique in propUniques)
            {
                var value = propUnique.GetValue(entity);
                string fieldName = propUnique.Name;
                if (value != null || String.IsNullOrEmpty(value.ToString()))
                {
                    if (_baseRepository.CheckUnique(value, propUnique.Name, id))
                    {
                        string nameDisplay = fieldName;

                        var propNames = propUnique.GetCustomAttributes(typeof(PropertyName), true);

                        if (propNames.Length > 0)
                            nameDisplay = (propNames[0] as PropertyName).Name;

                        errors.Add(fieldName, string.Format(Resources.UniqueErrorMsg, nameDisplay));
                    }
                }
            }

            //kiểm tra field email
            var propEmails = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Email)));
            foreach (var propEmail in propEmails)
            {
                var value = propEmail.GetValue(entity);
                var fieldName = propEmail.Name;

                if (!(String.IsNullOrEmpty(value.ToString())))
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(value.ToString());

                    if (!(match.Success))
                    {
                        var nameDisplay = fieldName;

                        var propNames = propEmail.GetCustomAttributes(typeof(PropertyName), true);
                        if (propNames.Length > 0)
                            nameDisplay = (propNames[0] as PropertyName).Name;

                        errors.Add(fieldName, String.Format(Resources.EmailErrorMsg, nameDisplay));
                    }
                }
            }

            //kiểm tra độ dài
            var propLengths = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Length)));
            foreach (var propLength in propLengths)
            {
                var fieldName = propLength.Name;
                var value = propLength.GetValue(entity);
                var lengths = propLength.GetCustomAttributes(typeof(Length), true);
                int fieldLength = 255;

                if (lengths.Length > 0)
                {
                    fieldLength = (lengths[0] as Length).length;
                }

                if (value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString().Length > fieldLength)
                {
                    var propNames = propLength.GetCustomAttributes(typeof(PropertyName), true);
                    string nameDisplay = fieldName;
                    if (propNames.Length > 0)
                    {
                        nameDisplay = (propNames[0] as PropertyName).Name;
                    }

                    errors.Add(fieldName, String.Format(Resources.TooLongValid, nameDisplay, fieldLength));
                }
            }

            //kiểm tra custome
            Dictionary<string, string> errorCustom = ValidateCustom(entity, id);

            if (errorCustom != null)
                errors = errors.Concat(errorCustom).ToDictionary(x => x.Key, x => x.Value);

            return errors;
        }
        #endregion

        #region Validate Cumtom
        /// <summary>
        /// Validate dữ liệu chung riêng cho phép overite
        /// Author : mhungwebdev (29/8/2022)
        /// </summary>
        /// <param name="entity">Đối tượng cần validate</param>
        /// <param name="id">id của đối tượng validate</param>
        /// <returns>Đối tượng errors có kiểu Dictionary<string,string>, key là tên field, value là thông báo lỗi</returns>
        public virtual Dictionary<string, string> ValidateCustom(MISAEntity entity, Guid? id)
        {
            return null;
        }
        #endregion

        /// <summary>
        /// Validate một list record
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="entities">list record</param>
        /// <returns>List object lỗi có key là Tên khóa chính - value là id của khóa chính, key là fieldError - value là object chứa thông báo lỗi có key là các field</returns>
        protected List<Object> ValidateListRecord(List<MISAEntity> entities)
        {
            List<Object> listError = new List<Object>();
            foreach (MISAEntity entity in entities)
            {
                Dictionary<string, string> errors = new Dictionary<string, string>();
                var propPK = typeof(MISAEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PrimaryKey))).FirstOrDefault();
                var namePK = propPK.Name;
                Guid id = (Guid)propPK.GetValue(entity);
                errors = ValidateGeneral(entity, id);

                if (errors.Count() > 0)
                {
                    Dictionary<string, object> error = new Dictionary<string, object>();
                    error.Add(namePK, id);
                    error.Add("fieldError", errors);

                    listError.Add(error);
                }
            }

            return listError;
        }

        
        public virtual int InsertMulti(List<MISAEntity> entities)
        {
            List<Object> listError = ValidateListRecord(entities);
            if (listError.Count() > 0)
                HandleThrowError(InteractiveMode.Multi, listError);
            
            var res = _baseRepository.InsertMulti(entities);

            return res;
        }

        /// <summary>
        /// Xử lý trả về lỗi
        /// Author : mhungwebdev (9/9/2022)
        /// </summary>
        /// <param name="interactiveMode">trạng thái tương tác với DB</param>
        /// <param name="errors">list error</param>
        /// <param name="error">đối tượng error</param>
        /// <exception cref="MISAException">Thông báo lỗi về Client</exception>
        public virtual void HandleThrowError(InteractiveMode interactiveMode, List<Object> errors = null,Dictionary<string,string> error = null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("devMsg", "");
            result.Add("userMsg", Resources.InputInValid);

            if(interactiveMode == InteractiveMode.Multi)
                result.Add("errors", errors);

            if (interactiveMode != InteractiveMode.Single)
                result.Add("errors", error);

            throw new MISAException(result);
        }
    }
}
