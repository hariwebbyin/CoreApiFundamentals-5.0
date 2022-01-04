using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Data
{
  public class CampContextFactory : IDesignTimeDbContextFactory<CampContext> //A factory for creating derived DbContext instances.
                                                                             //Implement this interface to enable design-time services for context types that
                                                                             //do not have a public default constructor. At design-time, derived DbContext instances
                                                                             //can be created in order to enable specific design-time experiences such as Migrations.
                                                                             //Design-time services will automatically discover implementations of this interface that are in
                                                                             //the startup assembly or the same assembly as the derived context.
    {
    public CampContext CreateDbContext(string[] args)
    {
      var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

      return new CampContext(new DbContextOptionsBuilder<CampContext>().Options, config);
    }
  }
}
