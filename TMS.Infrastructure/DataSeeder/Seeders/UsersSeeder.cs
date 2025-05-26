using Microsoft.AspNetCore.Identity;
using TMS.Core.Entities;
using TMS.Infrastructure.AppConfigurations;
using TMS.Infrastructure.Data.DbContextTools;
using TMS.Infrastructure.DataSeeder.Interfaces;

namespace TMS.Infrastructure.DataSeeder.Seeders;

public class UsersSeeder : IDataSeeder
{
    public EnvironmentEnum Environment => EnvironmentEnum.Development;
    public int Priority => 4;
    public async Task<DbRequest> SeedAsync(AppDbContext context)
    {
       try
       {
           if(await context.Users.AnyAsync()) return  DbRequest.Success("Nothing To add");

       
           
           var getAdminRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower()=="admin");
           var getTeamLeaderRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower()=="teamleader");
           
           var getEmployeeRole = await context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower()=="employee");
           
           if(getAdminRole is null || getTeamLeaderRole is null) return DbRequest.Failure("Admin Role was not founded");
           
           
           var usersList = new List<User>();
           var user = new User();
           user.UserName = "rami";
           var hashedPassword = new PasswordHasher<User>()
               .HashPassword(user,"123");
           user.PasswordHash = hashedPassword;
           user.Roles = [ getAdminRole , getTeamLeaderRole ];
           user.Employee = new Employee()
           {
                FirstName = "Rami",
                LastName = "Alzend",
                Email = "ramialzend@mail.com",
                Phone = "34234",
                BirthDate = DateTime.Now,
                HireDate = DateTime.Now,
                NationalIdentificationNumber = "123123"
                
           };
           usersList.Add(user);
           var user2 = new User();
           user2.UserName = "ibrahim";
           user2.PasswordHash = hashedPassword;
           user2.Roles = [ getEmployeeRole ];
           user2.Employee = new Employee()
           {
               FirstName = "Ibrahim",
               LastName = "Zaytoun",
               Email = "IbrahimZaytoun@mail.com",
               Phone = "342343",
               BirthDate = DateTime.Now,
               HireDate = DateTime.Now,
               NationalIdentificationNumber = "1231231"
                
           };
           usersList.Add(user2);
           
           var user3 = new User();
           user3.UserName = "rama";
           user3.PasswordHash = hashedPassword;
           user3.Roles = [getEmployeeRole ];
           user3.Employee = new Employee()
           {
               FirstName = "Rama",
               LastName = "Alkhateeb",
               Email = "Alkhateeb@mail.com",
               Phone = "3423433",
               BirthDate = DateTime.Now,
               HireDate = DateTime.Now,
               NationalIdentificationNumber = "12312311"
           };
           usersList.Add(user3);
           
           await context.Users.AddRangeAsync(usersList);
           
           var result = await context.SaveChangesAsync();
            
           return result > 0 ? DbRequest.Success() : DbRequest.Failure("Error according to roles");
       }
       catch (Exception e)
       {
           return DbRequest.Failure(e.Message);
       }
    }
}
