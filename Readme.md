# Cross Language 0.1.0.1 (XLang)

## Status
[![Build Status](https://travis-ci.com/ByteChkR/XLang.svg?branch=main)](https://travis-ci.com/ByteChkR/XLang)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/686539719ae04408a281110610176d39)](https://www.codacy.com/gh/ByteChkR/XLang/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ByteChkR/XLang&amp;utm_campaign=Badge_Grade)
[![codecov](https://codecov.io/gh/ByteChkR/XLang/branch/main/graph/badge.svg?token=IRXADCIHYW)](https://codecov.io/gh/ByteChkR/XLang)

## Documentation
[Documentation Page](https://bytechkr.github.io/XLang/index.html)

## XLC
Console Interpreter which allows for execution of XL Scripts.

### Arguments
```
XLang Parser Console(xlc) Version: 0.1.0.1

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

### Arguments
```
Syntax
		xle.exe <file or folder>
```

## Examples
Example Scripts.
Can be explored by dragging the whole folder on to 'xle.exe'.
