using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epp.Models
{
    public class ChanelProgramNext
    {
        [JsonProperty("program_Start")]
        public string program_Start { get; set; }

        [JsonProperty("program_Stop")]
        public string program_Stop { get; set; }

        [JsonProperty("program_Name")]
        public string program_Name { get; set; }

        [JsonProperty("program_Descriptio")]
        public string program_Descriptio { get; set; }
    }
}
