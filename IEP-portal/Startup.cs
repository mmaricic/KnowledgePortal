using IEP_portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Linq;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
[assembly: OwinStartupAttribute(typeof(IEP_portal.Startup))]
namespace IEP_portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            createRolesAndUsers();

        }

        // In this method we will create default User roles and Admin user for login   
        private void createRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com";
                user.Name = "Admin";
                user.Surname = "Admin";
                user.Type =  "A";
                user.isActive = true;

                string userPWD = "Marija.123";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating User role    
            if (!roleManager.RoleExists("Teacher"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Teacher";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Student"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);
            }


            DatabaseModel dbModel = new DatabaseModel();

            if (!dbModel.Parameters.Any()) {
                var param = new Parameters();
                param.K = 3;
                param.S = 5;
                param.G = 10;
                param.P = 15;
                param.E = 3;
                param.M = 3;
                dbModel.Parameters.Add(param);

                dbModel.SaveChanges();
            }
           
        }
    }
}