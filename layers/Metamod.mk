
NAME=Metamod
VERSION=2.0.0
UNPACK=tar
DEPENDS=
URL=https://mms.alliedmods.net/mmsdrop/2.0/mmsource-2.0.0-git1387-linux.tar.gz

define Install
	cp -rn "$(SOURCES)"/* "$(1)"
endef
