# `Craftplacer.Windows.VisualStyles`

An API-less implementation of Windows XP Visual Styles.

## Why?

While developing SimpleClassicThemeTaskbar it became apparent that the Windows API is working against us so writing our own parser was the only option.

## How to get started?

Construct `VisualStyle` with a path to the `.msstyles` file you want to parse. Using the `SizeNames` and `ColorNames` properties you can pass those values to `GetColorScheme` which returns a `ColorScheme` to work with.