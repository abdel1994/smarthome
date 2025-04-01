namespace smarthome.Models;

    public class Gebruiker
    {
        public int Id { get; set; } // Primary key
        public string Naam { get; set; } = "";
        public string Email { get; set; } = "";
        public string Wachtwoord { get; set; } = "";
    }
