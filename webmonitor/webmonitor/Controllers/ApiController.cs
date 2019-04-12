using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webmonitor.Models;

namespace webmonitor.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly SigfoxDb1 _db;

        public ApiController(SigfoxDb1 db)
        {
            _db = db;
        }

        [Route("api/callback")]
        [HttpPost]
        public IActionResult Callback(SigfoxPayload payload)
        {
            try
            {
                _db.SigFoxPayloads.Add(payload);
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        [Route("api/check")]
        public IActionResult GetStatus(string device, int seq)
        {
            do
            {
                SigfoxPayload item = _db.SigFoxPayloads.Where(s => s.device == device)
                       .OrderByDescending(p => p.timestamp)
                       .FirstOrDefault();
                if (item == null)
                    return new StatusCodeResult(404);
                else
                {
                    if (seq == 0)
                    {
                        return Ok(item);
                    }
                    else
                    {
                        if (item.seqNumber == seq)
                            System.Threading.Thread.Sleep(1000);
                        else
                            return Ok(item);
                    }
                }
            }
            while (true);             
        }
    }
}