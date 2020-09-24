using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesignIntentApi.Dtos
{
    public class ForgeTokenDto
    {
        public string AccessToken { get; set; }

        public DateTime Expires { get; set; }
    }
}
