namespace GrupoMad.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Store>? Stores { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
