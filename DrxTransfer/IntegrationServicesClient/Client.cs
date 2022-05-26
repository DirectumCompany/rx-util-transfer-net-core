using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using NLog;
using Simple.OData.Client;

namespace DrxTransfer.IntegrationServicesClient
{
    class Client
    {
        private static Client instance;
        private static ODataClient client;

        private Client(string userName, string password, string servicesUrl)
        {
            var settings = new ODataClientSettings(new Uri(servicesUrl));
            var timeout = ConfigSettingsService.GetIntParamValue(Constants.ConfigServices.RequestTimeoutParamName, "600");

            settings.RequestTimeout = new TimeSpan(0, 0, timeout);
            settings.BeforeRequest += delegate (HttpRequestMessage message)
            {
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", userName, password)));
                message.Headers.Add("Authorization", "Basic " + authHeaderValue);
            };

            client = new ODataClient(settings);
        }

        /// <summary>
        /// Установка параметров подключения к сервису интеграции.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="password">Пароль.</param>
        public static void Setup(string userName, string password, Logger logger)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Не указано имя пользователя для подключения к сервису интеграции.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("не указан пароль для подключения к сервису интеграции.");

            // Получение адреса сервиса интеграции из конфига.
            if (instance == null)
                instance = new Client(userName, password, ConfigSettingsService.GetConfigSettingsValueByName(Constants.ConfigServices.IntegrationServiceUrlParamName));

            logger.Info("Подготовка клиента OData.");
        }

        /// <summary>
        /// Получение сущностей.
        /// </summary>
        /// <typeparam name="T">Тип сущности.</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEntities<T>() where T : class
        {
            var data = client.For<T>().FindEntriesAsync().Result;

            return data;
        }

        /// <summary>
        /// Получение сущностей по фильтру.
        /// </summary>
        /// <typeparam name="T">Тип сущности.</typeparam>
        /// <param name="expression">Условие фильтрации.</param>
        /// <returns>Сущность.</returns>
        public static IEnumerable<T> GetEntitiesByFilter<T>(ODataExpression expression) where T : class
        {
            var data = client.For<T>().Filter(expression).FindEntriesAsync().Result;

            return data;
        }

        /// <summary>
        /// Создать сущность.
        /// </summary>
        /// <typeparam name="T">Тип сущности.</typeparam>
        /// <param name="entity">Экзмемпляр сущности.</param>
        /// <returns>Созданна сущность.</returns>
        public static T CreateEntity<T>(T entity) where T : class
        {
            var data = client.For<T>().Set(entity).InsertEntryAsync().Result;

            return data;
        }

        /// <summary>
        /// Обновить сущность.
        /// </summary>
        /// <typeparam name="T">Тип сущности.</typeparam>
        /// <param name="entity">Экзмемпляр сущности.</param>
        /// <returns>Обновленная сущность.</returns>
        public static T UpdateEntity<T>(T entity) where T : class
        {
            var data = client.For<T>().Key(entity).Set(entity).UpdateEntryAsync().Result;

            return data;
        }

        /// <summary>
        /// Получение экземпляра клиента OData.
        /// </summary>
        /// <returns>Simple.OData.</returns>
        public static ODataClient Instance()
        {
            if (client == null)
                return null;

            return client;
        }
    }
}
