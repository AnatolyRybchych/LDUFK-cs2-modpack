
NAME=CounterStrikeSharp
VERSION=1.0.363
UNPACK=zip
DEPENDS=
URL=https://github.com/roflmuffin/CounterStrikeSharp/releases/download/v$(VERSION)/counterstrikesharp-linux-$(VERSION).zip

define Install
	cp -rn "$(SOURCES)"/* "$(1)"
endef
