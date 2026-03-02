
include $(INCLUDE_DIR)/utils.mk
include $(INCLUDE_DIR)/unpack.mk
include $(INCLUDE_DIR)/download.mk

SOURCES=$(BUILD_DIR)/$(NAME)-$(VERSION)

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

DOWNLOAD=curl
DL_PATH=$(DL_DIR)/$(notdir $(URL))

ifneq ($(patsubst file://%,%,$(URL)),$(URL))
URL:=$(patsubst file://%,%,$(URL))
DOWNLOAD=none
UNPACK=cp
DL_PATH=$(abspath $(URL))
else ifneq ($(patsubst %.git,%,$(URL)),$(URL))
DOWNLOAD=git
UNPACK=cp
DL_PATH=$(DL_DIR)/$(NAME)-$(VERSION)
else ifneq ($(URL),)
DOWNLOAD=curl
ifeq ($(UNPACK),)
$(error UNPACK is mandatory for a web link)
endif

else
DOWNLOAD=none
UNPACK=none
UNPACK=cp
endif

define LayerInfo
NAME=$(NAME)
VERSION=$(VERSION)
DEPENDS=$(DEPENDS)
ORDER=$(ORDER)
endef

MarkFile=$(BUILD_DIR)/.mark/$(NAME)-$(VERSION).$(1)
MarkDlFile=$(DL_DIR)/.mark/$(NAME)-$(VERSION).$(1)

info:
	@:
	$(shell mkdir -p "$(BUILD_DIR)/.info")
	$(file >$(BUILD_DIR)/.info/$(LAYER).info,$(LayerInfo))

download: $(call MarkDlFile,download)
$(call MarkDlFile,download):
	$(call Download,$(DOWNLOAD),$(URL),$(DL_PATH))
	mkdir -p "$(dir $@)" && touch "$@"

unpack: $(call MarkFile,unpack)
$(call MarkFile,unpack): $(call MarkDlFile,download)
	$(call UnpackArchive,$(UNPACK),$(SOURCES),$(DL_PATH))
	mkdir -p "$(dir $@)" && touch "$@"

compile: $(call MarkFile,compile)
$(call MarkFile,compile): $(call MarkFile,unpack)
	$(call Compile)
	mkdir -p "$(dir $@)" && touch "$@"

install: compile
	@rm -rf "$(DEST)"
	@mkdir -p "$(DEST)"
	$(call Install,$(DEST))

.PHONY: install unpack info download compile
