using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using Foundation;
using Microsoft.AppCenter.Analytics;
using Xamarin.Essentials;

namespace ASWorkoutTracker.iOS.Auth
{
    public class FirebaseAuthentication : IFirebaseAuthentication
    {
        public async Task<LoginResult> AddUser(string username, string email, string password)
        {
            try
            {
                if (DeviceInfo.DeviceType == DeviceType.Virtual)
                    return new LoginResult
                    {
                        Success = true,
                        AuthID = "SIMUSER",
                        ProviderID = "Firebase",
                        Username = email.Substring(0, email.IndexOf('@')),
                        Email = email,
                        Token = Guid.NewGuid().ToString()
                    };

                var user = await Firebase.Auth.Auth.DefaultInstance.CreateUserAsync(email, password);
                var result = await user.User.GetIdTokenAsync();

                return new LoginResult
                {
                    Success = true,
                    AuthID = user.User.Uid,
                    ProviderID = user.User.ProviderId,
                    Username = user.AdditionalUserInfo.Username,
                    Email = user.User.Email,
                    Token = result
                };
            }
            catch (Exception ex)
            {
                var message = ((NSErrorException)ex).Error.LocalizedDescription;
                return new LoginResult
                {
                    Success = false,
                    Message = message
                };
            }
        }

        public async Task<bool> ForgotPassword(string email)
        {
            try
            {
                await Firebase.Auth.Auth.DefaultInstance.SendPasswordResetAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                var message = ((NSErrorException)ex).Error.LocalizedDescription;
                return false;
            }
        }

        public async Task<LoginResult> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                if (DeviceInfo.DeviceType == DeviceType.Virtual)
                    return new LoginResult
                    {
                        Success = true,
                        AuthID = "SIMUSER",
                        ProviderID = "Firebase",
                        Username = email.Substring(0, email.IndexOf('@')),
                        Email = email,
                        Token = Guid.NewGuid().ToString()
                    };

                var user = await Firebase.Auth.Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                var result = await user.User.GetIdTokenAsync();

                return new LoginResult
                {
                    Success = true,
                    AuthID = user.User.Uid,
                    ProviderID = user.User.ProviderId,
                    Username = user.AdditionalUserInfo.Username,
                    Email = user.User.Email,
                    Token = result
                };
            }
            catch (ArgumentNullException ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                var message = ((NSErrorException)ex).Error.LocalizedDescription;
                return new LoginResult
                {
                    Success = false,
                    Message = message
                };
            }
        }

        public bool SignOut()
        {
            try
            {
                _ = Firebase.Auth.Auth.DefaultInstance.SignOut(out NSError error);
                return error == null;
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Logout Issue", new Dictionary<string, string> { { "Issue", ex.Message } });
                return false;
            }
        }

        public bool IsLoggedIn()
        {
            var user = Firebase.Auth.Auth.DefaultInstance.CurrentUser;
#if DEBUG
            Analytics.TrackEvent("IsLoggedIn Report", new Dictionary<string, string> {
                { "IsLoggedIn", (user != null).ToString() },
                { "UserDetails", user != null ? user.ToString() : string.Empty }
            });
#endif
            return user != null;
        }
    }
}
