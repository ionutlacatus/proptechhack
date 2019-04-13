using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;
using webmonitor.Models;

namespace webmonitor.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        private string _appId = "8cc1cfa6-c376-40b9-b79e-3d8f1210bf13";
        private readonly SigfoxDb1 _db;

        public ApiController(SigfoxDb1 db)
        {
            _db = db;
        }

        [Route("api/sendNotification")]
        [HttpPost]
        public IActionResult SendNotification(NotificationPayload notificationPayload)
        {
            try
            {
                Sensor sensor = _db.Sensors.FirstOrDefault(s => s.DeviceId == notificationPayload.deviceId);
                if (sensor == null)
                    return new StatusCodeResult(404);

                using (var client = new WebClient())
                {
                    var vm = new
                    {
                        app_id = _appId,
                        data = new { tip = "confirmare", solicitantId = notificationPayload.userId, notificationPayload.hours, notificationPayload.deviceId },
                        contents = new { en = notificationPayload.text },
                        include_player_ids = new[] { sensor.OwnerId },
                        buttons = new[] { new { id = "yes", text = "Da" }, new { id = "no", text = "Nu" } }
                    };
                    var dataString = JsonConvert.SerializeObject(vm);
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Basic MDMxMjg4OTYtMTlkOC00ZjJiLTkzOTMtZmQ4NWZhYmQwNWNk");
                    string res = client.UploadString(new Uri("https://onesignal.com/api/v1/notifications"), "POST", dataString);
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return Ok("error. notif text: " + notificationPayload);
            }
        }

        private void SendNotificationOneSignal(ReservationPayload payload)
        {
           
        }

        [Route("api/callback")]
        [HttpPost]
        public IActionResult Callback(SigfoxPayload payload)
        {
            try
            {
                _db.SigFoxPayloads.Add(payload);
                Sensor sensor = _db.Sensors.FirstOrDefault(s => s.DeviceId == payload.device);
                if (sensor == null)
                {
                    // toate locurile de parcare sunt detinute de 88281e5b-2943-4f1b-b9b0-58538da1b2a8
                    sensor = new Sensor { DeviceId = payload.device, OwnerId = "88281e5b-2943-4f1b-b9b0-58538da1b2a8" };
                    _db.Sensors.Add(sensor);
                }
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        [Route("api/getqr")]
        [HttpGet]
        public ActionResult GetQR(string qrcode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    return Content("data:image/png;base64," + Convert.ToBase64String(ms.ToArray()));
                }
            }
        }

        [Route("api/sensors")]
        public IActionResult GetSensors(bool includeReservations)
        {
            List<Sensor> sensors;
            if (includeReservations)
            {
                sensors = _db.Sensors.Include(s => s.Reservations).ToList();
                DateTime now = DateTime.UtcNow;
                foreach (Sensor sensor in sensors)
                {
                    Reservation reservation = sensor.Reservations.FirstOrDefault(r => r.ReservationStartUtc < now && r.ReservationEndUtc > now);
                    if (reservation != null)
                    {
                        sensor.IsReserved = reservation.UserName;
                    }
                    SigfoxPayload lastSignal = _db.SigFoxPayloads.Where(s => s.device == sensor.DeviceId)
                       .OrderByDescending(p => p.timestamp)
                       .FirstOrDefault();
                    if (lastSignal == null || lastSignal.seqNumber % 2 == 0)
                        sensor.IsUsed = false;
                    else
                        sensor.IsUsed = true;
                }
            }
                
            else
                sensors = _db.Sensors.ToList();


            return Ok(sensors);
        }

        [Route("api/addreservation")]
        [HttpPost]
        public IActionResult AddReservation(ReservationPayload reservation)
        {
            Sensor sensor = _db.Sensors.FirstOrDefault(s => s.DeviceId == reservation.deviceId);
            if (sensor == null)
                return new StatusCodeResult(404);
            Reservation newRes = new Reservation
            {
                ReservationStartUtc = DateTime.UtcNow,
                ReservationEndUtc = DateTime.UtcNow.AddHours(reservation.hours),
                QRCode = Guid.NewGuid().ToString(),
                Sensor = sensor,
                UserName = reservation.userName
            };
            _db.Reservations.Add(newRes);
            _db.SaveChanges();
            return Ok(newRes);
        }

        [Route("api/adduser")]
        [HttpPost]
        public IActionResult AddUser(UserPayload userPayload)
        {
            User user = _db.Users.FirstOrDefault(u => u.UserId == userPayload.userId);
            if (user == null)
            {
                user = new User
                {
                    UserId = userPayload.userId,
                    UserName = userPayload.userName
                };
                _db.Users.Add(user);
            }
            else
            {
                user.UserId = userPayload.userId;
                user.UserName = userPayload.userName;
            }
            _db.SaveChanges();
            return Ok();
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