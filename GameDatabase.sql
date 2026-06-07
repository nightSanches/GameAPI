-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1:3316
-- Время создания: Июн 05 2026 г., 07:36
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
(5, 1, 'Ночной дозор', 'Пережить ночь в Деловом районе', 'event_night', 1, 100),
(6, 1, 'Сейсмостоустойчивость', 'Пережить землетрясение в Деловом районе', 'event_shake', 1, 200),
(7, 1, 'Повелитель ветра', 'Пережить сильный ветер в Деловом районе', 'event_wind', 1, 300),
(8, 1, 'Первая десятка!', 'Набрать 10 000 очков в Деловом районе', 'score_1', 10000, 100),
(9, 1, 'Знаток своего дела', 'Достичь 30 000 очков в Деловом районе', 'score_2', 30000, 300),
(10, 1, 'Профессиональный строитель', 'Преодолеть отметку в 60 000 очков в Деловом районе', 'score_3', 60000, 600),
(11, 1, 'Эверест', 'Набрать 100 000 очков в Деловом районе', 'score_4', 100000, 1000),
(12, 1, 'Идеальная десятка', 'Построить 10 идеальных этажей подряд в Деловом районе', 'ideal_1', 10, 100),
(13, 1, 'Двадцать без изъяна', 'Построить 20 идеальных этажей подряд в Деловом районе', 'ideal_2', 20, 300),
(14, 1, 'Ровно в ряд', 'Построить 50 идеальных этажей в одном забеге в Деловом районе', 'all_ideal_1', 50, 200),
(15, 1, 'Построено на века', 'Построить 100 идеальных этажей в одном забеге в Деловом районе', 'all_ideal_2', 100, 500),
(16, 2, 'Укротитель тьмы', 'Пережить ночь в Центре города', 'event_night', 1, 300),
(17, 2, 'Крепыш', 'Пережить землетрясение в Центре города', 'event_shake', 1, 600),
(18, 2, 'Непоколебимый', 'Пережить сильный ветер в Центре города', 'event_wind', 1, 900),
(19, 2, 'Первая городская', 'Набрать 10 000 очков в Центре города', 'score_1', 10000, 300),
(20, 2, 'Архитектор среднего звена', 'Достичь 30 000 очков в Центре города', 'score_2', 30000, 900),
(21, 2, 'Градостроительный магнат', 'Преодолеть отметку в 60 000 очков в Центре города', 'score_3', 60000, 1800),
(22, 2, 'Король небоскрёбов', 'Набрать 100 000 очков в Центре города', 'score_4', 100000, 3000),
(23, 2, 'Золотая серия', 'Построить 10 идеальных этажей подряд в Центре города', 'ideal_1', 10, 300),
(24, 2, 'Бриллиантовый пояс', 'Построить 30 идеальных этажей подряд в Центре города', 'ideal_2', 30, 900),
(25, 2, 'Безупречный силуэт', 'Построить 50 идеальных этажей в одном забеге в Центре города', 'all_ideal_1', 50, 600),
(26, 2, 'Город-совершенство', 'Построить 100 идеальных этажей в одном забеге в Центре города', 'all_ideal_2', 100, 1500),
(27, 3, 'Ночные эксперименты', 'Пережить ночь в Научном районе', 'event_night', 1, 900),
(28, 3, 'Сдвиг по фазе', 'Пережить землетрясение в Научном районе', 'event_shake', 1, 1800),
(29, 3, 'Ветрянная мельница', 'Пережить сильный ветер в Научном районе', 'event_wind', 1, 2700),
(30, 3, 'Младший научный сотрудник', 'Набрать 10 000 очков в Научном районе', 'score_1', 10000, 900),
(31, 3, 'Кандидат наук', 'Достичь 30 000 очков в Научном районе', 'score_2', 30000, 2700),
(32, 3, 'Нобелевский лоуреат', 'Преодолеть отметку в 60 000 очков в Научном районе', 'score_3', 60000, 5400),
(33, 3, 'Космический лифт', 'Набрать 100 000 очков в Научном районе', 'score_4', 100000, 9000),
(34, 3, 'Основы физики', 'Построить 10 идеальных этажей подряд в Научном районе', 'ideal_1', 10, 900),
(35, 3, 'Квантовая механика', 'Построить 30 идеальных этажей подряд в Научном районе', 'ideal_2', 30, 2700),
(36, 3, 'Формула успеха', 'Построить 50 идеальных этажей в одном забеге в Научном районе', 'all_ideal_1', 50, 1800),
(37, 3, 'Ноль погрешности', 'Построить 100 идеальных этажей в одном забеге в Научном районе', 'all_ideal_2', 100, 4500);

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
(2, 'Регулятор', 'Выравнивает небоскрёб по центру', 50),
(3, 'Магнит', 'Временно притягивает блоки к центру', 50);

-- --------------------------------------------------------

--
-- Структура таблицы `Districts`
--

CREATE TABLE `Districts` (
  `Id` int NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Unlock_Rep_Req` int NOT NULL DEFAULT '0',
  `Difficulty_Multiplier` decimal(4,2) NOT NULL DEFAULT '1.00',
  `Sort_Order` int NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Дамп данных таблицы `Districts`
--

INSERT INTO `Districts` (`Id`, `Name`, `Unlock_Rep_Req`, `Difficulty_Multiplier`, `Sort_Order`) VALUES
(1, 'Деловой район', 0, '3.00', 1),
(2, 'Центр города', 3000, '9.00', 2),
(3, 'Научный район', 9000, '18.00', 3);

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
(1, 'Стабильность крана', 'Уменьшает шатание крана'),
(2, 'Фундамент', 'Уменьшает шатание небоскреба'),
(3, 'Страховка', 'Увеличивает количество разрешенных промахов'),
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
(13, 5, 1, 10),
(14, 5, 2, 20),
(15, 5, 3, 30),
(16, 6, 1, 10),
(17, 6, 2, 20),
(18, 6, 3, 30),
(19, 3, 1, 10),
(20, 3, 2, 20),
(21, 3, 3, 30),
(22, 5, 4, 40),
(23, 5, 5, 50),
(24, 5, 6, 60),
(25, 5, 7, 70),
(26, 5, 8, 80),
(27, 5, 9, 90),
(28, 5, 10, 100),
(29, 1, 4, 40),
(30, 1, 5, 50),
(31, 1, 6, 60),
(32, 1, 7, 70),
(33, 1, 8, 80),
(34, 1, 9, 90),
(35, 1, 10, 100),
(36, 1, 11, 110),
(37, 1, 12, 120),
(38, 1, 13, 130),
(39, 1, 14, 140),
(40, 1, 15, 150),
(41, 1, 16, 160),
(42, 1, 17, 170),
(44, 2, 4, 40),
(45, 2, 5, 50),
(46, 2, 6, 60),
(47, 2, 7, 70),
(48, 2, 8, 80),
(49, 2, 9, 90),
(50, 2, 10, 100),
(51, 2, 11, 110),
(52, 2, 12, 120),
(53, 2, 13, 130),
(54, 2, 14, 140),
(55, 2, 15, 150),
(56, 2, 16, 160),
(57, 2, 17, 170),
(58, 1, 18, 180),
(59, 2, 18, 180);

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
(56, 'Player_56', 'hashedpassword', 'player', NULL, NULL, '2026-04-27 19:28:57', 0, NULL, NULL);

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

-- --------------------------------------------------------

--
-- Структура таблицы `Users_gifts`
--

CREATE TABLE `Users_gifts` (
  `Id` int NOT NULL,
  `User_id` int NOT NULL,
  `Last_bonus_dt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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
(3, 3, 1, 9300),
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
(55, 56, 1, 2500);

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
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

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
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=60;

--
-- AUTO_INCREMENT для таблицы `Users`
--
ALTER TABLE `Users`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=87;

--
-- AUTO_INCREMENT для таблицы `Users_achievements`
--
ALTER TABLE `Users_achievements`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=288;

--
-- AUTO_INCREMENT для таблицы `Users_bonuses`
--
ALTER TABLE `Users_bonuses`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=116;

--
-- AUTO_INCREMENT для таблицы `Users_gifts`
--
ALTER TABLE `Users_gifts`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=44;

--
-- AUTO_INCREMENT для таблицы `Users_scores`
--
ALTER TABLE `Users_scores`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=97;

--
-- AUTO_INCREMENT для таблицы `Users_stats`
--
ALTER TABLE `Users_stats`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT для таблицы `Users_upgrades`
--
ALTER TABLE `Users_upgrades`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=193;

--
-- AUTO_INCREMENT для таблицы `Users_wallet`
--
ALTER TABLE `Users_wallet`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=39;

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
