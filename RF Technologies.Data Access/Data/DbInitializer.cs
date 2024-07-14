using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RF_Technologies.Model;
using RF_Technologies.Utility;
using RF_Technologies.Data_Access.Repository.IRepository;


namespace RF_Technologies.Data_Access.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Student)).Wait();

                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "Admin@gmail.com",
                        Email = "Admin@gmail.com",
                        Name = "Admin",
                        NormalizedUserName = "ADMIN@GMAIL.COM",
                        NormalizedEmail = "ADMIN@GMAIL.COM",
                        PhoneNumber = "1234567890",
                    }, "Admin@123").GetAwaiter().GetResult();

                    ApplicationUser user = _db.User.FirstOrDefault(u => u.Email == "Admin@gmail.com");
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
