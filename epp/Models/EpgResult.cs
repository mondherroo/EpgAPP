using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace epp.Models
{
    public class EpgResult
    {
        [JsonProperty("channel_Id")]
        public string channel_Id { get; set; }

        [JsonProperty("channel_Name")]
        public string channel_Name { get; set; }

        [JsonProperty("chanel_Program_Now")]
        public ChanelProgramNow chanel_Program_Now { get; set; }

        [JsonProperty("chanel_Program_Next")]
        public ChanelProgramNext chanel_Program_Next { get; set; }
    }
}
