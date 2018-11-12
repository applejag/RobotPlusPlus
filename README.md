# Robot++
_A parser to compile the new Robot++ code into G1ANT.Robot code._

`master` | `indev`
--- | ---
[![Build status master](https://ci.appveyor.com/api/projects/status/fcskqjp186i3pckb/branch/master?svg=true)](https://ci.appveyor.com/project/jilleJr/robotplusplus/branch/master)|[![Build status indev](https://ci.appveyor.com/api/projects/status/fcskqjp186i3pckb/branch/indev?svg=true)](https://ci.appveyor.com/project/jilleJr/robotplusplus/branch/indev)

## Example
### Input
```
num = 10
hex = num.ToString("x")
dialog(num + " = 0x" + hex)
```

### Output
```g1ant
♥num=10
♥hex=⊂♥num.ToString("x")⊃
dialog message ⊂♥num+" = 0x"+♥hex⊃
```

## Preview from `Core 0.7.18309.1491`

_(Using VS Code to quickly compile and show result in split window)_
![cli-in-vscode](https://user-images.githubusercontent.com/2477952/48354881-144d6e80-e693-11e8-8ebd-a4e27d2332a5.gif)
