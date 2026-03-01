NAME=ldufk
VERSION=0.0.0
DIRECTORY=ldufk

ALL_LAYERS=$(foreach layer,$(LAYERS),$(Layer/$(layer)/Name))
DEPENDS=$(filter-out $(NAME),$(ALL_LAYERS))

define Install
	cp -r "$(SOURCES)"/* "$(1)"
endef
