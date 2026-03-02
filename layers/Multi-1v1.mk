
NAME=CS2-Multi-1v1
VERSION=1.0.0
DEPENDS=CounterStrikeSharp
URL=https://github.com/rockCityMath/CS2-Multi-1v1.git

define Compile
	(cd "$(SOURCES)" && dotnet build -c Release)
endef

define Install
	mkdir -p "$(1)"/addons/counterstrikesharp/plugins/CS2Multi1v1/
	cp -rf $(SOURCES)/bin/Release/*/CS2Multi1v1.dll "$(1)"/addons/counterstrikesharp/plugins/CS2Multi1v1/
endef