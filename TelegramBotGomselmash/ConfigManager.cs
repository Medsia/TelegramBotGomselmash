using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TelegramBotGomselmash
{
    public class ConfigManager
    {
        private dynamic? config;
        private FileSystemWatcher? watcher;
        private readonly string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public ConfigManager(string configFilePath)
        {
            if(!string.IsNullOrEmpty(configFilePath))
                this.configFilePath = configFilePath;
            LoadConfig(); // Загружаем конфигурацию при запуске
            StartConfigWatcher(); // Запускаем слежение за изменениями файла
        }

        // Метод для загрузки конфигурации
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string configContent = File.ReadAllText(configFilePath);
                    config = JsonSerializer.Deserialize<dynamic>(configContent);
                    Console.WriteLine("Конфигурация успешно загружена.");
                }
                else
                {
                    Console.WriteLine($"Файл конфигурации не найден: {configFilePath}. Создание нового файла с данными");
                    CreateDefaultConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке конфигурации: {ex.Message}. Создание нового файла с данными");
                CreateDefaultConfig();
            }
        }

        // Метод для загрузки значений по умолчанию
        private void CreateDefaultConfig()
        {
            config = new
            {
                BotConfiguration = new
                {
                    BotToken = "API_KEY"
                },
                adminId = 914603490,
                contacts = new
                {
                    BY = new
                    {
                        Spares = "Начальник отдела\r\nКардаш Алексей Анатольевич\r\nтел. +375 232 59-22-37\r\ne-mail: detal@gomselmash.by\r\n\r\nБрестская область – тел. +375 232 59-14-33\r\n\r\nВитебская область – тел. +375 232 59-23-03\r\n\r\nГомельская область – тел. +375 232 59-23-35\r\n\r\nГродненская область – тел. +375 232 59-23-65\r\n\r\nМинская область – тел. +375 232 59-20-41\r\n\r\nМогилёвская область – тел. +375 232 59-23-25",
                        Venicles = "Заместитель начальника управления продаж (Республика Беларусь)\r\nБогословская Ирина Валерьевна\r\nтел. +375 232 53-08-87; +375 232 59-23-52\r\nфакс. +375 232 59-26-79; +375 232 53-63-17\r\ne-mail: bogoslovskaya_irina@gomselmash.by\r\n\r\nБрестская область – тел. +375 232 59-26-44; +375 232 59-10-07\r\n\r\nВитебская область – тел. +375 232 59-23-00; +375 232 53-17-39\r\n\r\nГомельская область – тел. +375 232 59-23-91; +375 232 59-10-07\r\n\r\nГродненская область – тел. +375 232 59-23-10; +375 232 59-23-00\r\n\r\nМинская область – тел. +375 232 59-14-35; +375 232 59-10-07\r\n\r\nМогилевская область – тел. +375 232 59-22-05; +375 232 59-23-00",
                        Service = "Зам. начальника управления технического и сервисного обслуживания продукции (УТиСОП) – начальник отдела технического и сервисного обслуживания продукции в Республике Беларусь\r\nКрасовский Александр Александрович\r\nтел. +375 29 392-01-79, +375 232 53-21-87, +375 232 53-16-64\r\n\r\nБрестская и Гомельская области – тел. +375 44 725-90-13; +375 232 59-15-01; тел./факс +375 232 59-14-10; +375 232 59-13-07\r\n\r\nВитебская и Гродненская области – тел. +375 232 59-10-76; +375 232 59-10-23\r\n\r\nМинская и Могилёвская области – тел. +375 29 392-01-87; тел./факс +375 232 59-15-38; +375 232 59-11-64\r\n\r\nВ выходные и праздничные дни – тел./факс +375 232 59-14-10; +375 232 59-13-07; +375 232 59-15-38; +375 232 59-11-64"
                    },
                    RU = new
                    {
                        Spares = "Начальник отдела\r\nМороз Ольга Владимировна\r\nтел. +375 232 59-11-87\r\ne-mail: detal@gomselmash.by\r\n\r\nСпециалисты по продажам запасных частей\r\n\r\nтел./факс +375 232 59-13-10; +375 232 59-15-74; +375 232 59-27-44; +375 232 59-11-76\r\n\r\n",
                        Venicles = "Заместитель директора центра продаж и сервиса – начальник управления продаж (Европейская часть РФ)\r\nОвсянников Владимир Александрович\r\nтел./факс +375 232 53-08-90\r\ne-mail: evropa_rf@gomselmash.by\r\n\r\nСпециалисты по продажам техники\r\n\r\nтел. +375 232 59-15-58, +375 232 59-23-32, +375 232 59-13-01, +375 232 59-23-81, +375 232 53-08-44\r\nЗаместитель директора центра продаж и сервиса – начальник управления продаж (Азиатская часть РФ)\r\nЧеркас Сергей Антонович\r\nтел. +375 232 59-23-61\r\nфакс +375 232 53-20-62\r\ne-mail: azia_rf@gomselmash.by\r\n\r\nСпециалисты по продажам техники\r\n\r\nтел. +375 232 59-14-12; +375 232 59-15-41; +375 232 59-16-70",
                        Service = "тел./факс +375 232 59-10-33; +375 232 59-15-15; +375 232 59-10-29\r\n\r\nВ выходные и праздничные дни – тел./факс +375 232 59-14-10; +375 232 59-13-07; +375 232 59-15-38; +375 232 59-11-64"
                    },
                    KZ = new
                    {
                        Spares = "Начальник отдела\r\nМороз Ольга Владимировна\r\nтел. +375 232 59-11-87\r\n\r\nСпециалисты по продажам запасных частей\r\n\r\nтел./факс +375 232 59-13-10; +375 232 59-15-74; +375 232 59-27-44; +375 232 59-11-76",
                        Venicles = "Заместитель директора центра продаж и сервиса – начальник управления (страны ближнего зарубежья)\r\nИгнатов Михаил Петрович\r\nтел. +375 232 59-15-26\r\ne-mail: tspis_sbz@gomselmash.by\r\n\r\nСпециалисты по продажам техники\r\n\r\nтел. +375 232 59-14-60; +375 232 59-21-09; +375 232 59-11-58",
                        Service = "Сервисное обслуживание в странах ближнего зарубежья\r\n\r\nтел./факс +375 232 59-10-58; +375 232 59-11-89\r\n\r\nВ выходные и праздничные дни – тел./факс +375 232 59-14-10; +375 232 59-13-07; +375 232 59-15-38; +375 232 59-11-64"
                    }
                },
                photoLinks = new
                {
                    gs2124 = "https://disk.yandex.ru/d/eY5luKUHIQ7bNQ",
                    gh800_gh810 = "https://disk.yandex.ru/d/oV__71exTT5iXA",
                    gr700 = "https://disk.yandex.ru/d/la-bG0CnRh4yMg",
                    gs12a1 = "https://disk.yandex.ru/d/HdhJvt5mBhAvyg",
                    gs400 = "https://disk.yandex.ru/d/qEKmmRDhBGDcgg",
                    gs5 = "На данный момент материалы отсутствуют",
                    gs200 = "https://disk.yandex.ru/d/Q8fqpshk-yX-2w",
                    fs650 = "На данный момент материалы отсутствуют",
                    fs80_pro = "https://disk.yandex.ru/d/iENojmlzWxmFzA",
                    fs3000 = "На данный момент материалы отсутствуют",
                    cs200 = "На данный момент материалы отсутствуют",
                    cs150 = "На данный момент материалы отсутсвуют",
                    cs100 = "https://disk.yandex.ru/d/T4LyBcJjWWasow"
                },
                catalogLinks = new
                {
                    gs2124 = "https://disk.yandex.ru/i/hfCDzLUfXhhBOg",
                    gh800_gh810 = "https://disk.yandex.ru/i/o37ymF_Y7ZuXiQ",
                    gr700 = "https://disk.yandex.ru/d/jdtc_g2q_FCCHg",
                    gs12a1 = "https://disk.yandex.ru/i/xu1C4pXu39_m6w",
                    gs400 = "https://disk.yandex.ru/i/ySvqcracnq4AOA",
                    gs5 = "https://disk.yandex.ru/d/Sj8PF5F9fuTl0g",
                    gs200 = "https://disk.yandex.ru/i/DkwSTyMtVm_OlQ",
                    fs650 = "https://disk.yandex.ru/d/Ihnh5n3IRXy1dQ",
                    fs80_pro = "https://disk.yandex.ru/i/UeHBjo-YzyeZrw",
                    fs3000 = "https://disk.yandex.ru/d/7R5j8Kp8s08uAQ",
                    cs200 = "На данный момент материалы отсутствуют",
                    cs150 = "https://disk.yandex.ru/i/mm84mpDvOLsxOw",
                    cs100 = "https://disk.yandex.ru/i/M2rbrFRkmGbO5g"
                }
            };

            // Сериализация конфигурации в JSON и запись в файл
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);

            Console.WriteLine($"Создан файл конфигурации: {configFilePath}");
            LoadConfig();
        }

        // Метод для мониторинга файла конфигурации
        private void StartConfigWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = AppDomain.CurrentDomain.BaseDirectory;
            watcher.Filter = Path.GetFileName(configFilePath);
            watcher.Changed += (sender, args) => LoadConfig();
            watcher.EnableRaisingEvents = true;
        }

        // Метод для получения значений конфигурации
        public dynamic GetValue(params string[] keys)
        {
            JsonElement current = config; 
            foreach (var key in keys)
            {
                if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(key, out var next))
                {
                    throw new KeyNotFoundException($"Key '{key}' not found in config.");
                }
                current = next;
            }

            return current;
        }
        public string GetBotToken()
        {
            return config?.BotConfiguration?.BotToken ?? throw new Exception("Bot token not found in config.");
        }
    }
}
