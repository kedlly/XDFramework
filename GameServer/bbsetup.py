#coding:utf8

from bbfreeze import Freezer

freezer = Freezer(distdir='dist',  includes=["os", "ssl", "pkgutil"],)
freezer.addScript('Server.py', gui_only=False)
freezer()