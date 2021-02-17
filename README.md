# SCUMSLang <!-- omit in toc -->

SCUMSLang (pronounced S-C-U-M-S-Lang) is the programming language and the compiler for describing and creating triggers for Starcraft Broodwar/Remastered.

## Under heavy development <!-- omit in toc -->

This project is under heavy development. Please look at the features below the project is aiming at.

## 1. Table of Content

- [1. Table of Content](#1-table-of-content)
- [2. Comparison Table](#2-comparison-table)
  - [2.1. What is difference between compiler and preprocessor?](#21-what-is-difference-between-compiler-and-preprocessor)
- [3. Language Specification](#3-language-specification)
- [4. SCUMSLang API](#4-scumslang-api)
  - [4.1. In-built types and enum types](#41-in-built-types-and-enum-types)
  - [4.2. StarCraft conditions](#42-starcraft-conditions)
  - [4.3. StarCraft enum types](#43-starcraft-enum-types)
  - [4.4. StarCraft functions](#44-starcraft-functions)
- [5. Planned features](#5-planned-features)

## 2. Comparison Table

Property                                     | [SCUMSLang](https://github.com/SCUMSLang/SCUMSLang)                      | [LangUMS](https://github.com/LangUMS/langums)                     | [YATAPI](https://github.com/sethmachine/yatapi)            | [TrigEditPlus](http://www.staredit.net/topic/16517/)                                                                                                                                                        | [TrigEdit](http://www.stormcoast-fortress.net/cntt/software/scmdraft/)
---------------------------------------------|--------------------------------------------------------------------------|-------------------------------------------------------------------|------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------
Implementation Language                      | C#                                                                       | C++                                                               | Python                                                     | C++                                                                                                                                                                                                         | C++
Advertised Editor                            | [Visual Studio Code](https://code.visualstudio.com/)                     | [Visual Studio Code](https://code.visualstudio.com/)              | [PyCharm IDE](https://www.jetbrains.com/pycharm/download/) | [SciTE](https://www.scintilla.org/SciTEDownload.html)≙Lexilla.dll over<br/>[ScmDraft 2](http://www.stormcoast-fortress.net/cntt/software/scmdraft/download/) [plugin](http://www.staredit.net/topic/16517/) | Editor over in-built<br/>[ScmDraft 2](http://www.stormcoast-fortress.net/cntt/software/scmdraft/download/) plugin
Interface Language<sup>1*</sup>              | SCUMSLang (lang)                                                         | LangUMS (lang)                                                    | Python                                                     | Lua                                                                                                                                                                                                         | TrigEdit (lang)
Statically Typed<sup>*2</sup>                | Yes                                                                      | Yes                                                               | Yes                                                        | Yes                                                                                                                                                                                                         | Yes
Variables                                    | int, short, byte, string; overflow detection<br/>Death counter, switches | int (implicit); no overflow detection<br/>Death counter, switches | Death counter, switches                                    | Death counter, switches                                                                                                                                                                                     | Death counter, switches
Triggers                                     | One-to-One, Templates, Machine States                                    | One-to-One, Templates, Machine State                              | One-to-One, Templates                                      | One-to-One, Templates                                                                                                                                                                                       | One-to-One
Syntax Validation                            | TBD: Compiler, IDE                                                       | Compiler, Autocomplete                                            | Preprocessor, IDE                                          | Compiler, Autocomplete                                                                                                                                                                                      | Compiler
Compilation Validation                       | TBD: Yes                                                                 | Yes<sup>*3</sup>                                                  | No!<sup>*4</sup>                                           | Yes                                                                                                                                                                                                         | Yes
Compilation Trigger Integration<sup>*5</sup> | TBD: Paste in TrigEdit<sup>*6</sup>, .scx/.scm injection                 | .scx/.scm injection                                               | Paste in TrigEdit<sup>*6</sup>                             | .scx/.scm injection                                                                                                                                                                                         | .scx/.scm injection
API-Documentation                            | TBD: Yes; IDE-integrated                                                 | Yes                                                               | No; but usage instructions                                 | No; but usage instructions                                                                                                                                                                                  | No
Binary                                       | SCUMSLang.exe                                                            | langums.exe                                                       | -                                                          | TrigEditPlus.sdp                                                                                                                                                                                            | TrigEdit.sdp
Downloads                                    | TBD: GitHub, Chocolately                                                 | GitHub                                                            | GitHub                                                     | StarEdit StarCraft I Database                                                                                                                                                                               | ScmDraft 2
Last Release                                 | TBD                                                                      | Oct 25, 2017                                                      | Mar 7, 2019                                                | Sep 1, 2014                                                                                                                                                                                                 | 2004

<sup>*1</sup> Interface language is the language in which you are actual writing the triggers. If no language is used GUI is assumed.

<sup>*2</sup> Method parameters(, method return types), variables and constants/enumeration members are statically typed and are checked against the preprocessor/compiler.

<sup>*3</sup> Death counter are happily overflowing when compiler generated trigger are using that amount and more of death counter you provided to compiler.

<sup>*4</sup> @[sethmachine](https://github.com/sethmachine): [Does YATAPI check for any compilation or syntax errors?](https://github.com/sethmachine/yatapi#does-yatapi-check-for-any-compilation-or-syntax-errors)

> Currently, no (but a very good idea). The best way to check actual errors will be when copy and pasting to SCMDraft and seeing the TrigEdit complain about certain lines. YATAPI will also happily compile triggers with more than 64 actions or conditions (but this kind of check could be easily added in).

<sup>*5</sup> How are the compiled triggers in the map (.scx, .sxcm) integrated?

<sup>*6</sup> 

1. Copy compiled triggers 
2. Paste it in TrigEdit (Triggers->Trigger Editor) 
3. Press black check button with the blue tile
4. A windows opens to inform you that the compilation finished.

### 2.1. What is difference between compiler and preprocessor?

The pre-processor is a program that runs before the compiler and essentially performs text substitution. Read [here](https://stackoverflow.com/questions/12982106/what-does-preprocessing-exactly-mean-in-compiler) for more informations.

## 3. Language Specification

https://github.com/SCUMSLang/Specification

## 4. SCUMSLang API

### 4.1. In-built types and enum types

https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.UMSLFiles/src/System.umsh

### 4.2. StarCraft conditions

https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.UMSLFiles/src/Conditions

### 4.3. StarCraft enum types

https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.UMSLFiles/src/EnumTypes

### 4.4. StarCraft functions

https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.UMSLFiles/src/Functions

## 5. Planned features

- Pretty the same features like LangUMS
- Written in C#
- Independable state machines but dependent on those they have to be
- Enum types (e.g. Player.Player1, Player.Force1 and so on)
- Statically types (e.g. int, short, byte, bool, string, or concrete enum type)
- Integer pool with overflow detection
- Function overloading
- Side effectless sleep()
- Better IDE integration with in-code API documentation
- Chocolatey (package manager) support
