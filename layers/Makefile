
include $(INCLUDE_DIR)/utils.mk
include $(INCLUDE_DIR)/unpack.mk

include $(TOPDIR)/layers/$(LAYER).mk

ifeq ($(NAME),)
$(error Missing required parameter NAME for the layer $(LAYER))
endif

ifeq ($(VERSION),)
$(error Missing required parameter VERSION for the layer $(LAYER))
endif

define LayerInfo
NAME=$(NAME)
VERSION=$(VERSION)
DEPENDS=$(DEPENDS)
endef

SOURCES=$(BUILD_DIR)/$(NAME)-$(VERSION)
INSTALL_DIR=$(BUILD_DIR)/$(NAME)-$(VERSION)-install

MarkFile=$(SOURCES).$(1)

info:
	@:
	$(shell mkdir -p "$(BUILD_DIR)")
	$(file >$(BUILD_DIR)/$(LAYER).info,$(call LayerInfo))

download: $(call MarkFile,download)

$(call MarkFile,download):
	mkdir -p "$(DL_DIR)"
ifneq ($(URL),)
	curl -L --fail "$(URL)" > "$(DL_DIR)/$(notdir $(URL))"
endif
	mkdir -p "$(dir $@)" && touch "$@"

unpack: download
ifneq ($(UNPACK),)
	$(call UnpackArchive,$(UNPACK),$(SOURCES),$(abspath $(DL_DIR)/$(notdir $(URL))))
endif

compile: $(call MarkFile,compile)

$(call MarkFile,compile): unpack
	$(call Compile)
	mkdir -p "$(dir $@)" && touch "$@"

install: compile
	@rm -rf "$(INSTALL_DIR)"
	@mkdir -p "$(INSTALL_DIR)"
	$(call Install,$(INSTALL_DIR))

.PHONY: install unpack info download compile
