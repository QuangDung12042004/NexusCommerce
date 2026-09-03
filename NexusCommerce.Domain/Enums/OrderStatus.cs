using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusCommerce.Domain.Enums
{
    public enum OrderStatus
    {
        /// <summary>
        /// Đơn vừa được tạo, chờ khách thanh toán.
        /// Đặt = 0 để giá trị mặc định của int là Pending (hợp logic nghiệp vụ).
        /// </summary>
        Padding = 0,
        /// <summary>
        /// Khách đã thanh toán thành công.
        /// </summary>
        Paid = 1,
        /// <summary>
        /// đơn đã được giao cho đơn vị vận chuyển.
        /// <summary/>
        Shipper = 2,
        /// <summary>
        /// khách đã nhận được hàng.
        /// </summary>
        Delivered = 3,
        /// <summary>
        /// khách đã huy đơn hàng
        /// </summary>
        Canceled = 4
    }
}
