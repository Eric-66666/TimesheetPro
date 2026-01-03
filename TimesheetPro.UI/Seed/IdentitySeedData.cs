using Microsoft.AspNetCore.Identity;
using TimesheetPro.Core.Domain.IdentityEntities;
using TimesheetPro.Core.Enums;

namespace TimesheetPro.UI.Seed
{
    //role + user Initialization
    public static class IdentitySeedData
    {
        public static async Task SeedAsync(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = service.GetRequiredService<RoleManager<ApplicationRole>>();

            //create roles
            foreach (var roleName in Enum.GetNames(typeof(AppRoles)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName});
                }
            }

            //mock users
            
            string pmEmail = "pm@timesheetpro.com";
            string pmPassword = "Pm123456";

            string consultantEmail = "consultant@timesheetpro.com";
            string consultantPassword = "Consultant123";

            string adminEmail = "admin@timesheetpro.com";
            string adminPassword = "Admin123";

            //project manager
            var pmUser = await userManager.FindByEmailAsync(pmEmail);
            if (pmUser == null)
            {
                pmUser = new ApplicationUser
                {
                    UserName = pmEmail,
                    Email = pmEmail,
                    FullName = "Project Manager",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(pmUser, pmPassword);
                if (result.Succeeded)
                {
                    var pmRoles = new[] { AppRoles.ProjectManager, AppRoles.Finance };
                    await userManager.AddToRolesAsync(pmUser, pmRoles.Select(x => x.ToString()));
                }
            }


            //consultant
            var consultantUser = await userManager.FindByEmailAsync(consultantEmail);
            if (consultantUser == null)
            {
                consultantUser = new ApplicationUser
                {
                    UserName = consultantEmail,
                    Email = consultantEmail,
                    FullName = "Consultant",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(consultantUser, consultantPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(consultantUser, AppRoles.Consultant.ToString());
                }
            }

            //admin
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AppRoles.Admin.ToString());
                }
            }
        }
    }
}
