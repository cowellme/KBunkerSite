namespace KbpApi.Models
{
    public class Dot
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;

        public static List<Dot> GetAll()
        {
            //TODO: Get all from database

            return new List<Dot>();
        }
    }
}
