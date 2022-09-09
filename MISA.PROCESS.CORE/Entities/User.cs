using MISA.PROCESS.COMMON.Enum;
using MISA.PROCESS.COMMON.MISAAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.Entities
{
    public class User : InfoInteractive
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey("User")]
        [Length(36)]
        [NotUpdate]
        [Require]
        [ForFieldGetData]
        [FieldInsert]
        public Guid? UserID { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [Length(20)]
        [Require]
        [Unique]
        [PropertyName("Mã nhân viên")]
        [ForFieldGetData]
        [Search]
        [FieldInsert]
        public String? EmployeeCode { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        [Require]
        [Length(100)]
        [PropertyName("Họ và tên")]
        [ForFieldGetData]
        [Search]
        [FieldInsert]
        public String? FullName { get; set; }

        /// <summary>
        /// Id phòng ban
        /// </summary>
        [Require]
        [Length(36)]
        [PropertyName("Phòng ban")]
        [ForFieldGetData,ForeignKey("LEFT JOIN")]
        [FieldInsert]
        public Guid? DepartmentID { get; set; }

        /// <summary>
        /// Id vị trí
        /// </summary>
        [Require]
        [Length(36)]
        [PropertyName("Vị trí")]
        [ForFieldGetData,ForeignKey("LEFT JOIN")]
        [FieldInsert]
        public Guid? JobPositionID { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Email]
        [Length(100)]
        [Require]
        [Search]
        [ForFieldGetData]
        [FieldInsert]
        public String? Email { get; set; }

        /// <summary>
        /// Màu avatar
        /// </summary>
        [Length(20)]
        [Require]
        [ForFieldGetData]
        [FieldInsert]
        public String? AvatarColor { get; set; }

        /// <summary>
        /// Thuộc tính dư thừa, tên các vai trò
        /// </summary>
        [Search]
        [ForFieldGetData]
        [FieldInsert]
        public String? RoleNames { get; set; }

        /// <summary>
        /// Trạng thái người dùng dạng số
        /// </summary>
        [Require]
        [PropertyName("Trạng thái")]
        [ForFieldGetData]
        [FieldInsert]
        public ActiveStatus? ActiveStatus { get; set; }

        /// <summary>
        /// Trạng thái người dùng dạng text
        /// </summary>
        public String? ActiveStatusText { 
            get {
                switch (ActiveStatus)
                {
                    case Enum.ActiveStatus.Active:
                        return Resources.Resources.UserActive;
                    case Enum.ActiveStatus.NotActive:
                        return Resources.Resources.UserNotActive;
                    case Enum.ActiveStatus.WaitAcept:
                        return Resources.Resources.UserWaitAcept;
                    case Enum.ActiveStatus.StopActive:
                        return Resources.Resources.UserStopActive;
                    default:
                        return null;
                };
            } 
        }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [Search]
        [ForFieldGetData]
        public String? DepartmentName { get; }

        /// <summary>
        /// Tên vị trí công việc
        /// </summary>
        [Search]
        [ForFieldGetData]
        public String? JobPositionName { get; }

        /// <summary>
        /// Màu của active staus
        /// </summary>
        public String ActiveStatusColor { 
            get {
                switch (ActiveStatus)
                {
                    case Enum.ActiveStatus.Active:
                        return Resources.Resources.ActiveColor;
                    case Enum.ActiveStatus.NotActive:
                        return Resources.Resources.NotActiveColor;
                    case Enum.ActiveStatus.WaitAcept:
                        return Resources.Resources.WaitAceptColor;
                    case Enum.ActiveStatus.StopActive:
                        return Resources.Resources.StopActiveColor;
                    default:
                        return null;
                }
            } 
        }

        /// <summary>
        /// List roles của user
        /// </summary>
        public List<RoleUpdate> Roles { get; set; }
    }
}
