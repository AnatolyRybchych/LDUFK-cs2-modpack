
NAME=MatchZy
VERSION=0.8.15
UNPACK=zip
DEPENDS=CounterStrikeSharp
URL=https://github.com/shobhit-pathak/MatchZy/releases/download/$(VERSION)/MatchZy-$(VERSION).zip

define Install
	cp -rn "$(SOURCES)"/* "$(1)"
endef
