SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;


CREATE TABLE IF NOT EXISTS `characters` (
  `charId` bigint(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL,
  `worldId` smallint(5) unsigned NOT NULL,
  `status` tinyint(3) unsigned NOT NULL COMMENT 'transit/banned',
  `handle` varchar(32) NOT NULL,
  `firstName` varchar(32) NOT NULL,
  `lastName` varchar(32) NOT NULL,
  `background` varchar(1024) DEFAULT NULL,
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '0',
  `rotation` mediumint(11) NOT NULL DEFAULT '0',
  `healthC` mediumint(11) NOT NULL DEFAULT '500',
  `healthM` mediumint(11) NOT NULL DEFAULT '500',
  `innerStrC` mediumint(11) NOT NULL DEFAULT '200',
  `innerStrM` mediumint(11) NOT NULL DEFAULT '200',
  `level` mediumint(11) NOT NULL DEFAULT '1',
  `profession` smallint(6) NOT NULL DEFAULT '2',
  `alignment` smallint(6) NOT NULL DEFAULT '0',
  `pvpflag` smallint(6) NOT NULL DEFAULT '0',
  `exp` int(11) NOT NULL,
  `cash` int(11) NOT NULL,
  `district` text NOT NULL,
  PRIMARY KEY (`charId`),
  UNIQUE KEY `handle` (`handle`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=10 ;

INSERT INTO `characters` (`charId`, `userId`, `worldId`, `status`, `handle`, `firstName`, `lastName`, `background`, `x`, `y`, `z`, `rotation`, `healthC`, `healthM`, `innerStrC`, `innerStrM`, `level`, `profession`, `alignment`, `pvpflag`, `exp`, `cash`, `district`) VALUES
(2, 28, 1, 0, 'mrCoder', 'Mr', 'Coder', 'Likes cookies', 16900, 500, 2800, 0, 500, 500, 200, 200, 21, 2, 0, 0, 0, 0, 'slums'),
(3, 28, 1, 0, 'TheLevelOne', 'Testing', 'Chat', 'Just testing', 16900, 500, 2800, 15, 500, 500, 200, 200, 1, 2, 1, 0, 0, 0, 'slums'),
(4, 28, 1, 0, 'SexyGal', 'Sexy', 'Gal', 'This is a sexy gal boyz', -6300, 1900, 1500, 0, 500, 500, 200, 200, 17, 8, 1, 0, 1337, 987654321, 'downtown');

CREATE TABLE IF NOT EXISTS `inventory` (
  `invId` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `charId` bigint(11) unsigned NOT NULL,
  `goid` int(11) unsigned NOT NULL,
  `slot` tinyint(11) unsigned NOT NULL,
  PRIMARY KEY (`invId`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;


CREATE TABLE IF NOT EXISTS `rsivalues` (
  `charid` smallint(6) NOT NULL,
  `sex` smallint(6) NOT NULL,
  `body` smallint(6) NOT NULL,
  `hat` smallint(6) NOT NULL,
  `face` smallint(6) NOT NULL,
  `shirt` smallint(6) NOT NULL,
  `coat` smallint(6) NOT NULL,
  `pants` smallint(6) NOT NULL,
  `shoes` smallint(6) NOT NULL,
  `gloves` smallint(6) NOT NULL,
  `glasses` smallint(6) NOT NULL,
  `hair` smallint(6) NOT NULL,
  `facialdetail` smallint(6) NOT NULL,
  `shirtcolor` smallint(6) NOT NULL,
  `pantscolor` smallint(6) NOT NULL,
  `coatcolor` smallint(6) NOT NULL,
  `shoecolor` smallint(6) NOT NULL,
  `glassescolor` smallint(6) NOT NULL,
  `haircolor` smallint(6) NOT NULL,
  `skintone` smallint(6) NOT NULL,
  `tatto` smallint(6) NOT NULL,
  `facialdetailcolor` smallint(6) NOT NULL,
  `leggins` smallint(6) NOT NULL,
  PRIMARY KEY (`charid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

INSERT INTO `rsivalues` (`charid`, `sex`, `body`, `hat`, `face`, `shirt`, `coat`, `pants`, `shoes`, `gloves`, `glasses`, `hair`, `facialdetail`, `shirtcolor`, `pantscolor`, `coatcolor`, `shoecolor`, `glassescolor`, `haircolor`, `skintone`, `tatto`, `facialdetailcolor`, `leggins`) VALUES
(2, 1, 2, 0, 6, 8, 0, 0, 3, 4, 1, 3, 0, 2, 3, 0, 0, 0, 6, 0, 2, 3, 1),
(3, 0, 1, 3, 4, 5, 6, 5, 3, 3, 2, 1, 0, 2, 3, 4, 5, 1, 4, 2, 5, 0, 0),
(4, 1, 3, 0, 19, 12, 0, 1, 1, 4, 3, 9, 0, 22, 0, 0, 2, 1, 0, 10, 3, 0, 0);

CREATE TABLE IF NOT EXISTS `users` (
  `userId` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(32) NOT NULL,
  `passwordSalt` varchar(32) NOT NULL,
  `passwordHash` varchar(40) NOT NULL,
  `publicExponent` smallint(11) unsigned NOT NULL DEFAULT '0',
  `publicModulus` tinyblob,
  `privateExponent` tinyblob,
  `timeCreated` int(10) unsigned NOT NULL,
  `account_status` int(11) NOT NULL DEFAULT '0' COMMENT 'if banned',
  `sessionid` varchar(100) DEFAULT NULL,
  `passwordmd5` varchar(40) DEFAULT NULL,
  UNIQUE KEY `id` (`userId`),
  UNIQUE KEY `username` (`username`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=33 ;

INSERT INTO `users` (`userId`, `username`, `passwordSalt`, `passwordHash`, `publicExponent`, `publicModulus`, `privateExponent`, `timeCreated`, `account_status`, `sessionid`, `passwordmd5`) VALUES
(28, 'user', 'yYygfF9c', '0b2825013f155cc11f7433780f54b9b412e2a52f', 17, 0xed7707ab64f87a51dd9020f73752e9455386a402bb539c64f8a4310ec211499dd503da74187ee109c325cf07ec0a2fcf904c15cf38918b9782578d589294b6bc0033802ee099e8a8163d3a1d8e1a7b3238ce4b58a20c01b47180823c3adc0b1d, 0x5acba10cd3c86b012f1176042bbdd1a990e0a81f2981d9ea5f119a496851e7710daea65990e537ed24f7de37bc22033809a6d66cab7205376859c0794708d71a6279537218a3d3496096ad23532179f1bb71315e64ffbee609b98d8f08e05887, 1250413439, 0, '', 'ba2bb6cc7d44b5b63dd4cef48baed88c');

CREATE TABLE IF NOT EXISTS `worlds` (
  `worldId` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(20) NOT NULL,
  `type` tinyint(11) unsigned NOT NULL DEFAULT '1' COMMENT '1 for no pvp, 2 for pvp',
  `status` tinyint(11) unsigned NOT NULL DEFAULT '1' COMMENT 'World Status (Down, Open etc.)',
  `load` tinyint(3) unsigned NOT NULL DEFAULT '49',
  `numPlayers` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`worldId`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=5 ;

INSERT INTO `worlds` (`worldId`, `name`, `type`, `status`, `load`, `numPlayers`) VALUES
(1, 'Recursion', 1, 1, 49, 0),
(2, 'TestServer2', 1, 1, 49, 0),
(3, 'HostileWorld1', 2, 1, 49, 0),
(4, 'EnterTheMatricks', 1, 1, 49, 0);
