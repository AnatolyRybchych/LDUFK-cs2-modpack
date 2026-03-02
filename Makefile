

include include/utils.mk

export LAYERS=$(foreach layer,$(wildcard layers/*.mk),$(basename $(notdir $(layer))))
export TOPDIR=$(CURDIR)
export INCLUDE_DIR=$(TOPDIR)/include
export DL_DIR=$(TOPDIR)/dl
export BUILD_DIR=$(TOPDIR)/build

define PopulateLayer
$(info $(MAKE) -C layers info 'LAYER=$(1)' $(MAKEFLAGS))
$(shell $(MAKE) -C layers info 'LAYER=$(1)' $(MAKEFLAGS) >/dev/null 2>&1)

$(eval $(file <$(BUILD_DIR)/$(1).info))
$(eval export Layer/$(NAME)=$(1))
$(eval export Layer/$(1)/Name=$(NAME))
$(eval export Layer/$(1)/Version=$(VERSION))
$(eval export Layer/$(1)/Order=$(ORDER))
$(eval export Layer/$(1)/Depends=$(DEPENDS))
$(eval $(file <$(INCLUDE_DIR)/reset_info.mk))
endef

RequrDep=$(Layer/$(1)/Name) $(foreach dep,$(Layer/$(1)/Depends), \
	$(call RequrDep,$(Layer/$(dep))))

InstallDest=$(BUILD_DIR)/layers/$(Layer/$(1)/Index)-$(Layer/$(1)/Name)-$(Layer/$(1)/Version)

define DefineLayer

install/$(1): $$(foreach dep,$$(Layer/$(1)/Depends),install/$$(Layer/$$(dep)))
	$(MAKE) -C layers install \
		'LAYER=$(1)' \
		'DEST=$(call InstallDest,$(1))' $(MAKEFLAGS)

download/$(1):
	$(MAKE) -C layers download 'LAYER=$(1)' $(MAKEFLAGS)

endef

$(foreach layer,$(LAYERS), \
	$(eval $(call PopulateLayer,$(layer))))

$(foreach layer,$(LAYERS), \
	$(eval Layer/$(layer)/AllDepend=$(filter-out $(Layer/$(layer)/Name),$(shell echo $(call RequrDep,$(layer)) | xargs -n1 | sort -u | xargs))))

$(foreach layer,$(LAYERS), \
	$(eval Layer/$(layer)/Index=$(shell echo $(Layer/$(layer)/AllDepend) | wc -w | xargs printf '%1d%03d' "$(Layer/$(layer)/Order)")))

$(foreach layer,$(LAYERS), \
	$(eval $(call AssertEmpty,Some $(Layer/$(layer)/Name) dependencies have ORDER >= $(Layer/$(layer)/Order),\
		$(shell $(foreach dep,$(Layer/$(layer)/Depends),[ "$(Layer/$(layer)/Order)" -lt "$(Layer/$(Layer/$(dep))/Order)" ] && echo "$(dep)";)))))

all: install

clean:
	rm -rf build dl overlay

$(foreach layer,$(LAYERS), \
	$(eval $(call DefineLayer,$(layer))))

install_layers: $(foreach layer,$(LAYERS),install/$(layer))

install: install_layers
	rm -rf "$(TOPDIR)/overlay"
	mkdir -p "$(TOPDIR)/overlay"
	$(foreach layer_files,$(sort $(foreach layer,$(LAYERS),$(call InstallDest,$(layer)))), \
		cp -rf "$(layer_files)"/* "$(TOPDIR)/overlay";$(NL))

download: $(foreach layer,$(LAYERS),download/$(layer))

.PHONY: all clean download install clean_install overlay \
	$(foreach layer,$(LAYERS),install/$(layer))
