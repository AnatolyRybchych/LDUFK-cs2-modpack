

define Download/curl
	rm -rf "$(2)" && mkdir -p "$(dir $(2))" && curl --retry 5 --retry-delay 5  -L --fail "$(1)" > "$(2)"
endef

define Download/git
	rm -rf "$(2)" && mkdir -p "$(dir $(2))" && git clone "$(1)" $(if $(GIT_REF),-b "$(GIT_REF)",) $(2)
endef

define Download/cp
	rm -rf "$(2)" && mkdir -p "$(dir $(2))" && cp -rf "$(1)" "$(2)"
endef

define Download/none
	echo "Donwload is skipped"
endef

define Download
	$(call Download/$(1),$(2),$(3))
endef