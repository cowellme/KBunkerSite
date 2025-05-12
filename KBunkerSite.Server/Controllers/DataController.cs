using System.Text;
using KbpApi.Models;
using KbpApi.Works;
using KBunkerSite.Server.Works;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KBunkerSite.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        [HttpPatch]
        public IActionResult SetData(Item item)
        {
            try
            {
                Database.Add(item);
                Sync.SyncPlacement(item);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("sync")]
        public IActionResult SyncPlacement()
        {
            try
            {
                Sync.SyncPlacements();
                return Ok("Synchronized !");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("history")]
        public IActionResult GetHistory()
        {
            try
            {
                var history = History.Get();
                var jsonString = JsonConvert.SerializeObject(history);
                return Ok(jsonString);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("sync-adresses")]
        public IActionResult SyncAdresses()
        {
            try
            {
                return Ok(Sync.SyncAdresses());
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Route("add-bunker")]
        public IActionResult SetBunker(Bunker bunker)
        {
            try
            {
                Database.AddBunker(bunker);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("get-bunkers")]
        public IActionResult GetBunker()
        {
            try
            {
                var response = new List<Address>();
                var items = Database.GetItems();
                var newHistory = new History();

                var filterItems = items
                    .GroupBy(it => it.Code)
                    .Select(g => g.OrderByDescending(x => x.SaveTime).First())
                    .ToList();

                foreach (var item in filterItems) newHistory.Items.Add(item);
                newHistory.Save();

                var sortedItemsByTime = newHistory.Items.OrderByDescending(x => x.SaveTime).ToList();

                var responseHistory = new History
                {
                    Items = sortedItemsByTime
                };

                foreach (var address in sortedItemsByTime)
                {
                    response.Add(new Address
                    {
                        Code = address.Code,
                        Id = address.Id,
                        Latitude = address.Latitude,
                        Longitude = address.Longitude,
                        Name = address.Code
                    });
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public IActionResult GetAddresses()
        {
            try
            {
                var response = Database.GetAddresses();
                return Ok(response);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("reset")]
        public IActionResult Reset()
        {
            Database.Reset();

            return Ok("Database reseted!");
        }

        [HttpGet("csv")]
        public IActionResult ExportToCsv()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding(1251); // Windows-1251

            var data = History.Get();
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,Название,Широта,Долгота,Код,Адрес,Время обновления");

            foreach (var item in data.Items)
            {
                csvBuilder.AppendLine($"{item.Id},{EscapeCsv(item.Code)},{item.Latitude},{item.Longitude},{item.CodeDot},{EscapeCsv(item.NameDot)},{item.SaveTime}");
            }

            var bytes = encoding.GetBytes(csvBuilder.ToString());
            return File(bytes, "text/csv; charset=windows-1251", "export.csv");
        }

        private string EscapeCsv(string value)
        {
            // Экранирование специальных символов в CSV
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

    }
}

