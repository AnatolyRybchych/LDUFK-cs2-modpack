

define Unpack/zip
	rm -rf "$(1)" && mkdir -p "$(1).tmp" && cd "$(1).tmp" && unzip "$(2)" && mv -f "$(1).tmp/$(UNPACKED_DIR)" "$(1)"
endef

define Unpack/cp
	rm -rf "$(1)" && cp -rf "$(2)/$(UNPACKED_DIR)" "$(1)"
endef

define UnpackArchive
	$(call Unpack/$(1),$(2),$(3))
endef