using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace spsServerAPI.Models
{

    public class AuthRepository : IDisposable
    {
        
        private ApplicationUserManager _appUserManager;
        ApplicationDbContext autDb = new ApplicationDbContext();


        public AuthRepository()
        {
            //_ctx = new AuthContext();
            //_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            _appUserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public async Task<IdentityResult> RegisterUser(RegisterBindingModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.Email,
                Email = userModel.Email,
                Approved = false
            };

            var result = await _appUserManager.CreateAsync(user,userModel.Password);

            return result;
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _appUserManager.FindAsync(userName, password);
            return user;
        }

        public Client FindClient(string clientId)
        {
            
            //var client = _ctx.Clients.Find(clientId);
            var client = autDb.Clients.Find(clientId);
            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            //var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();
            var existingToken = autDb.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();
            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            autDb.RefreshTokens.Add(token);

            return await autDb.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            //var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);
            var refreshToken = await autDb.RefreshTokens.FindAsync(refreshTokenId);
            if (refreshToken != null)
            {
                autDb.RefreshTokens.Remove(refreshToken);
                return await autDb.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            autDb.RefreshTokens.Remove(refreshToken);
            return await autDb.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await autDb.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return autDb.RefreshTokens.ToList();
        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {

            //IdentityUser user = await _userManager.FindAsync(loginInfo);
            ApplicationUser user = await _appUserManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            var result = await _appUserManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _appUserManager.AddLoginAsync(userId, login);

            return result;
        }

        public void Dispose()
        {
            autDb.Dispose();
            //_appUserManager.Dispose();

        }
    }
}