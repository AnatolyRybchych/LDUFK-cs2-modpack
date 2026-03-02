
NAME=CounterStrikeSharp
VERSION=1.0.357
UNPACK=zip
DEPENDS=Metamod

# URL=https://github.com/roflmuffin/CounterStrikeSharp/releases/download/v$(VERSION)/counterstrikesharp-linux-$(VERSION).zip
URL=https://github.com/roflmuffin/CounterStrikeSharp/releases/download/v$(VERSION)/counterstrikesharp-with-runtime-linux-$(VERSION).zip
define Install
	cp -rn "$(SOURCES)"/* "$(1)"
endef
