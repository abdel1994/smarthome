using Microsoft.EntityFrameworkCore;
using smarthome.Models;

namespace smarthome.Data;

public class ApiDBContext: DbContext 
{
    public ApiDBContext(DbContextOptions<ApiDBContext> options)
     : base(options)
      {

      }

      // een DBset is een collectie van objecten die je kan gebruiken om te communiceren met de database
      public DbSet<Gebruiker> Gebruikers {get; set;}

}