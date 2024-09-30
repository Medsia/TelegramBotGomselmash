using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotGomselmash
{

    class Program
    {
        private static ConfigManager configManager;

        private static TelegramBotClient botClient;

        // Словарь для хранения состояний пользователей
        static Dictionary<long, UserState> userStates = new Dictionary<long, UserState>();

        static long adminId = 914603490;

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null && update.Message.Text != null)
            {
                var message = update.Message;
                var chatId = message.Chat.Id;

                if (!userStates.ContainsKey(chatId))
                {
                    userStates[chatId] = new UserState
                    {
                        UserHistory = UserHistory.Menu
                    };
                    Subscribers.AddSubscriber(chatId);
                    Subscribers.SaveSubscribers();
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать!");
                }
                if (userStates[chatId].UserHistory == UserHistory.SendingBroadcast)
                {
                    if (message.Text.ToLower() == "назад")
                    {
                        userStates[chatId].UserHistory = UserHistory.Menu;
                        await botClient.SendTextMessageAsync(chatId, "Рассылка отменена.");
                    }
                    else
                    {
                        string broadcastMessage = message.Text;

                        await SendBroadcastMessageAsync(botClient, broadcastMessage);

                        userStates[chatId].UserHistory = UserHistory.Menu;
                        await botClient.SendTextMessageAsync(chatId, "Рассылка завершена.");
                    }
                }
                if (userStates[chatId].UserHistory == UserHistory.ChangingAdmin)
                {
                    if (message.Text.ToLower() == "назад")
                    {
                        userStates[chatId].UserHistory = UserHistory.Menu;
                        await botClient.SendTextMessageAsync(chatId, "Смена отменена.");
                    }
                    else
                    {
                        if (long.TryParse(message.Text, out adminId))
                            await botClient.SendTextMessageAsync(chatId, "Смена завершена.");
                        else
                            await botClient.SendTextMessageAsync(chatId, "Некорректные данные.");

                        userStates[chatId].UserHistory = UserHistory.Menu;
                    }
                }
                var userState = userStates[chatId];
                switch (message.Text.ToLower())
                {
                    case ("/start"):
                        var replyKeyboard = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Фотоматериалы 📸"),
                                            new KeyboardButton("Каталоги 📄"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Покупка 💸"),
                                            new KeyboardButton("Сервис ⚙️"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Что вас интересует?",
                            replyMarkup: replyKeyboard);
                        return;
                    case ("/admin"):
                        if (IsAdmin(chatId))
                        {
                            var replyKeyboardAdmin = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Массовая рассылка"),
                                            new KeyboardButton("Сменить администратора"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                            {
                                ResizeKeyboard = true,
                            };

                            await botClient.SendTextMessageAsync(
                                message.Chat,
                                "Что вас интересует?",
                                replyMarkup: replyKeyboardAdmin);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                message.Chat,
                                "У вас нет прав администратора.");
                        }
                        return;
                    case ("массовая рассылка"):
                        if (IsAdmin(chatId))
                        {
                            userState.UserHistory = UserHistory.SendingBroadcast;
                            var replyKeyboardAdmin = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Массовая рассылка"),
                                            new KeyboardButton("Сменить администратора"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                            {
                                ResizeKeyboard = true,
                            };
                            await botClient.SendTextMessageAsync(
                                message.Chat,
                                "Введите текст рассылки",
                                replyMarkup: replyKeyboardAdmin);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                message.Chat,
                                "У вас нет прав администратора.");
                        }
                        return;
                    case ("сменить администратора"):
                        if (IsAdmin(chatId))
                        {
                            userState.UserHistory = UserHistory.ChangingAdmin;
                            var replyKeyboardAdmin = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Массовая рассылка"),
                                            new KeyboardButton("Сменить администратора"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                            {
                                ResizeKeyboard = true,
                            };

                            await botClient.SendTextMessageAsync(
                                message.Chat,
                                "Введите новый ID администратора(убедитесь в корректности отправляемых данных! id имеет следующий вид: 914603490)",
                                replyMarkup: replyKeyboardAdmin);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                message.Chat,
                                "У вас нет прав администратора.");
                        }
                        return;
                    case ("фотоматериалы 📸"):
                        userState.UserHistory = UserHistory.Menu;
                        var replyKeyboardPhotoModels = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Зерноуборочные комбайны 📸"),
                                            new KeyboardButton("Кормоуборочные комбайны 📸"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Самоходные косилки 📸"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите категорию продукции",
                            replyMarkup: replyKeyboardPhotoModels);
                        return;
                    case ("каталоги 📄"):
                        userState.UserHistory = UserHistory.Menu;
                        var replyKeyboardCatalogsModels = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Зерноуборочные комбайны 📄"),
                                            new KeyboardButton("Кормоуборочные комбайны 📄"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Самоходные косилки 📄"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите категорию продукции",
                            replyMarkup: replyKeyboardCatalogsModels);
                        return;
                    case ("покупка 💸"):
                        userState.UserHistory = UserHistory.Menu;
                        var replyKeyboardPurchase = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Техника 🚜"),
                                            new KeyboardButton("Запчасти ⚙️"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Что вы хотите приобрести?",
                            replyMarkup: replyKeyboardPurchase);
                        return;
                    case ("сервис ⚙️"):
                        userState.UserHistory = UserHistory.Service;
                        var replyKeyboardService = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Беларусь 🇧🇾"),
                                            new KeyboardButton("Россия 🇷🇺"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Казахстан 🇰🇿"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите регион",
                            replyMarkup: replyKeyboardService);
                        return;
                    case ("техника 🚜"):
                        userState.UserHistory = UserHistory.PurchaseVenicles;
                        var replyKeyboardVehiclesPurchase = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Беларусь 🇧🇾"),
                                            new KeyboardButton("Россия 🇷🇺"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Казахстан 🇰🇿"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите регион",
                            replyMarkup: replyKeyboardVehiclesPurchase);
                        return;
                    case ("запчасти ⚙️"):
                        userState.UserHistory = UserHistory.PurchaseSpares;
                        var replyKeyboardSparesPurchase = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Беларусь 🇧🇾"),
                                            new KeyboardButton("Россия 🇷🇺"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Казахстан 🇰🇿"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите регион",
                            replyMarkup: replyKeyboardSparesPurchase);
                        return;
                    case ("казахстан 🇰🇿"):
                        var replyKeyboardKazakhstan = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Беларусь 🇧🇾"),
                                            new KeyboardButton("Россия 🇷🇺"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Казахстан 🇰🇿"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };
                        if (userState.UserHistory == UserHistory.PurchaseSpares)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "KZ", "Spares")}",
                            replyMarkup: replyKeyboardKazakhstan);
                        }
                        else if (userState.UserHistory == UserHistory.PurchaseVenicles)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "KZ", "Venicles")}",
                            replyMarkup: replyKeyboardKazakhstan);
                        }
                        else if (userState.UserHistory == UserHistory.Service)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "KZ", "Service")}",
                            replyMarkup: replyKeyboardKazakhstan);
                        }

                        return;
                    case ("россия 🇷🇺"):
                        var replyKeyboardRussia = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Беларусь 🇧🇾"),
                                            new KeyboardButton("Россия 🇷🇺"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Казахстан 🇰🇿"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        if (userState.UserHistory == UserHistory.PurchaseSpares)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "RU", "Spares")}",
                            replyMarkup: replyKeyboardRussia);
                        }
                        else if (userState.UserHistory == UserHistory.PurchaseVenicles)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "RU", "Venicles")}",
                            replyMarkup: replyKeyboardRussia);
                        }
                        else if (userState.UserHistory == UserHistory.Service)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "RU", "Service")}",
                            replyMarkup: replyKeyboardRussia);
                        }
                        return;
                    case ("беларусь 🇧🇾"):
                        var replyKeyboardBelarus = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Беларусь 🇧🇾"),
                                            new KeyboardButton("Россия 🇷🇺"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Казахстан 🇰🇿"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        if (userState.UserHistory == UserHistory.PurchaseSpares)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "BY", "Spares")}",
                            replyMarkup: replyKeyboardBelarus);
                        }
                        else if (userState.UserHistory == UserHistory.PurchaseVenicles)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "BY", "Venicles")}",
                            replyMarkup: replyKeyboardBelarus);
                        }
                        else if (userState.UserHistory == UserHistory.Service)
                        {
                            await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"{configManager.GetValue("contacts", "BY", "Service")}",
                            replyMarkup: replyKeyboardBelarus);
                        }
                        return;
                    case ("зерноуборочные комбайны 📸"):
                        userState.UserHistory = UserHistory.ProductCategoryPhoto;
                        var replyKeyboardGrainHarvestersPhoto = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GS2124 📸"),
                                            new KeyboardButton("GH800/GH810 📸"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GR700 📸"),
                                            new KeyboardButton("GS12A1 📸"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GS400 📸"),
                                            new KeyboardButton("GS5 📸"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GS200 📸"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите модель",
                            replyMarkup: replyKeyboardGrainHarvestersPhoto);
                        return;
                    case ("кормоуборочные комбайны 📸"):
                        userState.UserHistory = UserHistory.ProductCategoryPhoto;
                        var replyKeyboardForageHarvestersPhoto = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("FS650 📸"),
                                            new KeyboardButton("FS80 PRO 📸"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("FS3000 (К-Г-6) 📸"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите модель",
                            replyMarkup: replyKeyboardForageHarvestersPhoto);
                        return;
                    case ("самоходные косилки 📸"):
                        userState.UserHistory = UserHistory.ProductCategoryPhoto;
                        var replyKeyboardSelfPropelledWindrowersPhoto = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("CS200 📸"),
                                            new KeyboardButton("CS150 CROSS 📸"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("CS100 📸"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите модель",
                            replyMarkup: replyKeyboardSelfPropelledWindrowersPhoto);
                        return;
                    case ("зерноуборочные комбайны 📄"):
                        userState.UserHistory = UserHistory.ProductCategoryCatalog;
                        var replyKeyboardGrainHarvestersCatalog = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GS2124 📄"),
                                            new KeyboardButton("GH800/GH810 📄"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GR700 📄"),
                                            new KeyboardButton("GS12A1 📄"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GS400 📄"),
                                            new KeyboardButton("GS5 📄"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("GS200 📄"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите модель",
                            replyMarkup: replyKeyboardGrainHarvestersCatalog);
                        return;
                    case ("кормоуборочные комбайны 📄"):
                        userState.UserHistory = UserHistory.ProductCategoryCatalog;
                        var replyKeyboardForageHarvestersCatalog = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("FS650 📄"),
                                            new KeyboardButton("FS80 PRO 📄"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("FS3000 (К-Г-6) 📄"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };

                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите модель",
                            replyMarkup: replyKeyboardForageHarvestersCatalog);
                        return;
                    case ("самоходные косилки 📄"):
                        userState.UserHistory = UserHistory.ProductCategoryCatalog;
                        var replyKeyboardSelfPropelledWindrowersCatalog = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("CS200 📄"),
                                            new KeyboardButton("CS150 CROSS 📄"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("CS100 📄"),
                                            new KeyboardButton("Назад"),
                                        },
                                        })
                        {
                            ResizeKeyboard = true,
                        };
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Выберите модель",
                            replyMarkup: replyKeyboardSelfPropelledWindrowersCatalog);
                        return;
                    case ("gs2124 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "gs2124")}");
                        return;
                    case ("gh800/gh810 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "gh800_gh810")}");
                        return;
                    case ("gr700 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "gr700")}");
                        return;
                    case ("gs12a1 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "gs12a1")} ");
                        return;
                    case ("gs400 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "gs400")}");
                        return;
                    case ("gs5 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "gs5")}");
                        return;
                    case ("gs200 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "gs200")}");
                        return;
                    case ("fs650 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "fs650")}");
                        return;
                    case ("fs80 pro 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "fs80_pro")}");
                        return;
                    case ("fs3000 (к-г-6) 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "fs3000")}");
                        return;
                    case ("cs200 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "cs200")}");
                        return;
                    case ("cs150 cross 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "cs150")}");
                        return;
                    case ("cs100 📸"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("photoLinks", "cs100")}");
                        return;
                    case ("gs2124 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "gs2124")}");
                        return;
                    case ("gh800/gh810 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "gh800_gh810")}");
                        return;
                    case ("gr700 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "gr700")}");
                        return;
                    case ("gs12a1 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "gs12a1")}");
                        return;
                    case ("gs400 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "gs400")}");
                        return;
                    case ("gs5 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "gs5")}");
                        return;
                    case ("gs200 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "gs200")}");
                        return;
                    case ("fs650 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "fs650")}");
                        return;
                    case ("fs80 pro 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "fs80_pro")}");
                        return;
                    case ("fs3000 (к-г-6) 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "fs3000")}");
                        return;
                    case ("cs200 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "cs200")}");
                        return;
                    case ("cs150 cross 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "cs150")}");
                        return;
                    case ("cs100 📄"):
                        await botClient.SendTextMessageAsync(message.Chat, $"Ссылка для скачивания: {configManager.GetValue("catalogLinks", "cs100")}");
                        return;
                    case ("назад"):
                        if (userState.UserHistory == UserHistory.Menu)
                            goto case ("/start");
                        else if (userState.UserHistory == UserHistory.ProductCategoryPhoto)
                            goto case ("фотоматериалы 📸");
                        else if (userState.UserHistory == UserHistory.ProductCategoryCatalog)
                            goto case ("каталоги 📄");
                        else if (userState.UserHistory == UserHistory.Purchase || userState.UserHistory == UserHistory.PurchaseSpares || userState.UserHistory == UserHistory.PurchaseVenicles)
                            goto case ("покупка 💸");
                        else
                        {
                            userState.UserHistory = UserHistory.Menu;
                            goto case ("/start");
                        }
                    default:
                        await botClient.SendTextMessageAsync(message.Chat, "Неизвестная команда.");
                        return;
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            await Task.CompletedTask;
        }
        public static async Task SendBroadcastMessageAsync(ITelegramBotClient botClient, string message)
        {
            foreach (var chatId in Subscribers.subscribers)
            {
                try
                {
                    await botClient.SendTextMessageAsync(chatId, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message to {chatId}: {ex.Message}");
                }
            }
        }
        private static bool IsAdmin(long chatId)
        {
            return chatId == adminId ? true : false;
        }
        static void Main(string[] args)
        {
            Subscribers.LoadSubscribers();
            configManager = new ConfigManager("config.json");
            botClient = Bot.GetTelegramBot(configManager.GetValue("BotConfiguration", "BotToken").GetString());
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types               
                ThrowPendingUpdates = true, // parameter responsible for processing messages received while the bot was offline
            };
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.WriteLine("Телеграм-бот запущен.");
            Console.ReadLine();
        }
    }
}
