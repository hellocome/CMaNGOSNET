﻿using System.Collections.Generic;
using CMaNGOSNET.Common.Database;

namespace CMaNGOSNET.AuthServer.Database.MySQL
{
    public static class MySQLLoginDatabaseStatements
    {
        public static readonly Dictionary<int, PreparedStatement> PreparedStatementMap = new Dictionary<int, PreparedStatement>();

        static MySQLLoginDatabaseStatements()
        {
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_REALMLIST, "SELECT id, name, address, localAddress, localSubnetMask, port, icon, flag, timezone, allowedSecurityLevel, population, gamebuild FROM realmlist WHERE flag <> 3 ORDER BY name", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_EXPIRED_IP_BANS, "DELETE FROM ip_banned WHERE unbandate<>bandate AND unbandate<=UNIX_TIMESTAMP()", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_EXPIRED_ACCOUNT_BANS, "UPDATE account_banned SET active = 0 WHERE active = 1 AND unbandate<>bandate AND unbandate<=UNIX_TIMESTAMP()", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_IP_BANNED, "SELECT * FROM ip_banned WHERE ip = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_IP_AUTO_BANNED, "INSERT INTO ip_banned (ip, bandate, unbandate, bannedby, banreason) VALUES (?, UNIX_TIMESTAMP(); UNIX_TIMESTAMP()+?, 'Trinity Auth', 'Failed login autoban')", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_IP_BANNED_ALL, "SELECT ip, bandate, unbandate, bannedby, banreason FROM ip_banned WHERE (bandate = unbandate OR unbandate > UNIX_TIMESTAMP()) ORDER BY unbandate", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_IP_BANNED_BY_IP, "SELECT ip, bandate, unbandate, bannedby, banreason FROM ip_banned WHERE (bandate = unbandate OR unbandate > UNIX_TIMESTAMP()) AND ip LIKE CONCAT('%%', ?, '%%') ORDER BY unbandate", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_BANNED, "SELECT bandate, unbandate FROM account_banned WHERE id = ? AND active = 1", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_BANNED_ALL, "SELECT account.id, username FROM account, account_banned WHERE account.id = account_banned.id AND active = 1 GROUP BY account.id", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_BANNED_BY_USERNAME, "SELECT account.id, username FROM account, account_banned WHERE account.id = account_banned.id AND active = 1 AND username LIKE CONCAT('%%', ?, '%%') GROUP BY account.id", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_ACCOUNT_AUTO_BANNED, "INSERT INTO account_banned VALUES (?, UNIX_TIMESTAMP(); UNIX_TIMESTAMP()+?, 'Trinity Auth', 'Failed login autoban', 1)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_ACCOUNT_BANNED, "DELETE FROM account_banned WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_SESSIONKEY, "SELECT a.sessionkey, a.id, aa.gmlevel  FROM account a LEFT JOIN account_access aa ON (a.id = aa.id) WHERE username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_VS, "UPDATE account SET v = ?, s = ? WHERE username = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_LOGONPROOF, "UPDATE account SET sessionkey = ?, last_ip = ?, last_login = NOW(); locale = ?, failed_logins = 0, os = ? WHERE username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_LOGONCHALLENGE, "SELECT a.sha_pass_hash, a.id, a.locked, a.lock_country, a.last_ip, aa.gmlevel, a.v, a.s, a.token_key FROM account a LEFT JOIN account_access aa ON (a.id = aa.id) WHERE a.username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_LOGON_COUNTRY, "SELECT country FROM ip2nation WHERE ip < ? ORDER BY ip DESC LIMIT 0,1", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_FAILEDLOGINS, "UPDATE account SET failed_logins = failed_logins + 1 WHERE username = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_FAILEDLOGINS, "SELECT id, failed_logins FROM account WHERE username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_ID_BY_NAME, "SELECT id FROM account WHERE username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_LIST_BY_NAME, "SELECT id, username FROM account WHERE username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_INFO_BY_NAME, "SELECT id, sessionkey, last_ip, locked, expansion, mutetime, locale, recruiter, os FROM account WHERE username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_LIST_BY_EMAIL, "SELECT id, username FROM account WHERE email = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_NUM_CHARS_ON_REALM, "SELECT numchars FROM realmcharacters WHERE realmid = ? AND acctid= ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_BY_IP, "SELECT id, username FROM account WHERE last_ip = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_BY_ID, "SELECT 1 FROM account WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_IP_BANNED, "INSERT INTO ip_banned (ip, bandate, unbandate, bannedby, banreason) VALUES (?, UNIX_TIMESTAMP(); UNIX_TIMESTAMP()+?, ?, ?)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_IP_NOT_BANNED, "DELETE FROM ip_banned WHERE ip = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_ACCOUNT_BANNED, "INSERT INTO account_banned VALUES (?, UNIX_TIMESTAMP(); UNIX_TIMESTAMP()+?, ?, ?, 1)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_ACCOUNT_NOT_BANNED, "UPDATE account_banned SET active = 0 WHERE id = ? AND active != 0", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_REALM_CHARACTERS_BY_REALM, "DELETE FROM realmcharacters WHERE acctid = ? AND realmid = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_REALM_CHARACTERS, "DELETE FROM realmcharacters WHERE acctid = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_REALM_CHARACTERS, "INSERT INTO realmcharacters (numchars, acctid, realmid) VALUES (?, ?, ?)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_SUM_REALM_CHARACTERS, "SELECT SUM(numchars) FROM realmcharacters WHERE acctid = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_ACCOUNT, "INSERT INTO account(username, sha_pass_hash, reg_mail, email, joindate) VALUES(?, ?, ?, ?, NOW())", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_REALM_CHARACTERS_INIT, "INSERT INTO realmcharacters (realmid, acctid, numchars) SELECT realmlist.id, account.id, 0 FROM realmlist, account LEFT JOIN realmcharacters ON acctid=account.id WHERE acctid IS NULL", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_EXPANSION, "UPDATE account SET expansion = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_ACCOUNT_LOCK, "UPDATE account SET locked = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_ACCOUNT_LOCK_CONTRY, "UPDATE account SET lock_country = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_LOG, "INSERT INTO logs (time, realm, type, level, string) VALUES (?, ?, ?, ?, ?)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_USERNAME, "UPDATE account SET v = 0, s = 0, username = ?, sha_pass_hash = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_PASSWORD, "UPDATE account SET v = 0, s = 0, sha_pass_hash = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_EMAIL, "UPDATE account SET email = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_REG_EMAIL, "UPDATE account SET reg_mail = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_MUTE_TIME, "UPDATE account SET mutetime = ? , mutereason = ? , muteby = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_MUTE_TIME_LOGIN, "UPDATE account SET mutetime = ? WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_LAST_IP, "UPDATE account SET last_ip = ? WHERE username = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_LAST_ATTEMPT_IP, "UPDATE account SET last_attempt_ip = ? WHERE username = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_ACCOUNT_ONLINE, "UPDATE account SET online = 1 WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_UPD_UPTIME_PLAYERS, "UPDATE uptime SET uptime = ?, maxplayers = ? WHERE realmid = ? AND starttime = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_OLD_LOGS, "DELETE FROM logs WHERE (time + ?) < ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_ACCOUNT_ACCESS, "DELETE FROM account_access WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_ACCOUNT_ACCESS_BY_REALM, "DELETE FROM account_access WHERE id = ? AND (RealmID = ? OR RealmID = -1)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_ACCOUNT_ACCESS, "INSERT INTO account_access (id,gmlevel,RealmID) VALUES (?, ?, ?)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_GET_ACCOUNT_ID_BY_USERNAME, "SELECT id FROM account WHERE username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_GET_ACCOUNT_ACCESS_GMLEVEL, "SELECT gmlevel FROM account_access WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_GET_GMLEVEL_BY_REALMID, "SELECT gmlevel FROM account_access WHERE id = ? AND (RealmID = ? OR RealmID = -1)", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_GET_USERNAME_BY_ID, "SELECT username FROM account WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_CHECK_PASSWORD, "SELECT 1 FROM account WHERE id = ? AND sha_pass_hash = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_CHECK_PASSWORD_BY_NAME, "SELECT 1 FROM account WHERE username = ? AND sha_pass_hash = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_PINFO, "SELECT a.username, aa.gmlevel, a.email, a.reg_mail, a.last_ip, DATE_FORMAT(a.last_login, '%Y-%m-%d %T'); a.mutetime, a.mutereason, a.muteby, a.failed_logins, a.locked, a.OS FROM account a LEFT JOIN account_access aa ON (a.id = aa.id AND (aa.RealmID = ? OR aa.RealmID = -1)) WHERE a.id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_PINFO_BANS, "SELECT unbandate, bandate = unbandate, bannedby, banreason FROM account_banned WHERE id = ? AND active ORDER BY bandate ASC LIMIT 1", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_GM_ACCOUNTS, "SELECT a.username, aa.gmlevel FROM account a, account_access aa WHERE a.id=aa.id AND aa.gmlevel >= ? AND (aa.realmid = -1 OR aa.realmid = ?)", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_INFO, "SELECT a.username, a.last_ip, aa.gmlevel, a.expansion FROM account a LEFT JOIN account_access aa ON (a.id = aa.id) WHERE a.id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_ACCESS_GMLEVEL_TEST, "SELECT 1 FROM account_access WHERE id = ? AND gmlevel > ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_ACCESS, "SELECT a.id, aa.gmlevel, aa.RealmID FROM account a LEFT JOIN account_access aa ON (a.id = aa.id) WHERE a.username = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_RECRUITER, "SELECT 1 FROM account WHERE recruiter = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_BANS, "SELECT 1 FROM account_banned WHERE id = ? AND active = 1 UNION SELECT 1 FROM ip_banned WHERE ip = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_WHOIS, "SELECT username, email, last_ip FROM account WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_LAST_ATTEMPT_IP, "SELECT last_attempt_ip FROM account WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_LAST_IP, "SELECT last_ip FROM account WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_REALMLIST_SECURITY_LEVEL, "SELECT allowedSecurityLevel from realmlist WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_ACCOUNT, "DELETE FROM account WHERE id = ?", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_IP2NATION_COUNTRY, "SELECT c.country FROM ip2nationCountries c, ip2nation i WHERE i.ip < ? AND c.code = i.country ORDER BY i.ip DESC LIMIT 0,1", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_AUTOBROADCAST, "SELECT id, weight, text FROM autobroadcast WHERE realmid = ? OR realmid = -1", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_GET_EMAIL_BY_ID, "SELECT email FROM account WHERE id = ?", ConnectionFlags.CONNECTION_SYNCH);
            // 0: uint32, 1: uint32, 2: uint8, 3: uint32, 4: string // Complete name: "Login_Insert_AccountLoginDeLete_IP_Logging"
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_ALDL_IP_LOGGING, "INSERT INTO logs_ip_actions (account_id,character_guid,type,ip,systemnote,unixtime,time) VALUES (?, ?, ?, (SELECT last_ip FROM account WHERE id = ?); ?, unix_timestamp(NOW()); NOW())", ConnectionFlags.CONNECTION_ASYNC);
            // 0: uint32, 1: uint32, 2: uint8, 3: uint32, 4: string // Complete name: "Login_Insert_FailedAccountLogin_IP_Logging"
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_FACL_IP_LOGGING, "INSERT INTO logs_ip_actions (account_id,character_guid,type,ip,systemnote,unixtime,time) VALUES (?, ?, ?, (SELECT last_attempt_ip FROM account WHERE id = ?); ?, unix_timestamp(NOW()); NOW())", ConnectionFlags.CONNECTION_ASYNC);
            // 0: uint32, 1: uint32, 2: uint8, 3: string, 4: string // Complete name: "Login_Insert_CharacterDelete_IP_Logging"
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_CHAR_IP_LOGGING, "INSERT INTO logs_ip_actions (account_id,character_guid,type,ip,systemnote,unixtime,time) VALUES (?, ?, ?, ?, ?, unix_timestamp(NOW()); NOW())", ConnectionFlags.CONNECTION_ASYNC);
            // 0: string, 1: string, 2: string                      // Complete name: "Login_Insert_Failed_Account_Login_due_password_IP_Logging"
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_FALP_IP_LOGGING, "INSERT INTO logs_ip_actions (account_id,character_guid,type,ip,systemnote,unixtime,time) VALUES ((SELECT id FROM account WHERE username = ?); 0, 1, ?, ?, unix_timestamp(NOW()); NOW())", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_ACCOUNT_ACCESS_BY_ID, "SELECT gmlevel, RealmID FROM account_access WHERE id = ? and (RealmID = ? OR RealmID = -1) ORDER BY gmlevel desc", ConnectionFlags.CONNECTION_SYNCH);

            PrepareStatement(LoginDatabaseStatements.LOGIN_SEL_RBAC_ACCOUNT_PERMISSIONS, "SELECT permissionId, granted FROM rbac_account_permissions WHERE accountId = ? AND (realmId = ? OR realmId = -1) ORDER BY permissionId, realmId", ConnectionFlags.CONNECTION_SYNCH);
            PrepareStatement(LoginDatabaseStatements.LOGIN_INS_RBAC_ACCOUNT_PERMISSION, "INSERT INTO rbac_account_permissions (accountId, permissionId, granted, realmId) VALUES (?, ?, ?, ?) ON DUPLICATE KEY UPDATE granted = VALUES(granted)", ConnectionFlags.CONNECTION_ASYNC);
            PrepareStatement(LoginDatabaseStatements.LOGIN_DEL_RBAC_ACCOUNT_PERMISSION, "DELETE FROM rbac_account_permissions WHERE accountId = ? AND permissionId = ? AND (realmId = ? OR realmId = -1)", ConnectionFlags.CONNECTION_ASYNC);
        }

        static void PrepareStatement(LoginDatabaseStatements statement, string sql, ConnectionFlags flag)
        {
            PreparedStatementMap.Add((int)statement, new PreparedStatement((int)statement, sql, flag));
        }
    }
}
