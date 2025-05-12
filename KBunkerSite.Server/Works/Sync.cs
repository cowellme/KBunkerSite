using KbpApi.Works;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace KBunkerSite.Server.Works
{
    public class Sync
    {
        public static void SyncPlacement(Item item)
        {
            try
            {
                var code = item.Code;
                var address = item.NameDot;
                var placements = Database.GetPlacements();
                if (placements.Any(x => x.Code == code))
                {
                    Database.AddPlacement(new Placement
                    {
                        Address = address,
                        Code = code,
                        LastDate = DateTime.Now
                    });

                    
                    return;
                }


                var placement = placements.First(x => x.Code == code);
                placement.Address = address;
                placement.LastDate = DateTime.Now;
                Database.UpdatePlacement(placement);

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static bool SyncPlacements()
        {
            try
            {
                var items = Database.GetItems();
                var placements = new List<Placement>();

                if (items == null) return false;

                foreach (var item in items)
                {
                    if (placements.Any(x => x.Code == item.Code)) continue;
                    //БиоCпарк: Бункер №152
                    placements.Add(new Placement
                    {
                        Code = item.Code,
                        Address = item.NameDot,
                        LastDate = DateTime.Now,
                    });
                }

                return Database.SetPlacements(placements); ;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public static bool SyncAdresses()
        {
            try
            {
                var items = Database.GetItems();

                if (items != null)
                {
                    var filterItems = items
                        .GroupBy(it => it.Code)
                        .Select(g => g.OrderBy(x => x.SaveTime).First())
                        .ToList();
                    return Database.UpdateAdresses(filterItems);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void AddToHistory(Item item)
        {
            try
            {
                var history = History.Get();
                for (int i = 0; i < 20; i++)
                {
                    history.Items.Add(item);
                }
                history.Save();
            }
            catch
            {
                // ignored
            }
        }
    }

    public class History
    {

        public List<Item> Items = new List<Item>();

        public static History Get()
        {
            var path = Environment.CurrentDirectory + @"\history.json";
            try
            {
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
                return responseHistory.Formated();
            }
            catch
            {
                return new History();
            }
        }

        public void Save()
        {
            try
            {
                var path = Environment.CurrentDirectory + @"\history.json";
                var stringJson = JsonConvert.SerializeObject(this);
                File.WriteAllText(path, stringJson);
            }
            catch 
            {
                //
            }
        }

        public History Formated()
        {
            var items = new List<Item>();
            
            foreach (var item in Items)
            {
                var name = item.Code.ToLower();
                var bio = name.Contains("биоспарк");
                var bunker = name.Contains("бункер");
                if (bio && bunker)
                {
                    name = name.Replace(" ", "");
                    name = name.Replace(":", "");
                    name = name.Replace("№", "");
                    name = name.Replace("биоспарк", "");
                    name = name.Replace("бункер", "");

                    name = "БиоСпарк: Бункер №" + name;
                }

                if (name.ToCharArray().Length > 32)
                    name = name.Remove(32);

                
                items.Add(new Item
                {
                    Code = name,
                    CodeDot = item.CodeDot,
                    Id = item.Id,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    NameDot = item.NameDot,
                    SaveTime = item.SaveTime,
                });
            }

            Items = items;
            return this;
        }
    }
}
