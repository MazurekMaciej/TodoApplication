using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using TodoApplication.Models;

namespace TodoApplication.DAL
{
    public class CrmContext : IdentityDbContext<CrmContext.ApplicationUser> //DbContext
    {
        public class ApplicationUser : IdentityUser
        {
            public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CrmContext.ApplicationUser> manager)
            {

                var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
                return userIdentity;
            }
        }
        public CrmContext()
       : base("DefaultConnection", throwIfV1Schema: false)
        {

        }
            public static CrmContext Create()
            {
                return new CrmContext();
            }

            public DbSet<ToDoModels> ToDoModels { get; set; }
            public DbSet<PersonModels> PersonModels { get; set; }

           // base.OnModelCreating(modelBuilder);
    }

   
}
