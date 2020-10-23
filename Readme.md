# Cross Language 0.0.1.0-prototype (XLang)

## XLC
Console Interpreter which allows for execution of XL Scripts.

### Arguments:
```
XLang Parser Console(xlc) Version: 0.0.1.0-prototype

Commandline Argument Help:
Syntax
         xlc.exe <input-file> <flag> <data0> <...> <flag1> <data0> <...
Flags:
        --help                  Display this help text
        --target | -t           Set Target start function. Default: "DEFAULT.Program.Main"
        --import | -i           Set Imported Namespaces. Default "XL"
```

## XLE
XL Token Explorer Application. Useful for browsing the output of the Parser.
Either Specify File and browse AST.
Or Write script inside the Program and directly execute it.

### Arguments:
```
Syntax
		xle.exe <file or folder>
```

## Examples
Example Scripts.
Can be explored by dragging the whole folder on to 'xle.exe'.
