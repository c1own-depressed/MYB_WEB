using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ReviewDTOs
{
    public class ReviewDTO
    {
        public string UserName { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; } = int.MinValue;

        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    }
}
