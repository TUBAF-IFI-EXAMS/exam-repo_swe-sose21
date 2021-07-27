# User section
### Contents
- [Introduction](#introduction-)
- [Preface](#preface-)
- [Command-line interface (CLI)](#command-line-interface-)
- [Markdown Math Element](#markdown-math-element-)

## Introduction [&#x21A5;](#user-section)
This is the user section of the wiki.  
It contains information about the usage of the program and about things to look out for.

## Preface [&#x21A5;](#user-section)
With version 1.2 the program provides support for Markdown documents with the following syntax elements:

- Headlines/Headings up to level 3
- Lists up to level 3
- Blockquotes up to level 3
- Bold and cursive/italic text (seperated & mixed)
- Inline verbatim with single ticks
- Simple text with or without linebreaks
- Paragraphs
- Inline LaTeX math (e.g. `$element$`)
- Other LaTeX commands (not much tested, use at own risk)
- [Custom syntax](#markdown-math-element-) for mathematical objects

## Command-line interface [&#x21A5;](#user-section)
To use the program, run the executable as follows:
``` sh
mdtotex <path to markdown file> [<path to output>]
```

The input has to be a Markdown file (`.md`) while the output path is optional.  
Output is generated as followed:
|Output path|Output location|
|---|---|
|not specified|`inputfolder/latex/<mdname>.tex`|
|`path-to-folder/`|`path-to-folder/<mdname>.tex`|
|`path-to-folder/<file-name>.tex`|`path-to-folder/<file-name>.tex`|


## Markdown Math Element [&#x21A5;](#user-section)
Here you find the syntax of the custom Markdown math element and every parameter that can currently be used.

#### Syntax: `!{type} element !{p1}{p2}...`
&nbsp;
<table>
<caption><code>!{type}</code></caption>
<tr>
<th>type</th>
<th>Description</th>
<th>Options for parameters<sup>[1]</sup> <code>!{p1}{p2}...</code></th>
</tr>
<tr>
<td><code>svfunc</code></td>
<td>Single Variable<br/>Function</td>
<td>

|Type of Output|Parameter|
|---|---|
|Calculated function with given input<br/>(optional precision, default = 2)|`result(<input>)[<precision>]`|
|First root found in given bounds<sup>[2]</sup><br/>(optional precision, default = 2)|`root(<lower>,<upper>)[<precision>]`|
|First derivative|`d1f`|
|Calculated first derivative<br/>with given input<br/>(optional precision, default = 2)|`d1f(<input>)[<precision>]`|
|Second derivative|`d2f`|
|Calculated second<br/>derivative with given input<br/>(optional precision, default = 2)|`d2f(<input>)[<precision>]`|

</td>
</tr>
</table>

> [1] Each output is added as a new line under the converted element.  
> [2] Uses [variant](https://numerics.mathdotnet.com/api/MathNet.Numerics.RootFinding/RobustNewtonRaphson.htm) of the Newton-Raphson algorithm which can fall back to bisection.