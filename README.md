# SCUMSLang

SCUMSLang (pronounced S-C-U-M-S-Lang) is the programming language and the compiler for describing and creating triggers for Starcraft Broodwar/Remastered.

## Under heavy development

This project is under heavy development. Please look at the features below the project is aiming at.

## Language Specification

Please take a look at the [language specification](https://github.com/SCUMSLang/Specification).

## In-Built Types & Enum Types

[See in-built types](https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.Headers/src/System.umsh).

## StarCraft Conditions

[See StarCraft conditions](https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.Headers/src/Conditions).

## StarCraft Enum Types

[See StarCraft enum types](https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.Headers/src/EnumTypes).

## StarCraft Functions

[See StarCraft functions](https://github.com/SCUMSLang/SCUMSLang/blob/main/src/SCUMSLang.Headers/src/Functions).

## File Extensions

**.umsh**

> The extension `.umsh` stands for User Mapping Settings Header. It is a header file to expose an API that can be used in the actual program.

**.umsl**

> The extension `.umsl` stands for User Mapping Settings Language. It is the source file that is getting compiled.

## Planned Features

- Pretty the same features like [LangUMS](https://github.com/LangUMS/langums)
- Written in C#
- Independable state machines but dependent on those they have to be
- Enum Types (e.g. Player.Player1, Unit.ProtossProbe and so on)
- Type Annotations (e.g. int, bool, string, or concrete enum type)
- Function overloading
- Side effectless sleep()
- Better IDE integration
- Chocolatey (package manager) support
