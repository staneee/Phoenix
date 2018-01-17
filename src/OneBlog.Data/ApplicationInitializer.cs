using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace OneBlog.Data
{
    /// <summary>
    /// 数据初始化
    /// </summary>
    public class ApplicationInitializer
    {
        private ApplicationDbContext _ctx;
        private UserManager<ApplicationUser> _userMgr;
        private RoleManager<IdentityRole> _roleMgr;
        public ApplicationInitializer(ApplicationDbContext ctx, UserManager<ApplicationUser> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _ctx = ctx;
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task SeedAsync()
        {
            // Seed User
            if (await _userMgr.FindByNameAsync("admin@chenrensong.com") == null)
            {
                // 添加系统角色
                if (!await _roleMgr.RoleExistsAsync("Administrator"))
                {
                    await _roleMgr.CreateAsync(new IdentityRole("Administrator"));
                }
                if (!await _roleMgr.RoleExistsAsync("Anonymous"))
                {
                    await _roleMgr.CreateAsync(new IdentityRole("Anonymous"));
                }
                if (!await _roleMgr.RoleExistsAsync("Editor"))
                {
                    await _roleMgr.CreateAsync(new IdentityRole("Editor"));
                }
                // 创建账号

                var user_crs = new ApplicationUser()
                {
                    Email = "admin@chenrensong.com",
                    UserName = "admin@chenrensong.com",
                    DisplayName = "系统管理员",
                    Signature = "OneBlog 系统管理员",
                    EmailConfirmed = true
                };

                var userResult = await _userMgr.CreateAsync(user_crs, "admin@chenrensong");

                if (!userResult.Succeeded)
                {
                    throw new InvalidProgramException("Failed to create seed user");
                }

                // 为账号分配角色
                var roleResult = await _userMgr.AddToRoleAsync(user_crs, "Administrator");

                if (!roleResult.Succeeded)
                {
                    throw new InvalidProgramException("Failed to create seed role");
                }

            }
        }
    }
}
