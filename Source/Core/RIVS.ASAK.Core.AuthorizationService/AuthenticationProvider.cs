using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RIVS.ASAK.Core.Contract;
using RIVS.ASAK.Core.Contract.Configuration;
using RIVS.ASAK.Core.Contract.DTO;
using RIVS.ASAK.Core.Tools.Serialization;
using RIVS.ASAK.Core.Tools.Strings;

namespace RIVS.ASAK.Core.AuthorizationService
{

    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _configuration;
        private readonly string _url;
        private readonly ILogger _logger;
        private readonly Guid _objectId;

        public AuthenticationProvider(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IAppConfigurationService appConfigurationService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = loggerFactory.Create(GetType());
            //читаем прямо из конфига
            try
            {
                var ip = _configuration.GetSection("webclient:ip").Value;
                var port = _configuration.GetSection("webclient:port").Value;
                _url = $"http://{ip}:{port}/api/auth/";
            }
            catch (Exception ex)
            {
                _logger.Error($"AuthenticationProvider ctor {ex.Message}");
            }
            
            _objectId = appConfigurationService.GetSystemId();
        }

        public async Task<UserAccess2DescriptorDTO> AuthorizationAsync(string login, string password, CancellationToken ct)
        {
            var data = new UserAccess2FilterDTO
            {
                Login = login,
                PasswordHash = CryptoProvider.ComputeHashHex(password),
                ActionId = Permissions.Authorization,
                ObjectId = _objectId,
            };

            //TODO: убрать заглушку
            //Тут заглушка, когда нету доступа к провайдеру авторизации
            return login switch
            {
                "dev" => new UserAccess2DescriptorDTO()
                {
                    Result = 0,
                    Message = "Доступ разрешён",
                    FullName = "* Dev",
                    UserId = new Guid("d13d446b-a433-458e-9415-6dac7d84a2f5"),
                    RoleId = new Guid("e69428c5-fe6d-460f-9d8f-2d9d76476d3c")
                },
                "admin" => password == "123456" ? new UserAccess2DescriptorDTO()
                {
                    Result = 0,
                    Message = "Доступ разрешён",
                    FullName = "* Admin",
                    UserId = new Guid("8ea84048-3c3d-4b13-a340-f846238906a6"),
                    RoleId = new Guid("69017545-e6c4-498c-853b-bb10063ad65e")
                } : new UserAccess2DescriptorDTO() { Result = 1, Message = "Неверный логин или пароль" },
                _ => new UserAccess2DescriptorDTO() { Result = 1, Message = "Неверный логин или пароль" }
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                        JsonSerializerHelper.JsonSettings);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{_url}access2", content, ct);
                    response.EnsureSuccessStatusCode();
                    var contentString = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<UserAccess2DescriptorDTO>(contentString);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error("AuthenticationService.AuthorizationAsync", ex);
                    return null;
                }
            }
        }

        public async Task<UserAccess2DescriptorDTO> AuthorizationAsync(string login, string password, Guid actionId, Guid objectId, CancellationToken ct)
        {
            var data = new UserAccess2FilterDTO
            {
                Login = login,
                PasswordHash = CryptoProvider.ComputeHashHex(password),
                ActionId = actionId,
                ObjectId = objectId,
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                        JsonSerializerHelper.JsonSettings);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{_url}access2", content, ct);
                    response.EnsureSuccessStatusCode();
                    var contentString = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<UserAccess2DescriptorDTO>(contentString);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error("AuthenticationService.AuthorizationAsync", ex);
                    return null;
                }
            }
        }

        public async Task<UserPermissionDTO> GetPermissionAsync(Guid userId, Guid actionId, Guid objectId, CancellationToken ct)
        {
            var data = new UserPermissionFilterDTO
            {
                UserId = userId,
                ActionId = actionId,
                ObjectId = objectId,
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                        JsonSerializerHelper.JsonSettings);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{_url}permission", content, ct);
                    response.EnsureSuccessStatusCode();
                    var contentString = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<UserPermissionDTO>(contentString);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error("AuthenticationService.GetPermissionAsync", ex);
                    return null;
                }
            }
        }

        public async Task<UserPermissionDTO> GetPermissionAsync(Guid userId, Guid actionId, CancellationToken ct)
        {
            var data = new UserPermissionFilterDTO
            {
                UserId = userId,
                ActionId = actionId,
                ObjectId = _objectId,
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented,
                        JsonSerializerHelper.JsonSettings);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{_url}permission", content, ct);
                    response.EnsureSuccessStatusCode();
                    var contentString = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<UserPermissionDTO>(contentString);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error("AuthenticationService.GetPermissionAsync", ex);
                    return null;
                }
            }
        }
    }
}
