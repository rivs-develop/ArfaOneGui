using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;

namespace RIVS.ASAK.Core.Tools
{
    /// <summary>
    /// Класс сетевых услуг.
    /// </summary>
    public static class NetTool
    {
        // кэш хостов
        private static readonly Dictionary<string, string> Cash =
            new Dictionary<string, string>();
        // время последнего обновления
        private static readonly Dictionary<string, DateTime> Updates =
            new Dictionary<string, DateTime>();

        /// <summary>
        /// Время обновления кэша.
        /// </summary>
        public static uint UpdateMinutes = 10;

        /// <summary>
        /// Метод сравнения хостов.
        /// </summary>
        /// <param name="host1">Первый хост.</param>
        /// <param name="host2">Второй хост.</param>
        /// <returns>Возвращает булевый результат сравнения.</returns>
        public static bool IsHostsEquals(string host1, string host2)
        {
            return GetHostName(host1) == GetHostName(host2);
        }

        /// <summary>
        /// Метод проверки заданного хоста текущему.
        /// </summary>
        /// <param name="host">Заданный хост.</param>
        /// <returns>Возвращает булевый результат сравнения.</returns>
        public static bool IsCurrentHost(string host)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(host, out ipAddress))
            {
                return IPAddress.Equals(ipAddress, GetCurrentHostIPAddress());
            }
            else
            {
                return IsHostsEquals(host, Environment.MachineName);
            }
        }

        /// <summary>
        /// Метод запроса имени хоста.
        /// </summary>
        /// <param name="host">Хост в любом формате.</param>
        /// <returns>Возвращает строковое имя хоста или null, если хост не найден.</returns>
        public static string GetHostName(string host)
        {
            if (host == null)
                host = string.Empty;
            DateTime queryTime = DateTime.Now;
            // если нет в кэше или превышено время обновления кэша
            if (!Cash.ContainsKey(host) || (queryTime - Updates[host]).TotalMinutes > UpdateMinutes)
            {
                try
                {
                    var hostInfo = Dns.GetHostEntry(host);
                    Cash[host] = hostInfo.HostName.ToLower();
                    Updates[host] = queryTime;
                }
                catch
                {
                    return null;
                }
            }
            return Cash[host];
        }

        /// <summary>
        /// Метод запроса имени текущего хоста.
        /// </summary>
        /// <returns>Возвращает строковое имя хоста.</returns>
        public static string GetCurrentHostName()
        {
            return GetHostName(Environment.MachineName);
        }

        /// <summary>
        /// Метод запроса адреса хоста.
        /// </summary>
        /// <param name="host">Хост в любом формате.</param>
        /// <returns>Возвращает строковый адрес хоста.</returns>
        public static string GetHostAddress(string host)
        {
            return GetHostIPAddress(host).ToString();
        }

        /// <summary>
        /// Метод запроса адреса текущего хоста.
        /// </summary>
        /// <returns>Возвращает строковое имя текущего хоста.</returns>
        public static string GetCurrentHostAddress()
        {
            return GetHostAddress(Environment.MachineName);
        }

        /// <summary>
        /// Метод запроса IP-адреса хоста.
        /// </summary>
        /// <param name="host">Хост в любом формате.</param>
        /// <returns>Возвращает IP-адрес хоста или null, если не смогли определить адрес.</returns>
        public static IPAddress GetHostIPAddress(string host)
        {
            IPAddress result;
            if (!IPAddress.TryParse(host, out result))
            {
                try
                {
                    result = Dns.GetHostEntry(host).AddressList.FirstOrDefault(adr => adr.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            return result;
        }
        private static readonly Dictionary<string,IPAddress>  _cache=new Dictionary<string, IPAddress>();
        public static IPAddress GetHostIPAddressWithCahe(string host)
        {
            var h = host.ToLower();
            if (!_cache.ContainsKey(h))
                _cache[h] = GetHostIPAddress(h);
            return _cache[h];
        }
        public static void ResetCahe()
        {
            _cache.Clear();
        }
        /// <summary>
        /// Метод запроса IP-адреса текущего хоста.
        /// </summary>
        /// <returns>Возвращает IP-адрес хоста.</returns>
        public static IPAddress GetCurrentHostIPAddress()
        {
            return GetHostIPAddress(Environment.MachineName);
        }

        public static bool IsIpAddress(string address)
        {
            IPAddress ipAddress;
            var isIpAddress = IPAddress.TryParse(address, out ipAddress);
            return isIpAddress;
        }

        public static bool IsHostName(string address)
        {
            return Uri.CheckHostName(address) == UriHostNameType.Dns;
        }

        public static bool AddressEquals(string address1, string address2)
        {
            var a1 = GetHostIPAddressWithCahe(address1);
            var a2 = GetHostIPAddressWithCahe(address2);
            if (a1 == null && a2 == null)
            {
                return String.Compare(address1, address2, StringComparison.OrdinalIgnoreCase) == 0;
            }
            if (a1 == null || a2 == null)
            {
                return false;
            }
            return a1.Equals(a2);
        }

        /// <summary>
        /// Получение уникального ID (Windows Installation GUID)
        /// </summary>
        /// <remarks>Если машина восстанавливается из резервной копии или клона
        /// (например, в случае аварийного восстановления, сценариев быстрого развертывания виртуальной машины)
        /// , тогда MachineGuid значение будет одинаковым на нескольких машинах.</remarks>
        public static Guid GetMachineGuid()
        {
            const string location = @"SOFTWARE\Microsoft\Cryptography";
            const string name = "MachineGuid";
            try
            {
                using (var localMachineX64View =
                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var rk = localMachineX64View.OpenSubKey(location))
                    {
                        if (rk == null)
                        {
                            throw new KeyNotFoundException(
                                string.Format("Key Not Found: {0}", location));
                        }

                        object machineGuid = rk.GetValue(name);
                        if (machineGuid == null)
                        {
                            throw new IndexOutOfRangeException(
                                string.Format("Index Not Found: {0}", name));
                        }

                        return Guid.Parse(machineGuid.ToString());
                    }
                }
            }
            catch (Exception)
            {
                // не оставляем хост без гуида - назначаем принудительно
                // но надо помнить, что для сервера (а он у нас один) - один гуид
                // а для клиентов - разный
                return new Guid("ee075ab0-8f5f-40e7-b912-ebf9eabeaf6a");
            }
            
        }
    }
}
