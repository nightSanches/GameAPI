-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1:3316
-- Время создания: Май 17 2026 г., 16:30
-- Версия сервера: 8.0.30
-- Версия PHP: 7.2.34

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `GameDatabase`
--

-- --------------------------------------------------------

--
-- Структура таблицы `Achievements`
--

CREATE TABLE `Achievements` (
  `Id` int NOT NULL,
  `District_Id` int NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Description` text NOT NULL,
  `Condition_Type` varchar(100) NOT NULL,
  `Condition_Value` int NOT NULL DEFAULT '0',
  `Reward_Rep` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Achievements`
--

INSERT INTO `Achievements` (`Id`, `District_Id`, `Name`, `Description`, `Condition_Type`, `Condition_Value`, `Reward_Rep`) VALUES
(1, 1, 'Первый шаг', 'Достигнуть 10 этажа в Промзоне', 'max_floor', 10, 10),
(2, 1, 'Идеальная десятка', 'Построить 10 идеальных этажей подряд', 'perfect_streak', 10, 25),
(3, 2, 'Бизнес-старт', 'Достичь 50 этажа в Деловом районе', 'max_floor', 50, 50),
(4, 3, 'Градостроитель', 'Провести 20 забегов в Центре', 'games_played', 20, 75);

-- --------------------------------------------------------

--
-- Структура таблицы `Bonuses`
--

CREATE TABLE `Bonuses` (
  `Id` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Price_money` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Bonuses`
--

INSERT INTO `Bonuses` (`Id`, `Name`, `Description`, `Price_money`) VALUES
(1, 'Стабилизатор', 'Временно уменьшает шатание крана', 50),
(2, 'Регулировка', 'Временно выравнивает небоскрёб по центру', 50),
(3, 'Страховка', 'Временно даёт неуязвимость от ошибок', 50);

-- --------------------------------------------------------

--
-- Структура таблицы `Districts`
--

CREATE TABLE `Districts` (
  `Id` int NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Unlock_Rep_Req` int NOT NULL DEFAULT '0',
  `Difficulty_Multiplier` decimal(3,2) NOT NULL DEFAULT '1.00',
  `Sort_Order` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Districts`
--

INSERT INTO `Districts` (`Id`, `Name`, `Unlock_Rep_Req`, `Difficulty_Multiplier`, `Sort_Order`) VALUES
(1, 'Промзона', 0, '1.00', 1),
(2, 'Деловой район', 500, '1.30', 2),
(3, 'Центр города', 1500, '1.60', 3),
(4, 'Научный район', 3000, '2.00', 4),
(5, 'Космический лифт', 6000, '2.50', 5);

-- --------------------------------------------------------

--
-- Структура таблицы `Upgrades`
--

CREATE TABLE `Upgrades` (
  `Id` int NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Description` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Upgrades`
--

INSERT INTO `Upgrades` (`Id`, `Name`, `Description`) VALUES
(1, 'Стабильность крана', 'Уменьшает шатания крана на большой высоте'),
(2, 'Фундамент', 'Уменьшение шатания небоскреба на большой высоте'),
(3, 'Доп. золото', 'Увеличить получаемое золото в конце игры'),
(5, 'Доп. множитель', 'Увеличивает максимальный множитель на x0.1'),
(6, 'Вместимость бонусов', 'Увеличивает вместимость бонусов');

-- --------------------------------------------------------

--
-- Структура таблицы `Upgrades_cost`
--

CREATE TABLE `Upgrades_cost` (
  `Id` int NOT NULL,
  `Upgrade_id` int NOT NULL,
  `Level` int NOT NULL DEFAULT '1',
  `Price_money` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Upgrades_cost`
--

INSERT INTO `Upgrades_cost` (`Id`, `Upgrade_id`, `Level`, `Price_money`) VALUES
(1, 1, 1, 10),
(2, 1, 2, 20),
(3, 1, 3, 30),
(4, 2, 1, 10),
(5, 2, 2, 20),
(6, 2, 3, 30),
(7, 3, 1, 10),
(8, 3, 2, 20),
(9, 3, 3, 30),
(13, 5, 1, 10),
(14, 5, 2, 20),
(15, 5, 3, 30),
(16, 6, 1, 10),
(17, 6, 2, 20),
(18, 6, 3, 30);

-- --------------------------------------------------------

--
-- Структура таблицы `Users`
--

CREATE TABLE `Users` (
  `Id` int NOT NULL,
  `Nickname` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'player',
  `Token` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Registration_date` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Email_confirmed` tinyint(1) NOT NULL DEFAULT '0',
  `Email_confirmation_token` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email_confirmation_token_expires` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users`
--

INSERT INTO `Users` (`Id`, `Nickname`, `Password`, `Role`, `Token`, `Email`, `Registration_date`, `Email_confirmed`, `Email_confirmation_token`, `Email_confirmation_token_expires`) VALUES
(1, 'Player_01', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(2, 'Player_02', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(3, 'Player_03', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(4, 'Player_04', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(5, 'Player_05', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(6, 'Player_06', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(7, 'Player_07', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(8, 'Player_08', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(9, 'Player_09', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(10, 'Player_10', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(11, 'Player_11', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(12, 'Player_12', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(13, 'Player_13', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(14, 'Player_14', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(15, 'Player_15', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(16, 'Player_16', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(17, 'Player_17', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(18, 'Player_18', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(19, 'Player_19', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(20, 'Player_20', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(21, 'Player_21', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(22, 'Player_22', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(23, 'Player_23', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(24, 'Player_24', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(25, 'Player_25', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(26, 'Player_26', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(27, 'Player_27', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(28, 'Player_28', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(29, 'Player_29', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(30, 'Player_30', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(31, 'Player_31', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(32, 'Player_32', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(33, 'Player_33', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(34, 'Player_34', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(35, 'Player_35', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(37, 'Player_37', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(38, 'Player_38', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(39, 'Player_39', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(40, 'Player_40', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(41, 'Player_41', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(42, 'Player_42', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(43, 'Player_43', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(44, 'Player_44', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(45, 'Player_45', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(46, 'Player_46', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(47, 'Player_47', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(48, 'Player_48', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(49, 'Player_49', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(50, 'Player_50', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(51, 'Player_51', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(52, 'Player_52', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(53, 'Player_53', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(54, 'Player_54', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(55, 'Player_55', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(56, 'Player_56', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL),
(62, 'testtest', '$2a$11$AIVRRE7zdVlOZzuDHLwVM.axCddubJFLUCVyyaa5cel.nGvP.iZZi', 'player', 'MjKs1DvLjXAX5aaovZbSET5mQZ8fYlrCQBzxeEH49jNGxVBRIDRmGvvyCnTqjEGlTfC8kDBHmXfylgEsAT9veaHrKp9ZpM1tph4a', NULL, '2026-05-05 13:48:26', 0, NULL, NULL),
(63, 'bebra', '$2a$11$RPeeZIxR5qPjfcWguKOeq.f9Ulm0G3/9omqtwE6Zl1f.Vm3Ej94VS', 'player', 'gGyVIIaAVTLOzcXhHG2dPMAp4bfrPzS1DxUM78g2O06hO2StDsNJVF7A2SrHGkP2VQoo83x5StkSZwh0yUX8T9BRZb3HqDpuyDOM', NULL, '2026-05-05 23:21:48', 0, 'c8e01f440325429b8d5e529a9106ce6eMjsnLPApEUmWpoCQzhPrg', '2026-05-07 02:21:48'),
(64, 'sanich', '$2a$11$s04Lgx0S9HUX1ES/YmZbeOo03uSVYbbWZCrswQrZO4plzH.0yFphq', 'player', 'ZRyLS6NI6CjcPPtFNCuqc7glMnaee55BeHimMp5UXInj8MCzjYRlD3CUWqfPxl83RrdhHNByPUfWbbE4xpd35EEftWlCmZdtvj46', NULL, '2026-05-08 15:40:30', 0, NULL, NULL),
(75, 'admin', '$2a$11$EesOnvGqS6rasWdU2QGu5ehEFkk03n2lV3HsMlMlkievh6pPuI9m.', 'player', 'WPWr6cJGBSgXdnI8gctv1AlrO6QmYrqeRYQzNR3MShGmgpBrhLxTPh3GLFWlKtiCMrRqJ4cRDBGFELfo8dI7pfiRJan3Vs6VlsyW', 'u246012@gmail.com', '2026-05-17 10:10:57', 0, '8854bc6403a6493ba91913ff0f29e48951luooZ1y0iMSIRmuqC63A', '2026-05-18 13:10:57');

-- --------------------------------------------------------

--
-- Структура таблицы `Users_achievements`
--

CREATE TABLE `Users_achievements` (
  `Id` int NOT NULL,
  `User_Id` int NOT NULL,
  `Achievement_Id` int NOT NULL,
  `Current_Progress` int NOT NULL DEFAULT '0',
  `Is_Unlocked` tinyint NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `Users_bonuses`
--

CREATE TABLE `Users_bonuses` (
  `Id` int NOT NULL,
  `User_id` int NOT NULL,
  `Bonus_id` int NOT NULL,
  `Quantity` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users_bonuses`
--

INSERT INTO `Users_bonuses` (`Id`, `User_id`, `Bonus_id`, `Quantity`) VALUES
(68, 62, 1, 0),
(69, 62, 2, 0),
(70, 62, 3, 0),
(71, 63, 1, 0),
(72, 63, 2, 0),
(73, 63, 3, 0),
(74, 64, 1, 0),
(75, 64, 2, 0),
(76, 64, 3, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `Users_gifts`
--

CREATE TABLE `Users_gifts` (
  `Id` int NOT NULL,
  `User_id` int NOT NULL,
  `Last_bonus_dt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users_gifts`
--

INSERT INTO `Users_gifts` (`Id`, `User_id`, `Last_bonus_dt`) VALUES
(28, 62, NULL),
(29, 63, NULL),
(30, 64, NULL);

-- --------------------------------------------------------

--
-- Структура таблицы `Users_scores`
--

CREATE TABLE `Users_scores` (
  `Id` int NOT NULL,
  `User_id` int NOT NULL,
  `District_Id` int NOT NULL DEFAULT '1',
  `Best_score` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users_scores`
--

INSERT INTO `Users_scores` (`Id`, `User_id`, `District_Id`, `Best_score`) VALUES
(1, 1, 1, 9999),
(2, 2, 1, 9500),
(3, 3, 2, 9300),
(4, 4, 1, 9300),
(5, 5, 1, 8700),
(6, 6, 1, 8450),
(7, 7, 1, 8200),
(8, 8, 1, 7900),
(9, 9, 1, 7650),
(10, 10, 1, 7630),
(11, 11, 1, 7614),
(12, 12, 1, 7400),
(13, 13, 1, 7200),
(14, 14, 1, 7200),
(15, 15, 1, 7000),
(16, 16, 1, 6800),
(17, 17, 1, 6600),
(18, 18, 1, 6400),
(19, 19, 1, 6200),
(20, 20, 1, 6100),
(21, 21, 1, 6100),
(22, 22, 1, 5900),
(23, 23, 1, 5800),
(24, 24, 1, 5700),
(25, 25, 1, 5500),
(26, 26, 1, 5300),
(27, 27, 1, 5200),
(28, 28, 1, 5000),
(29, 29, 1, 4900),
(30, 30, 1, 4800),
(31, 31, 1, 4700),
(32, 32, 1, 4600),
(33, 33, 1, 4500),
(34, 34, 1, 4400),
(35, 35, 1, 4300),
(36, 37, 1, 4200),
(37, 38, 1, 4100),
(38, 39, 1, 4000),
(39, 40, 1, 4050),
(40, 41, 1, 3900),
(41, 42, 1, 3800),
(42, 43, 1, 3700),
(43, 44, 1, 3600),
(44, 45, 1, 3500),
(45, 46, 1, 3500),
(46, 47, 1, 3400),
(47, 48, 1, 3300),
(48, 49, 1, 3200),
(49, 50, 1, 3100),
(50, 51, 1, 3000),
(51, 52, 1, 2900),
(52, 53, 1, 2800),
(53, 54, 1, 2700),
(54, 55, 1, 2600),
(55, 56, 1, 2500),
(61, 62, 1, 0),
(62, 63, 1, 0),
(63, 64, 1, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `Users_stats`
--

CREATE TABLE `Users_stats` (
  `Id` int NOT NULL,
  `User_id` int NOT NULL,
  `Games_played_count` int NOT NULL DEFAULT '0',
  `Blocks_placed_count` int NOT NULL DEFAULT '0',
  `IBlocks_placed_count` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users_stats`
--

INSERT INTO `Users_stats` (`Id`, `User_id`, `Games_played_count`, `Blocks_placed_count`, `IBlocks_placed_count`) VALUES
(2, 63, 0, 0, 0),
(3, 64, 0, 0, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `Users_upgrades`
--

CREATE TABLE `Users_upgrades` (
  `Id` int NOT NULL,
  `User_id` int NOT NULL,
  `Upgrade_id` int NOT NULL,
  `Level` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users_upgrades`
--

INSERT INTO `Users_upgrades` (`Id`, `User_id`, `Upgrade_id`, `Level`) VALUES
(111, 62, 1, 0),
(112, 62, 2, 0),
(113, 62, 3, 0),
(115, 62, 5, 0),
(116, 63, 1, 0),
(117, 63, 2, 0),
(118, 63, 3, 0),
(120, 63, 5, 0),
(122, 64, 1, 0),
(123, 64, 2, 0),
(124, 64, 3, 0),
(125, 64, 5, 0),
(126, 64, 6, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `Users_wallet`
--

CREATE TABLE `Users_wallet` (
  `Id` int NOT NULL,
  `User_id` int NOT NULL,
  `Money` int NOT NULL DEFAULT '0',
  `Reputation` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Users_wallet`
--

INSERT INTO `Users_wallet` (`Id`, `User_id`, `Money`, `Reputation`) VALUES
(23, 62, 0, 0),
(24, 63, 0, 0),
(25, 64, 0, 0);

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `Achievements`
--
ALTER TABLE `Achievements`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `achievements_ibfk_1` (`District_Id`);

--
-- Индексы таблицы `Bonuses`
--
ALTER TABLE `Bonuses`
  ADD PRIMARY KEY (`Id`);

--
-- Индексы таблицы `Districts`
--
ALTER TABLE `Districts`
  ADD PRIMARY KEY (`Id`);

--
-- Индексы таблицы `Upgrades`
--
ALTER TABLE `Upgrades`
  ADD PRIMARY KEY (`Id`);

--
-- Индексы таблицы `Upgrades_cost`
--
ALTER TABLE `Upgrades_cost`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `Upgrade_id` (`Upgrade_id`);

--
-- Индексы таблицы `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `nickname` (`Nickname`);

--
-- Индексы таблицы `Users_achievements`
--
ALTER TABLE `Users_achievements`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `users_achievements_ibfk_1` (`User_Id`),
  ADD KEY `users_achievements_ibfk_2` (`Achievement_Id`);

--
-- Индексы таблицы `Users_bonuses`
--
ALTER TABLE `Users_bonuses`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `User_id` (`User_id`),
  ADD KEY `Bonus_id` (`Bonus_id`);

--
-- Индексы таблицы `Users_gifts`
--
ALTER TABLE `Users_gifts`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `user_id` (`User_id`);

--
-- Индексы таблицы `Users_scores`
--
ALTER TABLE `Users_scores`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `user_id` (`User_id`) USING BTREE,
  ADD KEY `District_Id` (`District_Id`) USING BTREE;

--
-- Индексы таблицы `Users_stats`
--
ALTER TABLE `Users_stats`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `User_id` (`User_id`);

--
-- Индексы таблицы `Users_upgrades`
--
ALTER TABLE `Users_upgrades`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `User_id` (`User_id`),
  ADD KEY `Upgrade_id` (`Upgrade_id`);

--
-- Индексы таблицы `Users_wallet`
--
ALTER TABLE `Users_wallet`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `User_id` (`User_id`) USING BTREE;

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `Achievements`
--
ALTER TABLE `Achievements`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `Bonuses`
--
ALTER TABLE `Bonuses`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT для таблицы `Districts`
--
ALTER TABLE `Districts`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `Upgrades`
--
ALTER TABLE `Upgrades`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT для таблицы `Upgrades_cost`
--
ALTER TABLE `Upgrades_cost`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT для таблицы `Users`
--
ALTER TABLE `Users`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=76;

--
-- AUTO_INCREMENT для таблицы `Users_achievements`
--
ALTER TABLE `Users_achievements`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `Users_bonuses`
--
ALTER TABLE `Users_bonuses`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=83;

--
-- AUTO_INCREMENT для таблицы `Users_gifts`
--
ALTER TABLE `Users_gifts`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=33;

--
-- AUTO_INCREMENT для таблицы `Users_scores`
--
ALTER TABLE `Users_scores`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=66;

--
-- AUTO_INCREMENT для таблицы `Users_stats`
--
ALTER TABLE `Users_stats`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `Users_upgrades`
--
ALTER TABLE `Users_upgrades`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=137;

--
-- AUTO_INCREMENT для таблицы `Users_wallet`
--
ALTER TABLE `Users_wallet`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `Achievements`
--
ALTER TABLE `Achievements`
  ADD CONSTRAINT `achievements_ibfk_1` FOREIGN KEY (`District_Id`) REFERENCES `Districts` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Upgrades_cost`
--
ALTER TABLE `Upgrades_cost`
  ADD CONSTRAINT `upgrades_cost_ibfk_1` FOREIGN KEY (`Upgrade_id`) REFERENCES `Upgrades` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Users_achievements`
--
ALTER TABLE `Users_achievements`
  ADD CONSTRAINT `users_achievements_ibfk_1` FOREIGN KEY (`User_Id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  ADD CONSTRAINT `users_achievements_ibfk_2` FOREIGN KEY (`Achievement_Id`) REFERENCES `Achievements` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Users_bonuses`
--
ALTER TABLE `Users_bonuses`
  ADD CONSTRAINT `users_bonuses_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `users_bonuses_ibfk_2` FOREIGN KEY (`Bonus_id`) REFERENCES `Bonuses` (`Id`) ON DELETE CASCADE;

--
-- Ограничения внешнего ключа таблицы `Users_gifts`
--
ALTER TABLE `Users_gifts`
  ADD CONSTRAINT `users_gifts_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Users_scores`
--
ALTER TABLE `Users_scores`
  ADD CONSTRAINT `users_scores_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  ADD CONSTRAINT `users_scores_ibfk_2` FOREIGN KEY (`District_Id`) REFERENCES `Districts` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Users_stats`
--
ALTER TABLE `Users_stats`
  ADD CONSTRAINT `users_stats_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Ограничения внешнего ключа таблицы `Users_upgrades`
--
ALTER TABLE `Users_upgrades`
  ADD CONSTRAINT `users_upgrades_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `users_upgrades_ibfk_2` FOREIGN KEY (`Upgrade_id`) REFERENCES `Upgrades` (`Id`) ON DELETE CASCADE;

--
-- Ограничения внешнего ключа таблицы `Users_wallet`
--
ALTER TABLE `Users_wallet`
  ADD CONSTRAINT `users_wallet_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
