using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Domain.Exceptions
{
    public class DomainException : Exception
    {
        /// <summary>
        /// Khởi tạo với thông báo lỗi.
        /// </summary>
        public DomainException(string message) : base(message)
        {
        }
        /// <summary>
        /// Khởi tạo với thông báo lỗi và exception gốc (dùng khi wrap exception).
        /// </summary>
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
