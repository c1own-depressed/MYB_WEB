using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SettingsDTO
    {
        public int Id { get; set; }

        public bool IsLightTheme { get; set; }

        required public string Language { get; set; }

        required public string Currency { get; set; }
    }
}
