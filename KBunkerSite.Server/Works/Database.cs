using KbpApi.Works;
using Microsoft.EntityFrameworkCore;

namespace KBunkerSite.Server.Works  
{

    class Database
    {
        public static void AddBunker(Bunker bunker)
        {
            using var db = new ApplicationContext();
            db.Bunkers.Add(bunker);
            db.SaveChanges();
        }
        public static List<Placement> GetPlacements()
        {
            try
            {
                using var db = new ApplicationContext();
                var placements = db.Placements.ToList();
                return placements;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new List<Placement>();
            }
        }
        public static List<Address> GetAddresses()
        {
            try
            {
                using var db = new ApplicationContext();
                var addresses = db.Addresses.ToList();
                return addresses.DistinctBy(x => x.Name).ToList();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new List<Address>();
            }
        }
        public static void Reset()
        {
            using var db = new ApplicationContext(true);
            db.SaveChanges();
        }
        public static void Add(Item item)
        {
            using var db = new ApplicationContext();
            db.Items.Add(item);
            db.SaveChanges();
        }
        public static void UpdatePlacement(Placement placement)
        {
            try
            {
                using var db = new ApplicationContext();
                db.Placements.Update(placement);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        public static void AddPlacement(Placement placement)
        {
            try
            {
                using var db = new ApplicationContext();
                db.Placements.Add(placement);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        public static List<Item>? GetItems()
        {
            try
            {
                using var db = new ApplicationContext();
                var items = db.Items;
                return items.ToList();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
        public static bool SetPlacements(List<Placement> placements)
        {
            try
            {
                using var db = new ApplicationContext();
                db.Placements.AddRange(placements);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }
        private static List<Address>? ItemsToAddresses(List<Item> items)
        {
            try
            {
                var response = new List<Address>();

                foreach (var item in items)
                {
                    response.Add(new Address
                    {
                        Code = item.Code,
                        Name = item.NameDot,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                    });
                }

                return response;
            }
            catch
            {
                return null;
            }
        }
        public static bool UpdateAdresses(List<Item> items)
        {
            try
            {
                //var adresses = ItemsToAddresses(items);

                //if (adresses != null)
                //{
                //    using var db = new ApplicationContext();
                //    db.Addresses.UpdateRange(adresses);
                //    db.SaveChanges();
                //    return true;
                //}

                return false;
            }
            catch
            {
                return false;
            }
        }
    }


    public class Address
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }
    public class Item
    {
        public int Id { get; set; }
        public string Code { get; set; } = String.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
        public string CodeDot { get; set; }
        public string NameDot { get; set; }
        public DateTime SaveTime { get; set; } = DateTime.Now;
    }
    public class Bunker
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public int Volume { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
    public class Placement
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public DateTime LastDate { get; set; }
    }
    public sealed class ApplicationContext : DbContext
    {
        private string _connectoinString = "server=127.0.0.1;uid=root;pwd=1234;database=kbp;";
        public DbSet<Item> Items { get; set; } = null!; 
        public DbSet<Bunker> Bunkers { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Placement> Placements { get; set; } = null!;
        public ApplicationContext(bool reset = false)
        {
            if (reset)
                Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseMySql(_connectoinString, new MySqlServerVersion(new Version(8, 0, 20)));
    }

}
