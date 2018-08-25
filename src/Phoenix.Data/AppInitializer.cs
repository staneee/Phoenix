using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Phoenix.Data
{
    /// <summary>
    /// 数据初始化
    /// </summary>
    public class AppInitializer
    {
        private AppDbContext _ctx;
        private UserManager<AppUser> _userMgr;
        private RoleManager<IdentityRole> _roleMgr;
        public AppInitializer(AppDbContext ctx, UserManager<AppUser> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _ctx = ctx;
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task SeedAsync()
        {
            _ctx.Database.Migrate();
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

                var user_crs = new AppUser()
                {
                    Email = "admin@chenrensong.com",
                    UserName = "admin@chenrensong.com",
                    DisplayName = "系统管理员",
                    Signature = "Phoenix 系统管理员",
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
