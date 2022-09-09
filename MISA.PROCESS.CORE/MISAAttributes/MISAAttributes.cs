using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PROCESS.COMMON.MISAAttributes
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKey : Attribute {
        public string TableName = string.Empty;

        public PrimaryKey(string _tableName)
        {
            this.TableName = _tableName; 
        }
    }

    /// <summary>
    /// Những field k được map
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotMap : Attribute { }

    /// <summary>
    /// field là email
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Email : Attribute { }

    /// <summary>
    /// Giá trị độc nhất
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Unique : Attribute { }

    /// <summary>
    /// field bắt buộc nhập
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Require : Attribute { }

    /// <summary>
    /// Attribute khai báo tên hiển thị cho field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyName : Attribute
    {
        public string Name = string.Empty;

        public PropertyName(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// attr xác định field k update
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotUpdate : Attribute { }

    /// <summary>
    /// attr xác định field phục vụ tìm kiếm
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Search : Attribute { }

    /// <summary>
    /// attr xác định độ dài cho phép cho dữ liệu
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Length : Attribute
    {
        public int length = 0;

        public Length(int _length)
        {
            length = _length;
        }
    }

    /// <summary>
    /// Xác định field sẽ lấy dữ liệu
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForFieldGetData : Attribute { }

    /// <summary>
    /// Xác định field là khóa ngoại
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKey : Attribute {
        public string TypeJoin = String.Empty;

        public ForeignKey(string _typeJoin)
        {
            TypeJoin = _typeJoin;
        }
    }

    /// <summary>
    /// Field sẽ được thêm khi insert
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldInsert : Attribute { }
}
