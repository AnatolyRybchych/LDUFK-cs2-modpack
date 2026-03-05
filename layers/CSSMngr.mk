
NAME=CSSMngr
VERSION=0.0.0
URL=file://$(TOPDIR)/plugins/CSSMngr
DEPENDS=CounterStrikeSharp

define Compile
	(cd "$(SOURCES)" && dotnet build -c Release)
endef

define Install
	mkdir -p "$(1)"/addons/counterstrikesharp/plugins/CSSMngr/
	cp -rf $(SOURCES)/bin/Release/*/CSSMngr.dll "$(1)"/addons/counterstrikesharp/plugins/CSSMngr/
	cp -rf $(SOURCES)/config/actions "$(1)"/addons/counterstrikesharp/plugins/CSSMngr/
	cp -rf $(SOURCES)/config/commands.json "$(1)"/addons/counterstrikesharp/plugins/CSSMngr/
endef
