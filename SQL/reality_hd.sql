-- phpMyAdmin SQL Dump
-- version 4.6.6deb5
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Erstellungszeit: 20. Mai 2020 um 14:15
-- Server-Version: 5.7.29-0ubuntu0.18.04.1
-- PHP-Version: 7.2.24-0ubuntu0.18.04.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `reality_hd`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `buddylist`
--

CREATE TABLE `buddylist` (
  `id` int(11) NOT NULL,
  `charId` int(11) NOT NULL,
  `friendId` int(11) NOT NULL,
  `is_ignored` tinyint(4) NOT NULL DEFAULT '0',
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `buddylist`
--

INSERT INTO `buddylist` (`id`, `charId`, `friendId`, `is_ignored`, `deleted_at`) VALUES
(1, 3, 53, 0, NULL);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `characters`
--

CREATE TABLE `characters` (
  `charId` bigint(11) UNSIGNED NOT NULL,
  `userId` int(11) UNSIGNED NOT NULL,
  `worldId` smallint(5) UNSIGNED NOT NULL,
  `status` tinyint(3) UNSIGNED NOT NULL COMMENT 'transit/banned',
  `handle` varchar(32) NOT NULL,
  `firstName` varchar(32) NOT NULL DEFAULT '',
  `lastName` varchar(32) NOT NULL DEFAULT '',
  `background` varchar(1024) DEFAULT '',
  `x` float NOT NULL DEFAULT '17020',
  `y` float NOT NULL DEFAULT '495',
  `z` float NOT NULL DEFAULT '2693',
  `rotation` mediumint(11) NOT NULL DEFAULT '139',
  `healthC` mediumint(11) NOT NULL DEFAULT '500',
  `healthM` mediumint(11) NOT NULL DEFAULT '500',
  `innerStrC` mediumint(11) NOT NULL DEFAULT '200',
  `innerStrM` mediumint(11) NOT NULL DEFAULT '200',
  `level` mediumint(11) NOT NULL DEFAULT '1',
  `profession` smallint(6) NOT NULL DEFAULT '2',
  `alignment` smallint(6) NOT NULL DEFAULT '0',
  `pvpflag` smallint(6) NOT NULL DEFAULT '0',
  `exp` int(11) NOT NULL DEFAULT '0',
  `cash` int(11) NOT NULL DEFAULT '1000',
  `repMero` int(11) NOT NULL DEFAULT '0',
  `repMachine` int(11) NOT NULL DEFAULT '0',
  `repNiobe` int(11) NOT NULL DEFAULT '0',
  `repEPN` int(11) NOT NULL DEFAULT '0',
  `repCYPH` int(11) NOT NULL DEFAULT '0',
  `repGM` int(11) NOT NULL DEFAULT '0',
  `repZion` int(11) NOT NULL DEFAULT '0',
  `district` varchar(256) NOT NULL DEFAULT 'slums',
  `districtId` tinyint(3) UNSIGNED NOT NULL DEFAULT '1' COMMENT 'Default must be changed to 0 (tutorial) later',
  `factionId` mediumint(8) UNSIGNED NOT NULL DEFAULT '0',
  `crewId` mediumint(8) UNSIGNED NOT NULL DEFAULT '0',
  `is_deleted` int(11) NOT NULL DEFAULT '0',
  `is_online` tinyint(4) NOT NULL DEFAULT '0',
  `created` datetime NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `characters`
--

INSERT INTO `characters` (`charId`, `userId`, `worldId`, `status`, `handle`, `firstName`, `lastName`, `background`, `x`, `y`, `z`, `rotation`, `healthC`, `healthM`, `innerStrC`, `innerStrM`, `level`, `profession`, `alignment`, `pvpflag`, `exp`, `cash`, `repMero`, `repMachine`, `repNiobe`, `repEPN`, `repCYPH`, `repGM`, `repZion`, `district`, `districtId`, `factionId`, `crewId`, `is_deleted`, `is_online`, `created`) VALUES
(2, 28, 1, 0, 'TheNeo', 'Mr', 'Neo', 'Has all shit loaded for Development! YEAH!', 111375, 694.999, -7189.77, 0, 500, 500, 200, 200, 50, 2, 0, 0, 314690958, 987704321, 0, 0, 0, 0, 0, 0, 100, 'slums', 1, 125, 1, 0, 0, '0000-00-00 00:00:00'),
(3, 28, 1, 0, 'TheLevelOne', 'Testing', 'Chat', 'Just testing', 9483.31, 95, 13039.4, 0, 500, 500, 200, 200, 20, 2, 1, 0, 19359020, 105000, 20, -50, -10, -5, -100, 200, 10, 'slums', 1, 0, 0, 1, 0, '0000-00-00 00:00:00'),
(111, 48, 1, 0, 'LaurinHill', 'Laurin', 'Hill', 'That Thing', 17020, 495, 2693, 139, 500, 500, 200, 200, 1, 2, 0, 0, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 'slums', 1, 125, 2, 0, 0, '2017-01-28 12:17:36'),
(141, 33, 1, 0, 'Trinitys', 'Laurin', 'Hill', 'Trinity', 67661.2, -105, -10658.6, 0, 500, 500, 200, 200, 1, 2, 0, 0, 224466, 1000, 0, 0, 0, 0, 0, 0, 0, 'slums', 1, 0, 0, 0, 0, '2017-10-15 13:43:42'),
(110, 28, 1, 0, 'CSR_Tester', 'dasd', 'asd', 'asdasd', 7578, 1595, -12250.2, 0, 500, 500, 200, 200, 1, 2, 0, 0, 2540, 26000, 0, 0, 0, 0, 0, 0, 0, 'tutorial', 0, 0, 1, 0, 0, '2016-03-15 13:21:02'),
(97, 33, 1, 0, 'TesterArchive', 'Tester', 'Tester', 'Tester', 9606, 495, 1585, 209, 500, 500, 200, 200, 1, 2, 0, 0, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 'archive02', 5, 0, 0, 0, 0, '2016-03-14 15:46:41'),
(142, 28, 1, 0, 'NeoAnderbro', 'Thomas', 'Anderson', 'The cake is a lie', -7321.94, 95, -1847.68, 65, 500, 500, 200, 200, 1, 2, 0, 0, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 'slums', 13, 125, 1, 0, 0, '2020-04-12 12:48:02'),
(143, 50, 1, 0, 'entilsar', 'Martin', 'Kosital', 'My mate', 17012.8, 495, 2787.72, 11, 500, 500, 200, 200, 1, 2, 0, 0, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 'slums', 1, 0, 0, 0, 0, '2020-05-19 09:58:08'),
(145, 50, 1, 0, 'antietam305', 'antie', 'tam', '304', 17020, 495, 2693, 139, 500, 500, 200, 200, 1, 2, 0, 0, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 'slums', 1, 0, 0, 0, 0, '2020-05-19 10:01:06');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `char_abilities`
--

CREATE TABLE `char_abilities` (
  `id` int(10) UNSIGNED NOT NULL,
  `char_id` int(10) UNSIGNED NOT NULL,
  `slot` int(11) NOT NULL,
  `ability_id` bigint(11) NOT NULL,
  `level` int(10) UNSIGNED NOT NULL,
  `is_loaded` tinyint(1) NOT NULL DEFAULT '0',
  `ability_name` varchar(255) NOT NULL DEFAULT '',
  `added` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `char_abilities`
--

INSERT INTO `char_abilities` (`id`, `char_id`, `slot`, `ability_id`, `level`, `is_loaded`, `ability_name`, `added`) VALUES
(1, 3, 0, -2147481600, 50, 1, 'Awaken', '2012-01-11 15:23:32'),
(2, 3, 1, -2147367936, 2, 1, 'CombatInsightAbility', '2012-01-11 15:23:32'),
(3, 3, 2, -2147294208, 0, 0, 'Intense', '2012-01-11 15:25:42'),
(6, 3, 3, -2147350528, 0, 1, 'HinderingShotAbility', '2012-01-11 17:15:23'),
(7, 3, 4, -2147281920, 0, 1, 'Headbut Ability', '2012-01-12 06:53:21'),
(8, 3, 5, -2147280896, 0, 0, 'CheapShotAbility', '2012-01-12 06:53:21'),
(21, 3, 6, -2147437568, 2, 1, 'ExecuteProgramAbility', '2012-01-20 21:24:03'),
(22, 3, 7, -2147425280, 0, 1, 'LogicBlast1Ability', '2012-01-20 21:24:03'),
(23, 3, 8, -2147404800, 0, 1, 'RestoreHealth1Ability', '2012-01-20 21:27:40'),
(24, 3, 9, -2147445760, 50, 1, 'WriteCodeAbility', '2012-01-20 21:27:40'),
(25, 3, 10, -2146493440, 0, 1, 'Mobius Code', '2012-02-02 16:11:42'),
(26, 3, 11, -2146453504, 20, 1, 'Hacker Style', '2012-02-02 16:13:21'),
(27, 3, 12, -2147472384, 30, 1, 'HyperJump', '2012-02-02 16:14:27'),
(28, 3, 13, -2147295232, 20, 1, 'Hyper Sprint', '2012-02-02 16:17:04'),
(128, 3, 14, -2147466240, 1, 1, '', '2014-05-22 20:51:57'),
(129, 3, 2, -2147364864, 1, 0, '', '0000-00-00 00:00:00'),
(130, 2, 1, -2147295232, 1, 0, 'HyperSprintAbility', '2016-02-10 21:31:13'),
(132, 2, 0, -2147481600, 50, 1, ' \'Awaken\'', '2016-02-10 21:58:09'),
(133, 2, 1, -2147367936, 2, 0, ' \'CombatInsightAbility\'', '2016-02-10 21:58:09'),
(134, 2, 2, -2147294208, 0, 0, ' \'Intense\'', '2016-02-10 21:58:09'),
(135, 2, 3, -2147350528, 0, 0, ' \'HinderingShotAbility\'', '2016-02-10 21:58:09'),
(136, 2, 4, -2147281920, 0, 1, ' \'Headbut Ability\'', '2016-02-10 21:58:09'),
(137, 2, 5, -2147280896, 0, 1, ' \'CheapShotAbility\'', '2016-02-10 21:58:09'),
(138, 2, 6, -2147437568, 2, 1, ' \'E2ecuteProgramAbility\'', '2016-02-10 21:58:09'),
(139, 2, 7, -2147425280, 0, 0, ' \'LogicBlast1Ability\'', '2016-02-10 21:58:09'),
(140, 2, 8, -2147404800, 0, 1, ' \'RestoreHealth1Ability\'', '2016-02-10 21:58:09'),
(141, 2, 9, -2147445760, 50, 1, '\'WriteCodeAbility', '2016-02-10 21:58:09'),
(142, 2, 10, -2146493440, 0, 1, ' \'Mobius Code\'', '2016-02-10 21:58:09'),
(143, 2, 11, -2146453504, 20, 1, ' \'Hacker Style\'', '2016-02-10 21:58:09'),
(144, 2, 13, -2147472384, 50, 1, ' \'HyperJump\'', '2016-02-10 21:58:09'),
(145, 2, 14, -2147261440, 0, 1, ' \'EvadeCombatAbility\'', '2016-02-10 21:58:09'),
(146, 2, 15, -2147471360, 0, 1, ' \'HyperSpeedAbility\'', '2016-02-10 21:58:09'),
(147, 2, 16, -2147471360, 0, 1, ' \'HyperSpeedAbility\'', '2016-02-10 21:58:09'),
(148, 2, 17, -2147364864, 0, 0, ' \'OverhandSmashAbility\'', '2016-02-10 21:58:09'),
(149, 2, 18, -2147295232, 20, 1, ' \'Hyper Sprint\'', '2016-02-10 21:58:09'),
(150, 2, 22, -2147473408, 0, 0, ' HyperDodgeAbility', '2016-02-10 21:58:09'),
(151, 2, 19, -2146451456, 5, 1, ' \'CoderCombatStyleAbility\'', '2016-02-10 21:58:09'),
(152, 2, 20, -2147456000, 5, 1, ' \'RemotePro2y1Ability\'', '2016-02-10 21:58:09'),
(153, 2, 21, -2147465216, 0, 1, ' \'CoderAbility\'', '2016-02-10 21:58:09'),
(154, 2, 22, -2147465216, 0, 1, ' \'CoderAbility\'', '2016-02-10 21:58:09'),
(155, 2, 23, -2147465216, 0, 1, ' \'CoderAbility\' ', '2016-02-10 21:58:09'),
(156, 2, 24, -2147465216, 0, 0, ' \'CoderAbility\'', '2016-02-10 21:58:09'),
(157, 2, 25, -2147418112, 0, 1, ' \'NetworkHackerAbility\'', '2016-02-10 21:58:09'),
(158, 2, 26, -2147437568, 0, 0, ' \'E2ecuteProgramAbility\'', '2016-02-10 21:58:09'),
(159, 2, 27, -2147436544, 0, 0, ' \'FastHealing1Ability\'', '2016-02-10 21:58:09'),
(160, 2, 28, -2147435520, 0, 0, ' \'FreezeArea1Ability\'', '2016-02-10 21:58:09'),
(161, 2, 29, -2147434496, 0, 0, ' \'FreezeSystem1Ability\'', '2016-02-10 21:58:09'),
(162, 2, 30, -2147433472, 0, 1, ' \'GaussianBlur1Ability\'', '2016-02-10 21:58:09'),
(163, 2, 31, -2147432448, 0, 1, ' \'GroupRepairs2Ability\'', '2016-02-10 21:58:09'),
(164, 2, 32, -2147431424, 0, 0, ' \'GuardianHealerAbility\'', '2016-02-10 21:58:09'),
(165, 2, 33, -2147430400, 0, 1, ' \'HackerAbility\'', '2016-02-10 21:58:09'),
(166, 2, 34, -2147429376, 0, 0, ' \'HarmfulCodeAbility\'', '2016-02-10 21:58:09'),
(167, 2, 35, -2147428352, 0, 0, ' \'RestoreHealth4Ability\'', '2016-02-10 21:58:09'),
(168, 2, 36, -2147427328, 0, 1, ' \'HealerAbility\'', '2016-02-10 21:58:09'),
(169, 2, 37, -2147426304, 0, 1, ' \'GroupRepairs1Ability\'', '2016-02-10 21:58:09'),
(170, 2, 38, -2147425280, 0, 1, ' \'LogicBlast1Ability\'', '2016-02-10 21:58:09'),
(171, 2, 39, -2147424256, 0, 1, ' \'LogicBlast2Ability\'', '2016-02-10 21:58:09'),
(172, 2, 40, -2147423232, 0, 0, ' \'LogicBlast3Ability\'', '2016-02-10 21:58:09'),
(173, 2, 41, -2147422208, 0, 0, ' \'LogicBomb1Ability\'', '2016-02-10 21:58:09'),
(174, 2, 42, -2147421184, 0, 0, ' \'Miasma1Ability\'', '2016-02-10 21:58:09'),
(175, 2, 43, -2147420160, 0, 1, ' \'MovementAccelerator1Ability\'', '2016-02-10 21:58:09'),
(176, 2, 44, -2147419136, 0, 1, ' \'NetworkFirewall1Ability\'', '2016-02-10 21:58:09'),
(177, 2, 45, -2147418112, 0, 0, ' \'NetworkHackerAbility\'', '2016-02-10 21:58:09'),
(178, 2, 46, -2147417088, 0, 0, ' \'Overheat1Ability\'', '2016-02-10 21:58:09'),
(179, 2, 47, -2147416064, 0, 0, ' \'Overload1Ability\'', '2016-02-10 21:58:09'),
(180, 2, 48, -2147415040, 0, 0, ' \'PathogenistAbility\'', '2016-02-10 21:58:09'),
(181, 2, 49, -2147414016, 0, 0, ' \'PersonalFirewall1Ability\'', '2016-02-10 21:58:09'),
(182, 2, 50, -2147412992, 0, 0, ' \'ProcessorLag1Ability\'', '2016-02-10 21:58:09'),
(183, 2, 51, -2147411968, 15, 0, ' \'QuickRecoveryAbility\'', '2016-02-10 21:58:09'),
(184, 2, 52, -2147410944, 20, 1, ' \'RepairAndUpgradeAbility\'', '2016-02-10 21:58:09'),
(185, 2, 53, -2147409920, 0, 1, ' \'ResistCombat1Ability\'', '2016-02-10 21:58:09'),
(186, 2, 54, -2147408896, 0, 0, ' \'ResistContagionAbility\'', '2016-02-10 21:58:09'),
(187, 2, 55, -2147407872, 15, 1, ' \'ResistDamageAbility\'', '2016-02-10 21:58:09'),
(188, 2, 56, -2147406848, 15, 0, ' \'ResistInfectionAbility\'', '2016-02-10 21:58:09'),
(189, 2, 57, -2147405824, 15, 1, ' \'ResistVirusesAbility\'', '2016-02-10 21:58:09'),
(190, 2, 58, -2147404800, 0, 1, ' \'RestoreHealth1Ability\'', '2016-02-10 21:58:09'),
(191, 2, 59, -2147403776, 0, 0, ' \'FocusAttributeAbility\'', '2016-02-10 21:58:09'),
(192, 2, 60, -2147402752, 0, 0, ' \'PerceptionAttributeAbility\'', '2016-02-10 21:58:09'),
(193, 2, 61, -2147401728, 0, 1, ' \'RestoreHealth2Ability\'', '2016-02-10 21:58:09'),
(194, 2, 62, -2147400704, 0, 0, ' \'ReasonAttributeAbility\'', '2016-02-10 21:58:09'),
(195, 2, 63, -2147399680, 0, 0, ' \'BeliefAttributeAbility\'', '2016-02-10 21:58:09'),
(196, 2, 64, -2147398656, 0, 0, ' \'SelectivePhageAbility\'', '2016-02-10 21:58:09'),
(197, 2, 65, -2147397632, 0, 0, ' \'VitalityAttributeAbility\'', '2016-02-10 21:58:09'),
(198, 2, 66, -2147396608, 0, 0, ' \'MezAbility\'', '2016-02-10 21:58:09'),
(199, 2, 67, -2147395584, 0, 1, ' \'Slow1Ability\'', '2016-02-10 21:58:09'),
(200, 2, 68, -2147394560, 0, 0, ' \'HasteAbility\'', '2016-02-10 21:58:09'),
(201, 0, 0, 0, 0, 0, '', '2016-02-10 21:58:09'),
(258, 2, 69, -2147446784, 0, 1, 'TinkeringAbility', '2016-02-13 01:32:38'),
(259, 2, 70, 2147452928, 0, 0, 'RepairSimulacra1Ability', '2016-02-13 01:33:00'),
(260, 2, 71, -2147451904, 0, 1, 'RepairSimulacra2Ability', '2016-02-13 01:33:26'),
(261, 2, 72, -2147460096, 15, 1, 'DeflectCodeAbility', '2016-02-13 01:35:21'),
(262, 2, 73, -2147461120, 0, 1, 'LogicDaemon1Ability', '2016-02-13 01:42:44'),
(263, 2, 74, -2147456000, 0, 0, 'RemoteProxy1Ability', '2016-02-13 01:43:23'),
(264, 2, 75, -2147463168, 0, 1, 'FortifySimulacra1Ability', '2016-02-13 01:43:51'),
(265, 2, 76, -2147449856, 0, 0, 'StaticBlast1Ability', '2016-02-13 01:45:20'),
(266, 2, 77, -2147450880, 20, 0, 'RunProgramAbility', '2016-02-13 01:45:38'),
(267, 2, 78, -2147448832, 0, 0, 'StopProxy1Ability', '2016-02-13 01:46:12'),
(268, 2, 79, -2147447808, 0, 1, 'CodeShaperAbility', '2016-02-13 01:46:30'),
(269, 2, 80, -2147454976, 0, 1, 'RemoteProxy2Ability', '2016-02-13 01:47:10'),
(270, 2, 81, -2147453952, 0, 0, 'RemoteProxy3Ability', '2016-02-13 01:47:21'),
(271, 2, 82, -2147462144, 0, 0, 'Lockdown1Ability', '2016-02-13 01:48:42'),
(272, 2, 83, -2147458048, 0, 0, 'PowerBoost1Ability', '2016-02-13 01:48:32'),
(273, 2, 84, -2146884608, 30, 0, 'RunAdvancedProgramAbility', '2016-02-13 01:49:35'),
(274, 2, 85, -2146881536, 0, 0, 'Enrage1Ability', '2016-02-13 01:50:02'),
(275, 2, 86, -2146883584, 0, 0, 'RemoteProxy4Ability', '2016-02-13 01:50:25'),
(276, 2, 87, -2146880512, 0, 0, 'ProxyTechnicianAbility', '2016-02-13 01:50:59'),
(277, 2, 88, -2146879488, 0, 0, 'StaticBlast2Ability', '2016-02-13 01:52:02'),
(278, 2, 89, -2146878464, 0, 0, 'StaticBlast3Ability', '2016-02-13 01:52:14'),
(279, 2, 90, -2146875392, 0, 0, 'RemoteProxy5Ability', '2016-02-13 01:53:08'),
(280, 2, 91, -2147457024, 0, 0, 'ProxyCoderAbility', '2016-02-13 01:57:32'),
(281, 2, 92, -2146876416, 0, 0, 'TransmitCodeAbility', '2016-02-13 01:58:15'),
(282, 2, 93, -2146871296, 0, 0, 'StaticBlast4Ability', '2016-02-13 01:59:14'),
(283, 2, 94, -2146870272, 0, 0, 'ProxyMasterAbility', '2016-02-13 01:59:36'),
(284, 2, 95, -2146872320, 0, 0, 'Enrage2Ability', '2016-02-13 02:00:04'),
(285, 2, 96, -2147386368, 15, 1, 'TransmitVirusAbility', '2016-02-13 02:04:46'),
(286, 2, 97, -2147385344, 0, 1, 'PowerlessAbility', '2016-02-13 02:05:46'),
(287, 2, 98, -2147384320, 0, 1, 'UILag1Ability', '2016-02-13 02:06:10'),
(288, 2, 99, -2147383296, 0, 1, 'ViralAttackerAbility', '2016-02-13 02:06:32'),
(289, 2, 100, -2147382272, 0, 0, 'RootedAbility', '2016-02-13 02:07:02'),
(290, 2, 101, -2147439616, 0, 1, 'DisruptInputs1Ability', '2016-02-13 02:13:30'),
(291, 2, 102, -2147442688, 0, 1, 'CodeFreeze1Ability', '2016-02-13 02:13:53'),
(292, 2, 103, -2147441664, 0, 0, 'CodeFreeze2Ability', '2016-02-13 02:14:10'),
(293, 2, 104, -2147440640, 0, 1, 'CombatAura1Ability', '2016-02-13 02:15:07'),
(294, 2, 105, -2147438592, 0, 1, 'DownloadMissionMapAbility', '2016-02-13 02:15:26'),
(295, 2, 106, -2147443712, 0, 0, 'BolsterHealth1Ability', '2016-02-13 02:15:57'),
(296, 2, 107, -2147310592, 0, 1, 'EmergencyRepairs1Ability', '2016-02-13 02:16:30'),
(297, 2, 108, -2147309568, 0, 1, 'PassiveCodeAbility', '2016-02-13 02:16:42'),
(298, 2, 109, -2147306496, 0, 0, 'CombatEnhancement1Ability', '2016-02-13 02:16:59'),
(299, 2, 110, -2147167232, 30, 0, 'StandOffAbility', '2016-02-13 02:17:53'),
(300, 2, 111, -2147163136, 0, 0, 'LogicCannon1Ability', '2016-02-13 02:19:19'),
(301, 2, 112, -2147145728, 0, 0, 'LogicBarrageAbility', '2016-02-13 02:19:54'),
(302, 2, 113, -2147146752, 0, 0, 'LogicBlast6Ability', '2016-02-13 02:20:17'),
(303, 2, 114, -2147161088, 0, 0, 'Stun1Ability', '2016-02-13 02:20:41'),
(304, 2, 115, -2147142656, 0, 0, 'Stun2Ability', '2016-02-13 02:20:55'),
(305, 2, 116, -2147141632, 0, 0, 'BallistaAbility', '2016-02-13 02:21:38'),
(306, 2, 117, -2147140608, 0, 0, 'ArtilleristAbility', '2016-02-13 02:21:50'),
(307, 2, 118, -2147131392, 0, 0, 'CombatHackingAbility', '2016-02-13 02:22:16'),
(308, 2, 119, -2147166208, 0, 0, 'ArbalestAbility', '2016-02-13 02:22:41'),
(309, 2, 120, -2147162112, 0, 0, 'UpgradeAttacksAbility', '2016-02-13 02:23:16'),
(310, 2, 121, -2147160064, 0, 0, 'DodgeAbility', '2016-02-13 02:23:30'),
(311, 2, 122, -2147159040, 0, 0, 'PotencyAbility', '2016-02-13 02:23:45'),
(312, 2, 123, -2147158016, 0, 0, 'UpgradeArtistAbility', '2016-02-13 02:24:17'),
(313, 2, 124, -2147147776, 50, 0, 'CodeBranchingAbility', '2016-02-13 02:24:47'),
(314, 2, 125, -2147444736, 0, 0, 'AreaDisruption1Ability', '2016-02-13 02:25:46'),
(315, 2, 126, -2147129344, 30, 0, 'DedicatedCodeAbility', '2016-02-13 02:26:54'),
(316, 2, 127, -2147394560, 1, 1, 'ViralAttackerAbility', '2016-02-13 22:23:51'),
(515, 97, 0, -2147481600, 1, 1, '', '2016-03-14 15:48:23'),
(516, 97, 1, -2147367936, 1, 1, '', '2016-03-14 15:48:23'),
(517, 97, 2, -2147294208, 0, 1, '', '2016-03-14 15:48:24'),
(518, 97, 3, -2147281920, 0, 0, '', '2016-03-14 15:48:24'),
(519, 97, 4, -2147280896, 0, 0, '', '2016-03-14 15:48:24'),
(520, 97, 5, -2147437568, 1, 1, '', '2016-03-14 15:48:25'),
(521, 97, 6, -2147425280, 0, 0, '', '2016-03-14 15:48:25'),
(522, 97, 7, -2147404800, 0, 0, '', '2016-03-14 15:48:25'),
(523, 97, 8, -2147445760, 0, 0, '', '2016-03-14 15:48:26'),
(524, 97, 9, -2146493440, 0, 0, '', '2016-03-14 15:48:26'),
(525, 97, 10, -2146453504, 1, 0, '', '2016-03-14 15:48:26'),
(625, 110, 0, -2147481600, 1, 1, '', '2016-03-15 13:21:21'),
(626, 110, 1, -2147367936, 1, 1, '', '2016-03-15 13:21:21'),
(627, 110, 2, -2147294208, 0, 1, '', '2016-03-15 13:21:21'),
(628, 110, 3, -2147281920, 0, 1, '', '2016-03-15 13:21:22'),
(629, 110, 4, -2147280896, 0, 1, '', '2016-03-15 13:21:22'),
(630, 110, 5, -2147437568, 1, 1, '', '2016-03-15 13:21:22'),
(631, 110, 6, -2147425280, 0, 1, '', '2016-03-15 13:21:23'),
(632, 110, 7, -2147404800, 0, 1, '', '2016-03-15 13:21:23'),
(633, 110, 8, -2147445760, 0, 1, '', '2016-03-15 13:21:23'),
(634, 110, 9, -2146493440, 0, 1, '', '2016-03-15 13:21:24'),
(635, 110, 10, -2146453504, 1, 1, '', '2016-03-15 13:21:24'),
(636, 2, 128, -2147396608, 0, 0, '', '2016-06-02 16:51:49'),
(637, 2, 129, -2147416064, 0, 0, 'Overload1Ability', '2016-06-02 22:12:30'),
(638, 2, 130, -2146046976, 0, 0, '', '2016-06-02 22:18:45'),
(639, 2, 131, -2147148800, 0, 1, 'AvoidanceAbility', '2016-06-02 22:24:24'),
(640, 111, 0, -2147481600, 1, 1, '', '2017-01-28 12:17:43'),
(641, 111, 1, -2147367936, 1, 1, '', '2017-01-28 12:17:43'),
(642, 111, 2, -2147294208, 0, 1, '', '2017-01-28 12:17:44'),
(643, 111, 3, -2147281920, 0, 0, '', '2017-01-28 12:17:44'),
(644, 111, 4, -2147280896, 0, 0, '', '2017-01-28 12:17:44'),
(645, 111, 5, -2147437568, 1, 1, '', '2017-01-28 12:17:45'),
(646, 111, 6, -2147425280, 0, 0, '', '2017-01-28 12:17:45'),
(647, 111, 7, -2147404800, 0, 0, '', '2017-01-28 12:17:45'),
(648, 111, 8, -2147445760, 0, 0, '', '2017-01-28 12:17:46'),
(649, 111, 9, -2146493440, 0, 0, '', '2017-01-28 12:17:46'),
(650, 111, 10, -2146453504, 1, 0, '', '2017-01-28 12:17:46'),
(967, 141, 0, -2147481600, 1, 1, '', '2017-10-15 13:43:51'),
(968, 141, 1, -2147367936, 1, 1, '', '2017-10-15 13:43:52'),
(969, 141, 2, -2147294208, 0, 1, '', '2017-10-15 13:43:52'),
(970, 141, 3, -2147281920, 0, 1, '', '2017-10-15 13:43:52'),
(971, 141, 4, -2147280896, 0, 1, '', '2017-10-15 13:43:53'),
(972, 141, 5, -2147437568, 1, 1, '', '2017-10-15 13:43:53'),
(973, 141, 6, -2147425280, 0, 1, '', '2017-10-15 13:43:53'),
(974, 141, 7, -2147404800, 0, 1, '', '2017-10-15 13:43:54'),
(975, 141, 8, -2147445760, 0, 1, '', '2017-10-15 13:43:54'),
(976, 141, 9, -2146493440, 0, 1, '', '2017-10-15 13:43:54'),
(977, 141, 10, -2146453504, 1, 1, '', '2017-10-15 13:43:55'),
(978, 141, 11, -2147472384, 10, 1, '', '2017-10-15 13:43:55'),
(979, 141, 12, -2147295232, 10, 1, '', '2017-10-15 13:43:55'),
(980, 141, 13, -2147295232, 10, 1, '', '2017-10-15 13:43:56'),
(981, 142, 0, -2147481600, 1, 1, '', '2020-04-12 12:48:10'),
(982, 142, 1, -2147367936, 1, 1, '', '2020-04-12 12:48:10'),
(983, 142, 2, -2147294208, 0, 1, '', '2020-04-12 12:48:10'),
(984, 142, 3, -2147281920, 0, 0, '', '2020-04-12 12:48:10'),
(985, 142, 4, -2147280896, 0, 0, '', '2020-04-12 12:48:11'),
(986, 142, 5, -2147437568, 1, 1, '', '2020-04-12 12:48:11'),
(987, 142, 6, -2147425280, 0, 1, '', '2020-04-12 12:48:11'),
(988, 142, 7, -2147404800, 0, 1, '', '2020-04-12 12:48:12'),
(989, 142, 8, -2147445760, 0, 0, '', '2020-04-12 12:48:12'),
(990, 142, 9, -2146493440, 0, 1, '', '2020-04-12 12:48:12'),
(991, 142, 10, -2146453504, 1, 1, '', '2020-04-12 12:48:12'),
(992, 142, 11, -2147472384, 20, 1, '', '2020-04-12 12:48:13'),
(993, 142, 12, -2147295232, 20, 1, '', '2020-04-12 12:48:13'),
(994, 142, 13, -2147295232, 20, 1, '', '2020-04-12 12:48:13'),
(995, 143, 0, -2147481600, 1, 1, '', '2020-05-19 09:58:09'),
(996, 143, 1, -2147367936, 1, 1, '', '2020-05-19 09:58:09'),
(997, 143, 2, -2147294208, 0, 1, '', '2020-05-19 09:58:09'),
(998, 143, 3, -2147281920, 0, 0, '', '2020-05-19 09:58:09'),
(999, 143, 4, -2147280896, 0, 0, '', '2020-05-19 09:58:09'),
(1000, 143, 5, -2147437568, 1, 1, '', '2020-05-19 09:58:09'),
(1001, 143, 6, -2147425280, 0, 0, '', '2020-05-19 09:58:09'),
(1002, 143, 7, -2147404800, 0, 0, '', '2020-05-19 09:58:09'),
(1003, 143, 8, -2147445760, 0, 0, '', '2020-05-19 09:58:09'),
(1004, 143, 9, -2146493440, 0, 0, '', '2020-05-19 09:58:09'),
(1005, 143, 10, -2146453504, 1, 0, '', '2020-05-19 09:58:09'),
(1006, 143, 11, -2147472384, 20, 1, '', '2020-05-19 09:58:09'),
(1007, 143, 12, -2147295232, 20, 1, '', '2020-05-19 09:58:09'),
(1008, 143, 13, -2147295232, 20, 1, '', '2020-05-19 09:58:09'),
(1009, 145, 0, -2147481600, 1, 1, '', '2020-05-19 10:01:06'),
(1010, 145, 1, -2147367936, 1, 1, '', '2020-05-19 10:01:06'),
(1011, 145, 2, -2147294208, 0, 1, '', '2020-05-19 10:01:06'),
(1012, 145, 3, -2147281920, 0, 0, '', '2020-05-19 10:01:06'),
(1013, 145, 4, -2147280896, 0, 0, '', '2020-05-19 10:01:06'),
(1014, 145, 5, -2147437568, 1, 1, '', '2020-05-19 10:01:06'),
(1015, 145, 6, -2147425280, 0, 0, '', '2020-05-19 10:01:06'),
(1016, 145, 7, -2147404800, 0, 0, '', '2020-05-19 10:01:06'),
(1017, 145, 8, -2147445760, 0, 0, '', '2020-05-19 10:01:06'),
(1018, 145, 9, -2146493440, 0, 0, '', '2020-05-19 10:01:06'),
(1019, 145, 10, -2146453504, 1, 0, '', '2020-05-19 10:01:06'),
(1020, 145, 11, -2147472384, 20, 1, '', '2020-05-19 10:01:06'),
(1021, 145, 12, -2147295232, 20, 1, '', '2020-05-19 10:01:06'),
(1022, 145, 13, -2147295232, 20, 1, '', '2020-05-19 10:01:06');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `char_hardlines`
--

CREATE TABLE `char_hardlines` (
  `id` int(10) UNSIGNED NOT NULL,
  `char_id` int(10) UNSIGNED NOT NULL,
  `hl_id` int(10) UNSIGNED NOT NULL,
  `district_id` int(10) UNSIGNED NOT NULL,
  `added` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `crews`
--

CREATE TABLE `crews` (
  `id` int(11) NOT NULL,
  `crew_name` varchar(256) DEFAULT '0',
  `master_player_handle` varchar(256) DEFAULT '',
  `money` int(10) UNSIGNED NOT NULL,
  `org` int(11) NOT NULL,
  `faction_id` int(11) NOT NULL DEFAULT '0',
  `faction_rank` int(11) NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `crews`
--

INSERT INTO `crews` (`id`, `crew_name`, `master_player_handle`, `money`, `org`, `faction_id`, `faction_rank`, `created_at`, `deleted_at`) VALUES
(1, 'Reservoir Dogs', 'TheNeo', 1234, 0, 125, 1, '2017-06-09 05:30:50', '0000-00-00 00:00:00'),
(2, 'Hardline Dreams', 'LaurinHill', 50, 0, 125, 2, '2020-05-15 11:50:08', NULL),
(3, 'zion tac ops', 'entilsar', 333, 1, 125, 3, '2020-05-19 10:01:53', NULL);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `crew_members`
--

CREATE TABLE `crew_members` (
  `id` int(11) NOT NULL,
  `crew_id` int(11) NOT NULL DEFAULT '0',
  `char_id` int(11) NOT NULL DEFAULT '0',
  `is_captain` int(11) NOT NULL DEFAULT '0',
  `is_first_mate` int(11) NOT NULL,
  `created_at` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `crew_members`
--

INSERT INTO `crew_members` (`id`, `crew_id`, `char_id`, `is_captain`, `is_first_mate`, `created_at`) VALUES
(1, 1, 2, 1, 0, '2017-06-09 05:32:49'),
(2, 1, 142, 0, 1, '2017-06-09 05:32:55'),
(3, 1, 110, 0, 0, '2020-05-14 12:40:31'),
(4, 2, 111, 1, 0, '2020-05-19 07:09:21'),
(5, 3, 143, 1, 0, '2020-05-19 10:02:50'),
(6, 3, 145, 0, 1, '2020-05-19 10:02:50');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `data_hardlines`
--

CREATE TABLE `data_hardlines` (
  `Id` int(11) UNSIGNED NOT NULL,
  `HardLineId` smallint(6) UNSIGNED NOT NULL,
  `objectId` bigint(20) NOT NULL,
  `HardlineName` varchar(45) NOT NULL,
  `X` double NOT NULL,
  `Y` double NOT NULL,
  `Z` double NOT NULL,
  `ROT` double NOT NULL,
  `DistrictId` smallint(6) UNSIGNED NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `data_hardlines`
--

INSERT INTO `data_hardlines` (`Id`, `HardLineId`, `objectId`, `HardlineName`, `X`, `Y`, `Z`, `ROT`, `DistrictId`) VALUES
(2, 49, 0, 'MaraNorthWest', 7737.37, 95, 13801.5, 1.5708, 1),
(3, 152, 1039139603, 'MaraCentral', 17043.1, 495, 2398.8, -3.14159, 1),
(6, 72, 0, 'Tagged By Tastee Wheat', 39216.4, 95, -21475.1, 2.03713, 1),
(7, 125, 706741522, 'Tagged By Tastee Wheat', 25640, -515, -35813, 0.294524, 1),
(8, 73, 0, 'Tagged By Tastee Wheat', 13211.7, 95, -37821.4, -2.72435, 2),
(9, 98, 0, 'Tagged By Tastee Wheat', 9844.79, 1295, 1314.42, -2.42983, 2),
(10, 48, 273679824, 'Tagged By chefmaster', -6415, 95, -7121.87, 1.93895, 1),
(11, 146, 0, 'Tagged By Tastee Wheat', 52941.3, 495, 41981.6, -1.52171, 1),
(12, 113, 0, 'Tagged By TheThirdMan', -133308, 1295, -35477, 1.44808, 2),
(13, 129, 1213204213, 'Tagged By TheThirdMan', -2683.96, -705, 31556.3, 2.82252, 1),
(14, 120, 45090940, 'Tagged By IntegratedOne', 70083.9, 95, 44597.9, -0.269981, 1),
(15, 116, 423624714, 'Tagged By chefmaster', -492.907, 95, -31590.9, 0.343612, 1),
(16, 128, 0, 'Tagged By Midnight', 54965.4, 95, 15087.4, -0.0736311, 1),
(17, 78, 802160652, 'Tagged By CloudWolf', -49790.2, 95, -159434, -1.22718, 1),
(18, 147, 211813903, 'Tagged By SanMarco', 111007, -505, -59842.7, 1.25173, 1),
(19, 136, 0, 'Tagged By SanMarco', 111180, 95, -40913.8, 1.7426, 1),
(20, 114, 263194005, 'Tagged By SanMarco', 77349.4, 694.999, -43966.8, 0.368155, 1),
(21, 55, 0, 'Tagged By SanMarco', 82745.4, 694.999, -66760.6, -1.64443, 1),
(22, 130, 0, 'Tagged By Agent', 51820.8, 95, -42922, -1.00629, 1),
(23, 138, 0, 'Tagged By Superman111994', -30111, 95, -48204.8, 0, 1),
(24, 124, 1209008135, 'Tagged By SanMarco', 39584.6, 95, -80338.7, 2.57709, 1),
(25, 107, 0, 'Tagged By SanMarco', 63391.2, 95, -91925.9, 0.220893, 1),
(26, 151, 0, 'Tagged By Agent', 67566.8, -105, -10164.6, -2.30711, 1),
(27, 132, 326108784, 'Tagged By SanMarco', 96190.5, 95, -85320.7, 3.01887, 1),
(28, 115, 0, 'Tagged By SanMarco', 15657.9, 495, -70054.5, 1.93895, 1),
(29, 112, 0, 'Tagged By SanMarco', 40267.9, 95, -116994, -0.859029, 1),
(30, 104, 0, 'Tagged By IntegratedOne', -17757.8, 95, -141682, 0.981748, 1),
(31, 122, 1245709271, 'Tagged By SanMarco', 10377, 95, -110304, 2.67526, 1),
(32, 145, 0, 'Tagged By SanMarco', 55512.5, 85, -166913, -1.20264, 1),
(33, 149, 0, 'Tagged By SanMarco', 14659.5, 95, -143258, 2.35619, 1),
(34, 113, 0, 'Tagged By SanMarco', 107619, -505, -149092, -0.0490874, 1),
(35, 148, 0, 'Tagged By SanMarco', 79289.9, -505, -148965, 1.79169, 1),
(36, 111, 0, 'Tagged By SanMarco', 87635.3, 85, -117864, 1.42353, 1),
(37, 119, 0, 'Tagged By Cookie', -36326, 95, -23953.1, 1.86532, 1),
(38, 109, 0, 'Tagged By SanMarco', 135175, 95, -103708, -0.93266, 1),
(39, 76, 0, 'Tagged By Cookie', -67862.5, 95, 16314.2, 0.269981, 1),
(40, 150, 0, 'Tagged By SanMarco', 111671, -505, -99321.3, 1.44808, 1),
(41, 153, 0, 'Tagged By SanMarco', 12184.8, -705, 59766, -1.61988, 1),
(42, 131, 0, 'Tagged By SanMarco', 100459, 95, 11859, 1.49717, 1),
(43, 142, 3158258, 'Tagged By SanMarco', 94166.5, 95, 26721.2, -2.40528, 1),
(44, 121, 831520820, 'Tagged By SanMarco', 83610.7, 95, 57664.3, 2.99433, 1),
(45, 99, 53478188, 'Tagged By Cookie', -30434, -705, 20325.5, -0.981748, 1),
(46, 75, 1331694835, 'Tagged By Agent', -64467.5, -1105, -57980.9, 3.01887, 1),
(47, 105, 0, 'Tagged By Cookie', -92910.8, -705, 40713.9, 1.76715, 1),
(48, 101, 506462220, 'Tagged By Superman111994', -70778.6, 95, -140154, 3.09251, 1),
(49, 106, 1181747457, 'Tagged By Agent', -18499.8, 95, -89853.4, -0.490874, 1),
(50, 133, 648023732, 'Tagged By Forez', 85775, 694.999, -14502.5, -1.81623, 1),
(51, 144, 1039139603, 'Tagged By Forez', 115650, 694.999, 4825, -2.82252, 1),
(52, 12, 0, 'Tagged By Othinn', 11039.9, 95, 35967.4, 0.0490874, 3),
(53, 10, 0, 'Tagged By Othinn', 18310, -505, 15226.4, 0.809942, 3),
(54, 102, 0, 'Tagged By jayce77', -86023.7, 95, -77894.1, 1.81623, 1),
(55, 82, 0, 'Tagged By Cyberkat', 14806.5, 1895, 72657.9, 1.52171, 2),
(56, 109, 0, 'Tagged By Cyberkat', 3936.42, 1295, 96578.1, 1.5708, 2),
(57, 31, 787488747, 'Tagged By Cyberkat', -10586.3, 1895, 58667, 0.760854, 2),
(58, 105, 0, 'Tagged By Cyberkat', -19612.5, 694.999, 74764.7, -1.81623, 2),
(59, 94, 0, 'Tagged By Cyberkat', -15563.7, 694.999, 95005.9, 2.9207, 2),
(60, 75, 0, 'Tagged By Cyberkat', -7704.53, 1895, 84346.2, -0.981748, 2),
(61, 38, 482353886, 'Tagged By Cyberkat', -7466.52, 1895, 17801.8, -0.0981748, 2),
(62, 56, 0, 'Tagged By Cyberkat', 12047.9, 694.999, 17764.6, -2.87161, 2),
(63, 92, 0, 'Tagged By Cyberkat', -23801.4, 1895, 12300.4, 1.76715, 2),
(64, 37, 0, 'Tagged By Cyberkat', -20669.7, 1895, -19128, -0.245437, 2),
(65, 19, 407897936, 'Tagged By Agent', -14261.6, 95, 9710.15, -1.64443, 3),
(66, 14, 0, 'Tagged By Agent', -31420.5, 95, 8083.13, -0.147262, 3),
(67, 15, 0, 'Tagged By Agent', -42980.2, 95, 9352.13, 1.84078, 3),
(68, 23, 0, 'Tagged By Agent', -65336.5, 95, 32209.4, 0.490874, 3),
(69, 17, 0, 'Tagged By Agent', 36481.5, -1105, -30745, -1.91441, 3),
(70, 7, 0, 'Tagged By Agent', 42779.3, -1105, -5699.44, -3.11705, 3),
(71, 20, 174064055, 'Tagged By Agent', 79634.9, -1115, 2825.15, -2.79798, 3),
(72, 24, 0, 'Tagged By Agent', 19783.5, -505, -11294.7, -3.11705, 3),
(73, 6, 0, 'Tagged By Agent', 15340.4, 95, -32048.6, -1.1781, 3),
(74, 16, 0, 'Tagged By Agent', -1445.26, 95, -41080.3, 1.12901, 3),
(75, 5, 228589685, 'Tagged By Agent', -16244.9, 95, -19746.3, 1.79169, 3),
(76, 11, 0, 'Tagged By Agent', -9540.12, 95, -8841.22, 1.59534, 3),
(77, 13, 291504339, 'Tagged By Agent', -40011.6, 95, -19769.5, -2.84707, 3),
(78, 9, 37751227, 'Tagged By Agent', -48538.8, 95, -31991.9, 0.0245437, 3),
(79, 21, 400564319, 'Tagged By Agent', -33794.6, 95, -71125, -0.368155, 3),
(80, 22, 361759850, 'Tagged By Agent', 47043.9, -1105, -53683.6, 1.20264, 3),
(81, 18, 0, 'Tagged By Agent', 24972, -505, 33362.3, -2.62618, 3),
(82, 3, 0, 'Tagged By Agent', 42116.9, -105, 57953, 2.94524, 3),
(83, 4, 364906923, 'Tagged By Agent', 31450.7, -505, 15775, 0.0736311, 3),
(84, 2, 0, 'Tagged By Agent', -37444.4, 95, 23659.1, 2.23348, 3),
(85, 8, 0, 'Tagged By Agent', -22193.8, 95, 32347.7, 3.04342, 3),
(86, 74, 0, 'Tagged By Flerba', 42659.2, 95, -2943.5, 0.908117, 2),
(87, 97, 0, 'Tagged By SanMarco', -148758, 694.999, -49511.3, -0.785398, 2),
(88, 78, 0, 'Tagged By SanMarco', -127692, 95, -77862.7, 0.319068, 2),
(89, 101, 0, 'Tagged By SanMarco', -118336, 895, -57827, -1.42353, 2),
(90, 84, 0, 'Tagged By SanMarco', -161106, 95, -78546.1, 0.539961, 2),
(91, 102, 0, 'Tagged By SanMarco', -151386, -705, -105035, 3.04342, 2),
(92, 89, 0, 'Tagged By SanMarco', -102034, -705, -104025, -2.30711, 2),
(93, 79, 0, 'Tagged By SanMarco', -197691, -705, -106110, 2.23348, 2),
(94, 95, 0, 'Tagged By SanMarco', -213404, 95, -83702.2, 2.45437, 2),
(95, 77, 0, 'Tagged By SanMarco', -101645, 1295, -37998.6, -3.06796, 2),
(96, 87, 0, 'Tagged By SanMarco', -92388.8, 295, -52656.1, -1.9635, 2),
(97, 108, 0, 'Tagged By SanMarco', -84171.4, 95, -69825, -0.834486, 2),
(98, 91, 182462326, 'Tagged By SanMarco', -51335.2, 95, -26132.8, -0.711767, 2),
(99, 96, 0, 'Tagged By SanMarco', -33636.6, 1895, -45757.7, 0.294524, 2),
(100, 81, 0, 'Tagged By SanMarco', -2610.38, 1295, -32441.3, -1.27627, 2),
(101, 99, 0, 'Tagged By SanMarco', -38914.8, 95, -58865.1, 0.46633, 2),
(102, 49, 0, 'Tagged By SanMarco', 2945.95, 95, -87486.3, -1.93895, 2),
(103, 110, 0, 'Tagged By SanMarco', 13888.2, 95, -87848.4, -1.88986, 2),
(104, 90, 0, 'Tagged By SanMarco', 42918.6, 95, -82035.5, -0.687223, 2),
(105, 93, 0, 'Tagged By SanMarco', 13797.4, 95, -62540.9, -0.589049, 2),
(106, 39, 0, 'Tagged By SanMarco', 30494.8, 95, -50434.3, -2.01258, 2),
(107, 100, 0, 'Tagged By SanMarco', 28299.5, 694.999, -23233.1, -2.06167, 2),
(108, 111, 0, 'Tagged By SanMarco', 26340.3, 694.999, -9418.3, -2.9207, 2),
(109, 35, 0, 'Tagged By SanMarco', 34943.9, 95, 9724.07, 0.908117, 2),
(110, 36, 0, 'Tagged By SanMarco', 24184.7, 694.999, 3438.79, -0.809942, 2),
(111, 48, 0, 'Tagged By SanMarco', -43576.8, 1295, -236.303, 2.38074, 2),
(112, 63, 0, 'Tagged By SanMarco', -67676.3, 694.999, -13554.3, 3.06796, 2),
(113, 59, 0, 'Tagged By SanMarco', -91392.6, 1295, -13747.1, 0.785398, 2),
(114, 88, 0, 'Tagged By SanMarco', -77737.1, 1295, -3403.01, -1.44808, 2),
(115, 52, 0, 'Tagged By SanMarco', -62448, 1295, 15867.9, 2.20893, 2),
(116, 107, 0, 'Tagged By SanMarco', -57354.7, 1295, 33825, -2.69981, 2),
(117, 50, 0, 'Tagged By SanMarco', -71709.4, 1295, 33189, -0.613592, 2),
(118, 64, 0, 'Tagged By SanMarco', -70694, 1295, 40108, -1.47262, 2),
(119, 85, 628109112, 'Tagged By SanMarco', -89310.2, 1295, 29097.4, 1.59534, 2),
(120, 80, 0, 'Tagged By SanMarco', -115777, 1295, 12425, 2.99433, 2),
(121, 112, 0, 'Tagged By SanMarco', -132482, 1295, 15399.5, -3.06796, 2),
(122, 69, 0, 'Tagged By SanMarco', -145953, 694.999, 27883.6, -3.09251, 2),
(123, 55, 0, 'Tagged By SanMarco', -142873, 694.999, 42221.6, -1.42353, 2),
(124, 54, 0, 'Tagged By SanMarco', -127479, 694.999, 42677, -1.98804, 2),
(125, 67, 0, 'Tagged By SanMarco', -140880, 694.999, 69638.8, 1.27627, 2),
(126, 47, 0, 'Tagged By SanMarco', -84175.4, 1295, 78182.1, -2.72435, 2),
(127, 53, 0, 'Tagged By SanMarco', -44698.3, 694.999, 96080.3, 2.84707, 2),
(128, 86, 0, 'Tagged By SanMarco', -46627.5, 1895, 69787.3, -1.54625, 2),
(129, 41, 0, 'Tagged By SanMarco', 22764.8, 694.999, 42678.9, -0.589049, 2),
(130, 106, 0, 'Tagged By SanMarco', 22895.7, 694.999, 27604.9, -1.20264, 2),
(131, 81, 0, 'Tagged By AtraBile', -38843.9, 95, -1082.27, 1.15355, 1),
(132, 82, 724568318, 'Tagged By Cookie', -55135.7, 95, -110187, 0.93266, 1),
(133, 100, 195036127, 'Tagged By Maverick', -75253.7, 95, -7823.79, -1.66897, 1),
(134, 103, 0, 'Tagged By Tyndall', -73303.1, 2, -30083.3, 2.38074, 1),
(135, 80, 463470596, 'Tagged By Tyndall', -84633, 95, 8015.88, -0.0245437, 1),
(136, 79, 0, 'Tagged By Tyndall', -94295.7, -505, -33925, 2.82252, 1),
(137, 43, 0, 'Tagged By Tyndall', -19925, 1895, 34694.1, -1.88986, 2),
(138, 103, 0, 'Tagged By megabeans', -33575, 1895, 41507.1, -1.64443, 2),
(139, 1, 0, 'medium01', 391, -9, 269, -0.613592, 14),
(140, 1, 0, 'large01', 130, -12, -71, -0.613592, 12),
(141, 1, 0, 'small03', 177, -4, 341, -0.613592, 17),
(142, 1, 0, 'medium02', -9, 1, 26, -0.613592, 15),
(143, 2, 0, 'medium03', 377, 5, 317, -0.613592, 16),
(144, 1, 0, 'large02', -74, 1, -19, -0.613592, 13);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `districts`
--

CREATE TABLE `districts` (
  `id` int(11) NOT NULL,
  `key` text NOT NULL,
  `path` text NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `districts`
--

INSERT INTO `districts` (`id`, `key`, `path`) VALUES
(1, 'slums', 'resource/worlds/final_world/slums_barrens_full.metr'),
(2, 'downtown', 'resource/worlds/final_world/downtown/dt_world.metr'),
(3, 'international', 'resource/worlds/final_world/international/it.metr'),
(4, 'tutorial', 'resource/worlds/final_world/tutorial_v2/tutorial_v2.metr'),
(5, 'la', 'resource/worlds/loading_area/la.metr'),
(6, 'ccbackdrop', 'resource/worlds/ccbackdrop/ccbackdrop.metr'),
(7, 'archive01', 'resource/worlds/final_world/constructs/archive/archive01/archive01.metr'),
(8, 'archive02', 'resource/worlds/final_world/constructs/archive/archive02/archive02.metr'),
(9, 'archive_ashencourte', 'resource/worlds/final_world/constructs/archive/archive_ashencourte/archive_ashencourte.metr'),
(10, 'archive_datamine', 'resource/worlds/final_world/constructs/archive/archive_datamine/datamine.metr'),
(11, 'archive_sakura', 'resource/worlds/final_world/constructs/archive/archive_sakura/archive_sakura.metr'),
(12, 'archive_sati', 'resource/worlds/final_world/constructs/archive/archive_sati/sati.metr'),
(13, 'archive_widowsmoor', 'resource/worlds/final_world/constructs/archive/archive_widowsmoor/archive_widowsmoor.metr'),
(14, 'archive_yuki', 'resource/worlds/final_world/constructs/archive/archive_yuki/archive_yuki.metr'),
(15, 'large01', 'resource/worlds/final_world/constructs/large/large01/large01.metr'),
(16, 'large02', 'resource/worlds/final_world/constructs/large/large02/large02.metr'),
(17, 'medium01', 'resource/worlds/final_world/constructs/medium/medium01/medium01.metr'),
(18, 'medium02', 'resource/worlds/final_world/constructs/medium/medium02/medium02.metr'),
(19, 'medium03', 'resource/worlds/final_world/constructs/medium/medium03/medium03.metr'),
(20, 'small01', 'resource/worlds/final_world/constructs/small/small01/small01.metr'),
(21, 'small02', 'resource/worlds/final_world/constructs/small/small02/small02.metr'),
(22, 'small03', 'resource/worlds/final_world/constructs/small/small03/small03.metr'),
(24, 'urban01', 'resource/worlds/worldtest/constructs/urban01/urban01.metr'),
(25, 'urban02', 'resource/worlds/worldtest/constructs/urban02/urban02.metr'),
(26, 'urban03', 'resource/worlds/worldtest/constructs/urban03/urban03.metr');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `factions`
--

CREATE TABLE `factions` (
  `id` int(11) NOT NULL,
  `name` varchar(256) DEFAULT '0',
  `master_player_handle` varchar(256) DEFAULT '0',
  `money` int(11) NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `factions`
--

INSERT INTO `factions` (`id`, `name`, `master_player_handle`, `money`, `created_at`, `deleted_at`) VALUES
(125, 'The Duality Returns', 'TheNeo', 0, '2020-05-15 00:00:00', NULL);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `inventory`
--

CREATE TABLE `inventory` (
  `invId` int(11) UNSIGNED NOT NULL,
  `charId` bigint(11) UNSIGNED NOT NULL,
  `goid` bigint(11) NOT NULL,
  `slot` tinyint(11) UNSIGNED NOT NULL,
  `count` int(11) NOT NULL DEFAULT '0',
  `purity` int(11) NOT NULL DEFAULT '0',
  `updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `inventory`
--

INSERT INTO `inventory` (`invId`, `charId`, `goid`, `slot`, `count`, `purity`, `updated`, `created`) VALUES
(1, 3, 7332, 0, 0, 0, '2012-01-11 06:49:44', '2012-01-11 06:49:44'),
(4, 3, 7332, 0, 1, 1, '2012-01-30 09:58:00', '2012-01-30 09:58:00'),
(27, 3, 2147484672, 4, 0, 0, '0000-00-00 00:00:00', '2012-01-31 12:14:12'),
(26, 3, 2147490816, 1, 0, 0, '0000-00-00 00:00:00', '2012-01-31 12:13:56'),
(28, 3, 2147492864, 5, 0, 0, '0000-00-00 00:00:00', '2012-01-31 12:14:14'),
(29, 3, 2147494912, 3, 0, 0, '0000-00-00 00:00:00', '2012-01-31 12:14:16'),
(30, 3, 2147499008, 6, 0, 0, '0000-00-00 00:00:00', '2012-01-31 12:14:21'),
(31, 3, 2148660224, 1, 0, 0, '0000-00-00 00:00:00', '2012-01-31 12:14:24'),
(32, 3, 2147492864, 17, 0, 0, '0000-00-00 00:00:00', '2012-01-31 14:46:01'),
(33, 3, 2147492864, 7, 0, 0, '0000-00-00 00:00:00', '2012-01-31 14:46:05'),
(34, 3, 2147492864, 18, 0, 0, '0000-00-00 00:00:00', '2012-01-31 14:46:06'),
(35, 3, 2147492864, 8, 0, 0, '0000-00-00 00:00:00', '2012-01-31 14:46:07'),
(36, 3, 2147492864, 19, 0, 0, '0000-00-00 00:00:00', '2012-01-31 14:46:07'),
(37, 3, 2147492864, 9, 0, 0, '0000-00-00 00:00:00', '2012-01-31 14:46:08'),
(38, 3, 2148375552, 20, 0, 0, '0000-00-00 00:00:00', '2012-01-31 14:46:35'),
(39, 3, 2147490816, 10, 0, 0, '0000-00-00 00:00:00', '2012-01-31 15:48:32'),
(40, 3, 2147490816, 21, 0, 0, '0000-00-00 00:00:00', '2012-02-02 16:04:18'),
(41, 3, 2147490816, 11, 0, 0, '0000-00-00 00:00:00', '2012-02-02 16:04:22'),
(42, 3, 2147490816, 22, 0, 0, '0000-00-00 00:00:00', '2012-09-04 16:19:15'),
(43, 3, 2147490816, 12, 0, 0, '0000-00-00 00:00:00', '2012-09-04 16:19:16'),
(44, 3, 2147490816, 23, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:33:42'),
(45, 3, 2147490816, 13, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:33:45'),
(46, 3, 2147491840, 24, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:33:46'),
(47, 3, 2147490816, 14, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:34:21'),
(48, 3, 2147490816, 25, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:34:23'),
(49, 3, 2147490816, 15, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:34:23'),
(50, 3, 2147490816, 26, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:34:24'),
(51, 3, 2147490816, 16, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:34:24'),
(52, 3, 2147490816, 27, 0, 0, '0000-00-00 00:00:00', '2013-07-25 14:34:24'),
(53, 3, 2147490816, 28, 0, 0, '0000-00-00 00:00:00', '2013-07-25 15:07:52'),
(54, 3, 2147491840, 29, 0, 0, '0000-00-00 00:00:00', '2013-07-25 15:07:55'),
(55, 3, 2147490816, 30, 0, 0, '0000-00-00 00:00:00', '2013-07-25 15:10:36'),
(56, 3, 2147490816, 31, 0, 0, '0000-00-00 00:00:00', '2013-07-25 15:10:41'),
(57, 3, 2147490816, 32, 0, 0, '0000-00-00 00:00:00', '2013-08-09 09:14:07'),
(58, 3, 2147491840, 33, 0, 0, '0000-00-00 00:00:00', '2013-08-09 09:14:12'),
(59, 3, 2147492864, 35, 0, 0, '0000-00-00 00:00:00', '2013-08-09 09:14:14'),
(60, 3, 2148660224, 34, 0, 0, '0000-00-00 00:00:00', '2013-08-09 09:14:17'),
(61, 3, 2147490816, 2, 0, 0, '0000-00-00 00:00:00', '2014-10-06 16:00:00'),
(62, 3, 2147491840, 36, 0, 0, '0000-00-00 00:00:00', '2014-10-06 16:00:04'),
(63, 3, 2147492864, 37, 0, 0, '0000-00-00 00:00:00', '2014-10-06 16:00:12'),
(64, 3, 2147490816, 38, 0, 0, '0000-00-00 00:00:00', '2015-06-27 12:05:06'),
(65, 3, 2147490816, 39, 0, 0, '0000-00-00 00:00:00', '2015-06-27 12:05:11'),
(66, 3, 2147490816, 40, 0, 0, '0000-00-00 00:00:00', '2015-06-27 16:30:03'),
(67, 3, 2147490816, 41, 0, 0, '0000-00-00 00:00:00', '2015-06-27 16:30:09'),
(68, 3, 2147490816, 42, 0, 0, '0000-00-00 00:00:00', '2015-06-27 16:30:10'),
(69, 3, 2147490816, 43, 0, 0, '0000-00-00 00:00:00', '2015-06-27 16:30:11'),
(70, 3, 2147488768, 44, 0, 0, '0000-00-00 00:00:00', '2015-06-27 16:30:14'),
(71, 3, 2147492864, 45, 0, 0, '0000-00-00 00:00:00', '2015-06-27 16:30:19'),
(72, 3, 2147490816, 46, 0, 0, '0000-00-00 00:00:00', '2015-09-03 06:47:34'),
(73, 3, 2147490816, 47, 0, 0, '0000-00-00 00:00:00', '2015-09-03 06:47:35'),
(74, 3, 2147494912, 48, 0, 0, '0000-00-00 00:00:00', '2015-10-14 21:24:08'),
(75, 3, 2147494912, 49, 0, 0, '0000-00-00 00:00:00', '2015-10-14 21:24:08'),
(76, 3, 2147494912, 50, 0, 0, '0000-00-00 00:00:00', '2015-10-14 21:24:08'),
(77, 3, 2147494912, 51, 0, 0, '0000-00-00 00:00:00', '2015-10-14 21:24:09'),
(78, 3, 2147490816, 52, 0, 0, '0000-00-00 00:00:00', '2015-10-25 19:40:34'),
(79, 110, 2148374528, 0, 0, 0, '0000-00-00 00:00:00', '2016-03-17 21:34:18'),
(80, 110, 2148374528, 1, 0, 0, '0000-00-00 00:00:00', '2016-03-17 21:34:18'),
(81, 2, 2147490816, 7, 0, 0, '0000-00-00 00:00:00', '2016-03-20 23:52:21'),
(82, 2, 2147490816, 1, 0, 0, '0000-00-00 00:00:00', '2016-03-20 23:52:21'),
(83, 2, 2147490816, 2, 0, 0, '0000-00-00 00:00:00', '2016-12-07 22:53:26'),
(84, 2, 39766, 17, 0, 1, '0000-00-00 00:00:00', '2017-03-03 11:35:31'),
(85, 2, 39766, 20, 0, 1, '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(86, 113, 47140, 1, 0, 1, '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(87, 113, 7510, 103, 0, 1, '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(88, 113, 45928, 99, 0, 1, '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(155, 113, 38145, 101, 0, 0, '0000-00-00 00:00:00', '2017-03-10 21:27:44'),
(156, 113, 38146, 100, 0, 0, '0000-00-00 00:00:00', '2017-03-10 21:27:54'),
(97, 113, 38145, 2, 0, 0, '0000-00-00 00:00:00', '2017-03-10 21:22:12'),
(157, 113, 38142, 8, 0, 0, '0000-00-00 00:00:00', '2017-03-10 21:28:25'),
(158, 113, 38145, 3, 0, 0, '0000-00-00 00:00:00', '2017-03-10 23:49:13'),
(159, 113, 38146, 4, 0, 0, '0000-00-00 00:00:00', '2017-03-10 23:49:15'),
(160, 113, 38146, 5, 0, 0, '0000-00-00 00:00:00', '2017-03-10 23:49:16'),
(161, 113, 38146, 6, 0, 0, '0000-00-00 00:00:00', '2017-03-10 23:49:16'),
(162, 113, 38142, 9, 0, 0, '0000-00-00 00:00:00', '2017-03-10 23:49:19'),
(163, 2, 38146, 101, 0, 0, '0000-00-00 00:00:00', '2017-03-11 00:26:59'),
(164, 113, 47952, 13, 0, 1, '0000-00-00 00:00:00', '0000-00-00 00:00:00'),
(165, 2, 38146, 19, 0, 0, '0000-00-00 00:00:00', '2017-04-11 17:07:30'),
(166, 2, 7878, 10, 0, 0, '0000-00-00 00:00:00', '2017-05-11 16:20:49'),
(167, 126, 38145, 0, 0, 0, '2017-09-20 15:12:40', '2017-09-20 15:12:40'),
(170, 141, 671, 98, 0, 0, '2017-10-15 13:43:50', '2017-10-15 13:43:50'),
(171, 141, 842, 99, 0, 0, '2017-10-15 13:43:50', '2017-10-15 13:43:50'),
(172, 141, 5667, 102, 0, 0, '2017-10-15 13:43:50', '2017-10-15 13:43:50'),
(173, 141, 6734, 104, 0, 0, '2017-10-15 13:43:51', '2017-10-15 13:43:51'),
(174, 2, 2147490816, 8, 0, 0, '2017-10-17 16:34:57', '2017-10-17 16:34:57'),
(175, 2, 2147924992, 6, 0, 0, '2017-10-17 16:35:04', '2017-10-17 16:35:04'),
(176, 2, 2148375552, 9, 0, 0, '2017-10-17 16:37:33', '2017-10-17 16:37:33'),
(177, 2, 7873, 0, 0, 0, '2017-10-17 17:22:26', '2017-10-17 17:22:26'),
(178, 2, 7891, 11, 0, 0, '2017-10-17 17:22:42', '2017-10-17 17:22:42'),
(179, 2, 2147490816, 3, 0, 0, '2017-11-27 09:46:58', '2017-11-27 09:46:58'),
(180, 2, 2147855360, 13, 0, 0, '2017-11-27 09:47:05', '2017-11-27 09:47:05'),
(181, 2, 2147484672, 14, 0, 0, '2017-11-27 09:47:18', '2017-11-27 09:47:18'),
(182, 2, 2148660224, 15, 0, 0, '2017-11-27 09:47:41', '2017-11-27 09:47:41'),
(183, 2, 2147490816, 16, 0, 0, '2017-11-27 10:48:53', '2017-11-27 10:48:53'),
(184, 2, 45904, 98, 0, 0, '2017-12-07 11:20:22', '2017-12-07 11:20:22'),
(185, 2, 48171, 97, 0, 0, '2017-12-07 11:20:50', '2017-12-07 11:20:50'),
(186, 2, 2148375552, 5, 0, 0, '2019-02-19 14:16:04', '2019-02-19 14:16:04'),
(187, 142, 987, 98, 0, 0, '2020-04-12 12:48:08', '2020-04-12 12:48:08'),
(188, 142, 1243, 99, 0, 0, '2020-04-12 12:48:08', '2020-04-12 12:48:08'),
(189, 142, 6727, 100, 0, 0, '2020-04-12 12:48:09', '2020-04-12 12:48:09'),
(190, 142, 6386, 102, 0, 0, '2020-04-12 12:48:09', '2020-04-12 12:48:09'),
(191, 142, 1263, 104, 0, 0, '2020-04-12 12:48:09', '2020-04-12 12:48:09'),
(192, 2, 2147490816, 18, 0, 0, '2020-05-01 18:14:35', '2020-05-01 18:14:35'),
(193, 143, 987, 98, 0, 0, '2020-05-19 09:58:09', '2020-05-19 09:58:09'),
(194, 143, 5907, 99, 0, 0, '2020-05-19 09:58:09', '2020-05-19 09:58:09'),
(195, 143, 1101, 102, 0, 0, '2020-05-19 09:58:09', '2020-05-19 09:58:09'),
(196, 143, 1260, 104, 0, 0, '2020-05-19 09:58:09', '2020-05-19 09:58:09'),
(197, 144, 1030, 97, 0, 0, '2020-05-19 09:59:24', '2020-05-19 09:59:24'),
(198, 144, 987, 98, 0, 0, '2020-05-19 09:59:24', '2020-05-19 09:59:24'),
(199, 144, 1243, 99, 0, 0, '2020-05-19 09:59:24', '2020-05-19 09:59:24'),
(200, 144, 1061, 102, 0, 0, '2020-05-19 09:59:24', '2020-05-19 09:59:24'),
(201, 144, 1260, 104, 0, 0, '2020-05-19 09:59:24', '2020-05-19 09:59:24'),
(202, 145, 1030, 97, 0, 0, '2020-05-19 10:01:06', '2020-05-19 10:01:06'),
(203, 145, 987, 98, 0, 0, '2020-05-19 10:01:06', '2020-05-19 10:01:06'),
(204, 145, 1243, 99, 0, 0, '2020-05-19 10:01:06', '2020-05-19 10:01:06'),
(205, 145, 6727, 100, 0, 0, '2020-05-19 10:01:06', '2020-05-19 10:01:06'),
(206, 145, 1061, 102, 0, 0, '2020-05-19 10:01:06', '2020-05-19 10:01:06'),
(207, 145, 1260, 104, 0, 0, '2020-05-19 10:01:06', '2020-05-19 10:01:06');

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `locations`
--

CREATE TABLE `locations` (
  `Id` int(10) UNSIGNED NOT NULL,
  `Command` varchar(45) NOT NULL,
  `X` double NOT NULL,
  `Y` double NOT NULL,
  `Z` double NOT NULL,
  `District` tinyint(3) UNSIGNED NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `locations`
--

INSERT INTO `locations` (`Id`, `Command`, `X`, `Y`, `Z`, `District`) VALUES
(17, 'Whiteroom', 203183, 95, -172722, 1),
(18, 'Hallways', 211015, -105, -162254, 1),
(19, 'MaraCentral', 16802.3, 495, 3237.01, 1),
(20, 'VaultClub', 29966.4, 22295, -3233.15, 2),
(21, 'Whiteroom2', -58679.7, 95, 135827, 2),
(22, 'Whiteroom1', -58418.9, 95, 139168, 2),
(23, 'LargeHallways1', 101672, 95, 99688.9, 2),
(24, 'LargeHallways2', 101600, 100, 126000, 2),
(25, 'PolyVinyl', 28464.5, -705, 2111.1, 1),
(26, 'Neo', -91572.5, -705, -164673, 1),
(27, 'CamonChurch', 111091, -505, -48948.7, 1),
(28, 'SanguineClub', 106321, 694.999, -16772.9, 1),
(29, 'TaborPlaza', 48497.2, 495, 42309.5, 1),
(30, 'TaborTower', 51605.4, 7495, 53883, 1),
(31, 'KaltChemical', 81943, 95, -107681, 1),
(32, 'DaemonClub', -20607.7, 95, -123654, 1),
(33, 'Caves', -61657, -3908.18, -32561.3, 1),
(34, 'CentralPower', -33059.7, 95, -98468.7, 1),
(35, 'Probability', -85534.3, 109.712, -109513, 1),
(36, 'Ascension', -49979.7, 2495, -71401.1, 1),
(37, 'LinchpinClub', -25615.2, 95, -30282.1, 1),
(38, 'Mjolnir', 12105.5, 95, -32503.2, 1),
(39, 'Highschool', 42018.5, 495, -130268, 1),
(40, 'Majesty', 33480.9, 91.4854, -146214, 1),
(41, 'ZalmonCasino', 104162, -505, -175606, 1),
(42, 'HammersfieldCourts', 130386, -605, -140205, 1),
(43, 'AzimuthTwin', 133007, -108.184, -98555.5, 1),
(44, 'AvalonClub', 125652, 95, -116325, 1),
(45, 'Beryl', 140635, 3295, -109089, 1),
(46, 'Office1', 5033.63, 495, 15395.4, 0),
(47, 'ExtractionRoom1', 9246.28, 295, 15059.3, 0),
(48, 'Subway1', 6846.27, 295, 21113.5, 0),
(49, 'ReadChairWhiteRoom1', 7492.12, 3995, -15671, 0),
(50, 'Hypersphere', -60925.3, 95, 12865.7, 1),
(51, 'HeartHotel', -72165.7, 95, 5912.79, 1),
(52, 'Metacortex', -124095, 1295, -19459.8, 2),
(53, 'GovtBuilding', 68406.9, 1313, 27064.9, 2),
(54, 'TheLobby', -67381.9, 1295, 26559.9, 2),
(55, 'ClubHel1', -30507.6, -14505, -6885.32, 2),
(56, 'ClubHelBalcony1', -27307.2, -12905, -563.901, 2),
(57, 'SphinxClub', 25676.4, 495, -105156, 1),
(58, 'MaraNWHL', 7673.72, 95, 13844.9, 1),
(59, 'MaribeauTestBench', -33684.5, 1895, -45400.7, 2),
(60, 'TestTW', 186638.40625, -905, 50007.699219, 1),
(61, 'LargeHalls1', 186638, -905, 50007.7, 1);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `marketplace`
--

CREATE TABLE `marketplace` (
  `id` int(10) NOT NULL DEFAULT '0',
  `category` varchar(15) NOT NULL DEFAULT '0',
  `itemID` int(10) DEFAULT NULL,
  `purity` int(10) DEFAULT NULL,
  `delistPrice` int(10) DEFAULT NULL,
  `price` int(10) DEFAULT NULL,
  `charID` int(10) DEFAULT NULL,
  `charRep` int(10) DEFAULT NULL,
  `is_sold` int(10) DEFAULT NULL,
  `created` int(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `rsivalues`
--

CREATE TABLE `rsivalues` (
  `charid` smallint(6) NOT NULL,
  `sex` smallint(6) NOT NULL DEFAULT '0',
  `body` smallint(6) NOT NULL DEFAULT '0',
  `hat` smallint(6) NOT NULL DEFAULT '0',
  `face` smallint(6) NOT NULL DEFAULT '0',
  `shirt` smallint(6) NOT NULL DEFAULT '0',
  `coat` smallint(6) NOT NULL DEFAULT '0',
  `pants` smallint(6) NOT NULL DEFAULT '0',
  `shoes` smallint(6) NOT NULL DEFAULT '0',
  `gloves` smallint(6) NOT NULL DEFAULT '0',
  `glasses` smallint(6) NOT NULL DEFAULT '0',
  `hair` smallint(6) NOT NULL DEFAULT '0',
  `facialdetail` smallint(6) NOT NULL DEFAULT '0',
  `shirtcolor` smallint(6) NOT NULL DEFAULT '0',
  `pantscolor` smallint(6) NOT NULL DEFAULT '0',
  `coatcolor` smallint(6) NOT NULL DEFAULT '0',
  `shoecolor` smallint(6) NOT NULL DEFAULT '0',
  `glassescolor` smallint(6) NOT NULL DEFAULT '0',
  `haircolor` smallint(6) NOT NULL DEFAULT '0',
  `skintone` smallint(6) NOT NULL DEFAULT '0',
  `tattoo` smallint(6) NOT NULL DEFAULT '0',
  `facialdetailcolor` smallint(6) NOT NULL DEFAULT '0',
  `leggins` smallint(6) NOT NULL DEFAULT '0'
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Daten für Tabelle `rsivalues`
--

INSERT INTO `rsivalues` (`charid`, `sex`, `body`, `hat`, `face`, `shirt`, `coat`, `pants`, `shoes`, `gloves`, `glasses`, `hair`, `facialdetail`, `shirtcolor`, `pantscolor`, `coatcolor`, `shoecolor`, `glassescolor`, `haircolor`, `skintone`, `tattoo`, `facialdetailcolor`, `leggins`) VALUES
(2, 0, 0, 44, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0),
(3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
(141, 1, 1, 0, 0, 5, 0, 1, 4, 0, 3, 1, 0, 5, 16, 0, 1, 0, 1, 0, 1, 0, 0),
(97, 0, 1, 0, 2, 2, 0, 1, 1, 0, 1, 2, 0, 2, 1, 0, 1, 1, 0, 6, 1, 0, 0),
(110, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
(111, 1, 2, 0, 0, 5, 0, 0, 0, 0, 0, 10, 0, 5, 0, 0, 0, 0, 13, 0, 0, 0, 0),
(113, 1, 1, 0, 0, 6, 1, 0, 0, 0, 0, 1, 0, 3, 0, 9, 0, 0, 1, 0, 1, 0, 1),
(122, 1, 1, 0, 0, 5, 0, 0, 0, 0, 0, 1, 0, 5, 0, 0, 0, 0, 1, 0, 1, 0, 0),
(142, 0, 0, 0, 0, 2, 0, 1, 6, 3, 4, 8, 0, 0, 9, 0, 0, 0, 2, 15, 0, 0, 0),
(143, 0, 1, 0, 0, 1, 0, 10, 3, 0, 4, 11, 7, 12, 0, 0, 0, 0, 3, 7, 5, 0, 0),
(144, 0, 1, 5, 1, 2, 0, 4, 3, 0, 4, 1, 1, 0, 1, 0, 0, 0, 5, 0, 7, 0, 0),
(145, 0, 1, 5, 1, 2, 0, 4, 3, 3, 4, 1, 1, 0, 1, 0, 0, 0, 5, 0, 7, 0, 0);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `users`
--

CREATE TABLE `users` (
  `userId` int(11) UNSIGNED NOT NULL,
  `username` varchar(32) NOT NULL,
  `passwordSalt` varchar(32) DEFAULT NULL,
  `passwordHash` varchar(40) DEFAULT NULL,
  `publicExponent` smallint(11) UNSIGNED NOT NULL DEFAULT '0',
  `publicModulus` tinyblob,
  `privateExponent` tinyblob,
  `timeCreated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `account_status` int(11) NOT NULL DEFAULT '0' COMMENT 'if banned',
  `sessionid` varchar(100) DEFAULT NULL,
  `passwordmd5` varchar(40) DEFAULT NULL,
  `email_adress` varchar(255) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `users`
--

INSERT INTO `users` (`userId`, `username`, `passwordSalt`, `passwordHash`, `publicExponent`, `publicModulus`, `privateExponent`, `timeCreated`, `account_status`, `sessionid`, `passwordmd5`, `email_adress`) VALUES
(28, 'loluser', 'yYygfF9c', 'ba2bb6cc7d44b5b63dd4cef48baed88c', 17, 0xed7707ab64f87a51dd9020f73752e9455386a402bb539c64f8a4310ec211499dd503da74187ee109c325cf07ec0a2fcf904c15cf38918b9782578d589294b6bc0033802ee099e8a8163d3a1d8e1a7b3238ce4b58a20c01b47180823c3adc0b1d, 0x5acba10cd3c86b012f1176042bbdd1a990e0a81f2981d9ea5f119a496851e7710daea65990e537ed24f7de37bc22033809a6d66cab7205376859c0794708d71a6279537218a3d3496096ad23532179f1bb71315e64ffbee609b98d8f08e05887, '2017-11-27 13:49:17', 0, '', 'ba2bb6cc7d44b5b63dd4cef48baed88c', NULL),
(33, 'loluser2', 'yYygfF9c', 'ba2bb6cc7d44b5b63dd4cef48baed88c', 17, 0xed7707ab64f87a51dd9020f73752e9455386a402bb539c64f8a4310ec211499dd503da74187ee109c325cf07ec0a2fcf904c15cf38918b9782578d589294b6bc0033802ee099e8a8163d3a1d8e1a7b3238ce4b58a20c01b47180823c3adc0b1d, 0x5acba10cd3c86b012f1176042bbdd1a990e0a81f2981d9ea5f119a496851e7710daea65990e537ed24f7de37bc22033809a6d66cab7205376859c0794708d71a6279537218a3d3496096ad23532179f1bb71315e64ffbee609b98d8f08e05887, '2017-11-27 13:49:17', 0, '', 'ba2bb6cc7d44b5b63dd4cef48baed88c', NULL),
(48, 'loluser10', '', '372e291d08e14dd748faa8557d9f2839', 17, 0xed7707ab64f87a51dd9020f73752e9455386a402bb539c64f8a4310ec211499dd503da74187ee109c325cf07ec0a2fcf904c15cf38918b9782578d589294b6bc0033802ee099e8a8163d3a1d8e1a7b3238ce4b58a20c01b47180823c3adc0b1d, 0x5acba10cd3c86b012f1176042bbdd1a990e0a81f2981d9ea5f119a496851e7710daea65990e537ed24f7de37bc22033809a6d66cab7205376859c0794708d71a6279537218a3d3496096ad23532179f1bb71315e64ffbee609b98d8f08e05887, '2017-11-27 13:49:17', 0, NULL, '372e291d08e14dd748faa8557d9f2839', NULL),
(50, 'loluser3', 'yYygfF9c', 'ba2bb6cc7d44b5b63dd4cef48baed88c', 17, 0xed7707ab64f87a51dd9020f73752e9455386a402bb539c64f8a4310ec211499dd503da74187ee109c325cf07ec0a2fcf904c15cf38918b9782578d589294b6bc0033802ee099e8a8163d3a1d8e1a7b3238ce4b58a20c01b47180823c3adc0b1d, 0x5acba10cd3c86b012f1176042bbdd1a990e0a81f2981d9ea5f119a496851e7710daea65990e537ed24f7de37bc22033809a6d66cab7205376859c0794708d71a6279537218a3d3496096ad23532179f1bb71315e64ffbee609b98d8f08e05887, '2017-11-27 12:49:17', 0, '', 'ba2bb6cc7d44b5b63dd4cef48baed88c', NULL),
(51, 'loluser4', 'yYygfF9c', 'ba2bb6cc7d44b5b63dd4cef48baed88c', 17, 0xed7707ab64f87a51dd9020f73752e9455386a402bb539c64f8a4310ec211499dd503da74187ee109c325cf07ec0a2fcf904c15cf38918b9782578d589294b6bc0033802ee099e8a8163d3a1d8e1a7b3238ce4b58a20c01b47180823c3adc0b1d, 0x5acba10cd3c86b012f1176042bbdd1a990e0a81f2981d9ea5f119a496851e7710daea65990e537ed24f7de37bc22033809a6d66cab7205376859c0794708d71a6279537218a3d3496096ad23532179f1bb71315e64ffbee609b98d8f08e05887, '2017-11-27 12:49:17', 0, '', 'ba2bb6cc7d44b5b63dd4cef48baed88c', NULL),
(52, 'loluser5', 'yYygfF9c', 'ba2bb6cc7d44b5b63dd4cef48baed88c', 17, 0xed7707ab64f87a51dd9020f73752e9455386a402bb539c64f8a4310ec211499dd503da74187ee109c325cf07ec0a2fcf904c15cf38918b9782578d589294b6bc0033802ee099e8a8163d3a1d8e1a7b3238ce4b58a20c01b47180823c3adc0b1d, 0x5acba10cd3c86b012f1176042bbdd1a990e0a81f2981d9ea5f119a496851e7710daea65990e537ed24f7de37bc22033809a6d66cab7205376859c0794708d71a6279537218a3d3496096ad23532179f1bb71315e64ffbee609b98d8f08e05887, '2017-11-27 12:49:17', 0, '', 'ba2bb6cc7d44b5b63dd4cef48baed88c', NULL);

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `worlds`
--

CREATE TABLE `worlds` (
  `worldId` smallint(5) UNSIGNED NOT NULL,
  `name` varchar(20) NOT NULL,
  `type` tinyint(11) UNSIGNED NOT NULL DEFAULT '1' COMMENT '1 for no pvp, 2 for pvp',
  `status` tinyint(11) UNSIGNED NOT NULL DEFAULT '1' COMMENT 'World Status (Down, Open etc.)',
  `load` tinyint(3) UNSIGNED NOT NULL DEFAULT '49',
  `numPlayers` int(10) UNSIGNED NOT NULL DEFAULT '0'
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Daten für Tabelle `worlds`
--

INSERT INTO `worlds` (`worldId`, `name`, `type`, `status`, `load`, `numPlayers`) VALUES
(1, 'Recursion', 1, 1, 49, 0);

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `buddylist`
--
ALTER TABLE `buddylist`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `characters`
--
ALTER TABLE `characters`
  ADD PRIMARY KEY (`charId`),
  ADD KEY `handle` (`handle`);

--
-- Indizes für die Tabelle `char_abilities`
--
ALTER TABLE `char_abilities`
  ADD PRIMARY KEY (`id`),
  ADD KEY `char_id` (`char_id`);

--
-- Indizes für die Tabelle `char_hardlines`
--
ALTER TABLE `char_hardlines`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `crews`
--
ALTER TABLE `crews`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `crew_members`
--
ALTER TABLE `crew_members`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `data_hardlines`
--
ALTER TABLE `data_hardlines`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Id` (`Id`);

--
-- Indizes für die Tabelle `districts`
--
ALTER TABLE `districts`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `factions`
--
ALTER TABLE `factions`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `inventory`
--
ALTER TABLE `inventory`
  ADD PRIMARY KEY (`invId`);

--
-- Indizes für die Tabelle `locations`
--
ALTER TABLE `locations`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Id` (`Id`);

--
-- Indizes für die Tabelle `marketplace`
--
ALTER TABLE `marketplace`
  ADD PRIMARY KEY (`id`);

--
-- Indizes für die Tabelle `rsivalues`
--
ALTER TABLE `rsivalues`
  ADD PRIMARY KEY (`charid`);

--
-- Indizes für die Tabelle `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`userId`),
  ADD UNIQUE KEY `id` (`userId`),
  ADD UNIQUE KEY `username` (`username`);

--
-- Indizes für die Tabelle `worlds`
--
ALTER TABLE `worlds`
  ADD PRIMARY KEY (`worldId`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `buddylist`
--
ALTER TABLE `buddylist`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
--
-- AUTO_INCREMENT für Tabelle `characters`
--
ALTER TABLE `characters`
  MODIFY `charId` bigint(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=146;
--
-- AUTO_INCREMENT für Tabelle `char_abilities`
--
ALTER TABLE `char_abilities`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1023;
--
-- AUTO_INCREMENT für Tabelle `char_hardlines`
--
ALTER TABLE `char_hardlines`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT für Tabelle `crews`
--
ALTER TABLE `crews`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
--
-- AUTO_INCREMENT für Tabelle `crew_members`
--
ALTER TABLE `crew_members`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;
--
-- AUTO_INCREMENT für Tabelle `data_hardlines`
--
ALTER TABLE `data_hardlines`
  MODIFY `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=145;
--
-- AUTO_INCREMENT für Tabelle `districts`
--
ALTER TABLE `districts`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;
--
-- AUTO_INCREMENT für Tabelle `factions`
--
ALTER TABLE `factions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=126;
--
-- AUTO_INCREMENT für Tabelle `inventory`
--
ALTER TABLE `inventory`
  MODIFY `invId` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=208;
--
-- AUTO_INCREMENT für Tabelle `locations`
--
ALTER TABLE `locations`
  MODIFY `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=62;
--
-- AUTO_INCREMENT für Tabelle `users`
--
ALTER TABLE `users`
  MODIFY `userId` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=53;
--
-- AUTO_INCREMENT für Tabelle `worlds`
--
ALTER TABLE `worlds`
  MODIFY `worldId` smallint(5) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
