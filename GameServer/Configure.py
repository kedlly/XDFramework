#coding:utf8

import ConfigParser

__CONFIGURES__ = \
{
#section__keys = ("A", "B", ...)
	"server": ("ipAddress", "port")
}


def getConfigure(filepath):
	config = ConfigParser.ConfigParser()
	config.read(filepath)
	nd = {}
	for i in __CONFIGURES__.iteritems():
		key = i[0]
		nd[key] = list()
		for j in i[1]:
			nd[key].append (config.get(key, j))
	return nd