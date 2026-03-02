NAME=ldufk
VERSION=0.0.0
DIRECTORY=ldufk
DEPENDS=CounterStrikeSharp MatchZy
ORDER=1

define Install
	cp -r "$(SOURCES)"/* "$(1)"
endef
