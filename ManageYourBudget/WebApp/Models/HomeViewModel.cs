using Application.DTOs;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class HomeViewModel
    {
        public IEnumerable<HomeDTO>? Data { get; set; }
    }
}
