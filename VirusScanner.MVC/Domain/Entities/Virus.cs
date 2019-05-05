namespace VirusScanner.MVC.Domain.Entities
{
    public class Virus
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string Name { get; set; }

        public File File { get; set; }
    }
}
