
include $(INCLUDE_DIR)/utils.mk
include $(INCLUDE_DIR)/unpack.mk

include $(TOPDIR)/layers/$(LAYER).mk

ifeq ($(NAME),)
$(error Missing required parameter NAME for the layer $(LAYER))
endif

ifeq ($(VERSION),)
$(error Missing required parameter VERSION for the layer $(LAYER))
endif

ORDER:=$(if $(ORDER),$(ORDER),0)
ifeq ($(shell echo $$(($(ORDER) >= 0 && $(ORDER) <= 9))),0)
$(error ORDER should be in range [0; 9])
endif

define LayerInfo
NAME=$(NAME)
VERSION=$(VERSION)
DEPENDS=$(DEPENDS)
ORDER=$(ORDER)
endef

SOURCES=$(BUILD_DIR)/$(NAME)-$(VERSION)

MarkFile=$(SOURCES).$(1)

info:
	@:
	$(shell mkdir -p "$(BUILD_DIR)")
	$(file >$(BUILD_DIR)/$(LAYER).info,$(LayerInfo))

download: $(call MarkFile,download)

$(call MarkFile,download):
	mkdir -p "$(DL_DIR)"
ifneq ($(URL),)
	curl -L --fail "$(URL)" > "$(DL_DIR)/$(notdir $(URL))"
endif
ifeq ($(DIRECTORY),)
	mkdir -p "$(dir $@)" && touch "$@"
endif

unpack: download
ifneq ($(UNPACK),)
	$(call UnpackArchive,$(UNPACK),$(SOURCES),$(abspath $(DL_DIR)/$(notdir $(URL))))
endif
ifneq ($(DIRECTORY),)
	rm -rf "$(SOURCES)" && cp -rf "$(TOPDIR)/$(DIRECTORY)/$(UNPACKED_DIR)" "$(SOURCES)"
endif

compile: $(call MarkFile,compile)

$(call MarkFile,compile): unpack
	$(call Compile)
	mkdir -p "$(dir $@)" && touch "$@"

install: compile
	@rm -rf "$(DEST)"
	@mkdir -p "$(DEST)"
	$(call Install,$(DEST))

.PHONY: install unpack info download compile
