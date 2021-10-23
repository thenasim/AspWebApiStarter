using Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole()
                {
                    Name = Roles.Admin.ToString(),
                    NormalizedName = Roles.Admin.ToString().ToUpper()
                },
                new IdentityRole()
                {
                    Name = Roles.User.ToString(),
                    NormalizedName = Roles.User.ToString().ToUpper()
                }
            );
        }
    }
}