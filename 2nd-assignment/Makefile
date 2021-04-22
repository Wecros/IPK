# Author: Marek "Wecros" Filip <xfilip46@fit.vutbr.cz>
# Date: 2020/04/21

LOGIN = xfilip46
SRC_DIR = src
SHELL = bash
TARGET = ipk-sniffer
define FILEHEADER
/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>$(shell date '+%Y/%m/%d')</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

endef
export FILEHEADER

.PHONY: publish run build test pack clean help format analyze analyze-fix install-roslynator prepend-header

publish:
	dotnet publish -r linux-x64 -p:PublishSingleFile=true
	@cp bin/Debug/netcoreapp3.1/linux-x64/publish/$(TARGET) .

run:
	@dotnet run

build:
	dotnet build

test:
	dotnet test

pack: clean
	tar -cf $(LOGIN).tar *

clean:
	rm -rf bin obj
	rm -f $(LOGIN).tar $(TARGET)

format: --roslynator
	@roslynator format || :

analyze: --roslynator
	@roslynator analyze || :

analyze-fix: --roslynator
	@roslynator fix || :

--roslynator:
	@command -v roslynator >/dev/null || \
	    (echo "'roslynator' not installed, try running 'make install-roslynator'." ; exit 1)

install-roslynator:
	dotnet tool install -g roslynator.dotnet.cli

prepend-header:
	@fd --glob '*.cs' | \
	    parallel "(head -n 1 {} | grep -q '/// <author>') || (cat <(echo '$$FILEHEADER') {} | sponge {})"

help:
	@echo "make"
	@echo "    Execute 'make publish' command."
	@echo "make publish"
	@echo "    Build the project as a single binary ipk-file, which is placed in the root directory."
	@echo "make run"
	@echo "    Build and run the program."
	@echo "make build"
	@echo "    Build the project. Built files are placed in bin/ and obj/ directories."
	@echo "make test"
	@echo "    Build and test the project."
	@echo "make pack"
	@echo "    Create a zip file with the project."
	@echo "make clean"
	@echo "    Remove all unneeded files (e.g. binaries)."
	@echo "make format"
	@echo "    Format all of the C# source files using roslynator."
	@echo "make analyze"
	@echo "    Analyze all of the C# source files statically using roslynator."
	@echo "make analyze-fix"
	@echo "    Fix all of diagnostics found by analyzer using roslynator."
	@echo "make install-roslynator"
	@echo "    Install roslynator tool using dotnet, used as linter/formatter."
	@echo "make prepend-header"
	@echo "    Prepend all of the C# source files with doc template."
	@echo "make help"
	@echo "    Show this screen."
