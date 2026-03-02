
define NL


endef

define AssertEmpty
ifneq ($(2),)
$$(error $(1): $(2))
endif
endef
