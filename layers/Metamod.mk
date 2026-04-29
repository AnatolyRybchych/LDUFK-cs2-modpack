
NAME=Metamod
VERSION=2.0.0
BUILD=1398
UNPACK=tar
DEPENDS=
URL=https://github.com/alliedmodders/metamod-source/releases/download/$(VERSION).$(BUILD)/mmsource-2.0.0-git$(BUILD)-linux.tar.gz

define Install
	cp -rn "$(SOURCES)"/* "$(1)"
endef
