using System;
using System.Threading.Tasks;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using Firebase.Auth;
using Xamarin.Essentials;

namespace ASWorkoutTracker.Droid.Auth
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

                var user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                var result = token.Token;

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
                return new LoginResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> ForgotPassword(string email)
        {
            try
            {
                await FirebaseAuth.Instance.SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
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

                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);                
                var result = token.Token;

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
            catch (FirebaseAuthInvalidUserException ex)
            {
                ex.PrintStackTrace();
                return new LoginResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (FirebaseAuthInvalidCredentialsException ex)
            {
                ex.PrintStackTrace();
                return new LoginResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public bool SignOut()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }
        }

        public bool IsLoggedIn()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            return user != null;
        }
    }
}
