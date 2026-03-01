

include include/utils.mk

export LAYERS
export TOPDIR=$(CURDIR)
export INCLUDE_DIR=$(TOPDIR)/include
export DL_DIR=$(TOPDIR)/dl
export BUILD_DIR=$(TOPDIR)/build

define LoadLayer
define Layer/$(1)/Makefile
$(subst $$,$$$$,$(file <layers/$(1).mk))
endef

export Layer/$(1)/Makefile
LAYERS+=$(1)

$(info $(MAKE) -C layers info 'LAYER=$(1))
$(shell $(MAKE) -C layers info 'LAYER=$(1)' >/dev/null 2>&1)

$(eval $(file <$(BUILD_DIR)/$(1).info))
$(eval Layer/$(NAME)=$(1))
$(eval Layer/$(1)/Name=$(NAME))
$(eval Layer/$(1)/Version=$(VERSION))
$(eval Layer/$(1)/Depends=$(DEPENDS))
$(eval $(file <$(INCLUDE_DIR)/reset_info.mk))

endef

define DefineLayer

install/$(1): $$(foreach dep,$$(Layer/$(1)/Depends),install/$$(Layer/$$(dep)))
	$(MAKE) -C layers install 'LAYER=$(1)' $(MAKEFLAGS)

download/$(1):
	$(MAKE) -C layers download 'LAYER=$(1)' $(MAKEFLAGS)

endef

$(foreach Layer,$(wildcard layers/*.mk), \
	$(eval $(call LoadLayer,$(basename $(notdir $(Layer))))))

all: install

clean:
	rm -rf build dl overlay

$(foreach layer,$(LAYERS), \
	$(eval $(call DefineLayer,$(layer))))

install_layers: $(foreach layer,$(LAYERS),install/$(layer))

install: install_layers
	rm -rf "$(TOPDIR)/overlay"
	mkdir -p "$(TOPDIR)/overlay"
	$(foreach layer,$(LAYERS), cp -rf "$(BUILD_DIR)/$(Layer/$(layer)/Name)-$(Layer/$(layer)/Version)-install"/* "$(TOPDIR)/overlay";$(NL))

download: $(foreach layer,$(LAYERS),download/$(layer))

.PHONY: all clean download install clean_install overlay \
	$(foreach layer,$(LAYERS),install/$(layer))
