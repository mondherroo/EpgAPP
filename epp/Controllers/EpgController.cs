using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using epp.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace epp.Controllers
{
    [Produces("application/json")]
    [Route("/api/{version}/{clientRoute}/live/epg")]
    public class EpgController : Controller
    {
        private IMemoryCache _memoryCache;
        public EpgController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        [HttpPost, HttpGet]
        public IActionResult GetEpg([FromBody]EpgRequest epgRequest)
        {
            var now = DateTime.Now;

            
            var client = new RestClient("https://localhost:44342/");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful == false) return NotFound();
            var cont = JsonConvert.DeserializeObject<List<EpgProgram>>(response.Content);
            
            
                var result = new EpgResult
                {
                    channel_Id = epgRequest.ChannelId,
                    channel_Name = epgRequest.ChannelName,
                    chanel_Program_Now = new ChanelProgramNow
                    {
                        program_Name = cont.Where(m => m.Finish > now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Title).FirstOrDefault(),
                        program_Descriptio = cont.Where(m => m.Finish > now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Desc).FirstOrDefault(),
                        program_Start = cont.Where(m => m.Finish > now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Start).FirstOrDefault().ToString(),
                        program_Stop = cont.Where(m => m.Finish > now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Finish).FirstOrDefault().ToString()
                    },
                    chanel_Program_Next = new ChanelProgramNext
                    {
                        program_Name = cont.Where(m => m.Start >= now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Title).FirstOrDefault(),
                        program_Descriptio = cont.Where(m => m.Start >= now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Desc).FirstOrDefault(),
                        program_Start = cont.Where(m => m.Start >= now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Start).FirstOrDefault().ToString(),
                        program_Stop = cont.Where(m => m.Start >= now && m.ChannelId.Contains(epgRequest.ChannelName)).Select(m => m.Finish).FirstOrDefault().ToString()
                    }
                };
                if (_memoryCache.TryGetValue("epg", out EpgResult re)) return Ok(re);
                if (result == null) return NotFound();
                 re = result;
            DateTime ts = DateTime.Parse(result.chanel_Program_Now.program_Stop);
            DateTime tn = DateTime.Parse(now.TimeOfDay.ToString());
            var Intime = (ts - tn).TotalHours.ToString();
            double.TryParse(Intime, out double ExTime);
            _memoryCache.Set("epg", re, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(ExTime)));
                return Ok(re);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
