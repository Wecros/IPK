# Author: Marek "Wecros" Filip <xfilip46@fit.vutbr.cz>
# Date: 2020/04/21

LOGIN = xfilip46
SRC_DIR = src
SHELL = bash

.PHONY: run build test pack clean help format analyze analyze-fix install-roslynator

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
	rm -f $(LOGIN).tar

format: --roslynator
	@roslynator format || :

analyze: --roslynator
	@roslynator analyze || :

analyze-fix: --roslynator
	@roslynator fix || :

--roslynator:
	@command -v roslynator >/dev/null || \
	    (echo "'roslynator' not installed, try running install-roslynator." ; exit 1)

install-roslynator:
	dotnet tool install -g roslynator.dotnet.cli

help:
	@echo "make"
	@echo "    Execute 'make run' command."
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
	@echo "make help"
	@echo "    Show this screen."
