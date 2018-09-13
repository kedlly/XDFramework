#coding:utf8

from ConfigParser import ConfigParser, NoOptionError





def __getConfigure(filepath, propertyDict):
	config = ConfigParser()
	config.read(filepath)
	nd = {}
	for i in propertyDict.iteritems():
		key = i[0]
		nd[key] = {}
		for j in i[1]:
			try :
				nd[key][j] = config.get(key, j)
			except NoOptionError:
				nd[key][j] = None
	return nd

def __loadDatabaseCfg(file):
	options = __getConfigure(file, __DB_CONFIGURES__)
	db_type = options["Database"]["type"]
	return db_type, options[db_type]

def __getOracleDataSource(cfg_data):
	'oracle://username:password@192.168.1.6:1521/databasename'
	if cfg_data['NLS_LANG'] is not None:
		import os
		os.environ['NLS_LANG'] = cfg_data['NLS_LANG']
	return "oracle://%s:%s@%s:%s/%s" % (cfg_data["user"], cfg_data["password"], cfg_data["ipAddr"], cfg_data["port"], cfg_data["sid"])


__DB_CONFIGURES__ = \
{
#section__keys = ("A", "B", ...)
	"Database": ("type",),
	"Oracle": ("ipAddr", "port", "sid", "user", "password", "NLS_LANG")
}

__CFG_PROCESSOR__ = \
{
	"Oracle":__getOracleDataSource,
}



__LOGIC_SRV_KEY__ = "LogicServer"
__PMS_PROXY_SRV_KEY__ = "PMSProxyServer"
__SRV_OPTS__ = ("ipAddr", "port")
__SERVER_CONFIGURES__ = \
{
#section__keys = ("A", "B", ...)
	__LOGIC_SRV_KEY__ : __SRV_OPTS__,
	__PMS_PROXY_SRV_KEY__ : __SRV_OPTS__
}


__FILE_SRV_CFG__ = "config.ini"
__FILE_DB_CFG__ = "database.ini"


__db_type, __db_cfg = __loadDatabaseCfg(__FILE_DB_CFG__)
__SRVS_CFG__ =  __getConfigure(__FILE_SRV_CFG__, __SERVER_CONFIGURES__)

DATA_SOURCE_NAME =  __CFG_PROCESSOR__[__db_type](__db_cfg)
LOGIC_SERVER_IP, LOGIC_SERVER_PORT =__SRVS_CFG__[__LOGIC_SRV_KEY__].get(__SRV_OPTS__[0], "127.0.0.1"), int(__SRVS_CFG__[__LOGIC_SRV_KEY__].get(__SRV_OPTS__[1]))
PMS_PROXY_SERVER_IP, PMS_PROXY_SERVER_PORT = __SRVS_CFG__[__PMS_PROXY_SRV_KEY__].get(__SRV_OPTS__[0], "127.0.0.1"), int(__SRVS_CFG__[__PMS_PROXY_SRV_KEY__].get(__SRV_OPTS__[1]))