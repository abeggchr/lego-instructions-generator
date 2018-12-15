# Lego Instructions Generator

> Combines images/fotos to an image similar to a Lego building instruction.

## Usage

1. Put all pictures in a folder.
1. Create a `generate.txt` file
1. Open the solution in Visual Studio
1. Adjust the application arguments
1. Run it

### Command line options

* -i, --input         Required. Input path containing 'generate.txt' file an images.
* -o, --output        Output path where the instruction is generated to.
* -v, --verbose       (Default: false) Print input file names to output.
* -x, --startIndex    (Default: 1) Set start index.
* --help              Display this help screen.
* --version           Display version information.

## Examples

* content of `generate.txt` 
* created image

### Single picture

```
P1150025.JPG
-- comment ignored
```

![100](./Samples/100_small.png)

### Parts picture cropped with a 35% border

```
P1150025.JPG
P1150054.JPG 35
```

![101](./Samples/101_small.png)

### Simple parts picture

```
P1150026.JPG
P1150053.JPG
```

![102](./Samples/102_small.png)

### With text

```
P1150027.JPG
P1150052.JPG
6x
```

![103](./Samples/103_small.png)

## Disclaimer

Implemented for my personal use. You will have to change the source code when you use different image sizes, different colors, different file formats etc.
