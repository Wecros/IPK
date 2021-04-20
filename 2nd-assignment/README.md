# Packet Sniffer

## Makefile Targets
```
$ make help
make
    Execute 'make run' command.
make run
    Build and run the program.
make build
    Build the project. Built files are placed in bin/ and obj/ directories.
make test
    Build and test the project.
make pack
    Create a zip file with the project.
make clean
    Remove all unneeded files (e.g. binaries).
make format
    Format all of the C# source files using roslynator.
make analyze
    Analyze all of the C# source files statically using roslynator.
make analyze-fix
    Fix all of diagnostics found by analyzer using roslynator.
make install-roslynator
    Install roslynator tool using dotnet, used as linter/formatter.
make help
    Show this screen.
```
